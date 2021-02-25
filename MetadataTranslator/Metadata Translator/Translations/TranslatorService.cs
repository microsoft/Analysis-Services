using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Metadata_Translator
{
    public class TranslatorService
    {
        List<Language> Languages { get; set; }
        string SourceLanguage { get; set; }
        string SubscriptionKey { get; set; }
        string Endpoint { get; set; }
        string Location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceLanguage"></param>
        /// <param name="targetLanguage"></param>
        /// <param name="subscriptionKey"></param>
        /// <param name="endpoint"></param>
        /// <param name="location"></param>
        public TranslatorService(List<Language> languages, string sourceLanguage, string subscriptionKey, string endpoint, string location)
        {
            Languages = languages;
            SourceLanguage = sourceLanguage;

            SubscriptionKey = subscriptionKey;
            Endpoint = endpoint;
            Location = location;
        }

        public void Translate(List<ExpandoObject> dataRows, bool replaceExistingTranslations)
        {
            /// No languages, or only the one source language? Nothing to translate!
            /// 
            if (Languages == null || Languages.Count < 2) return;

            /// Get the target languages (i.e. not the SourceLanguage).
            /// 
            List<Language> targetLanguages = Languages.Where(l => !l.LanguageTag.Equals(SourceLanguage)).ToList();

            /// Filter down the data rows to those with values in the source language.
            /// 
            List<ExpandoObject> filteredRows = dataRows.Where(dr => !string.IsNullOrEmpty(dr.GetValue(SourceLanguage)))?.ToList();

            /// No rows? Nothing to translate!
            /// 
            if (filteredRows == null || filteredRows.Count == 0) return;

            /// Iterate over the TranslationGroups, all languages within the same group
            /// share the same translation id.
            /// 
            foreach(string id in targetLanguages.Select(tl => tl.TranslationId).Distinct())
            {
                Translate(filteredRows, targetLanguages.Where(tl => tl.TranslationId.Equals(id)), id, replaceExistingTranslations);
            }
        }

        private void Translate(List<ExpandoObject> dataRows, IEnumerable<Language> targetLanguages, string translationId, bool replaceExistingTranslations)
        {
            List<ExpandoObject> rowsToTranslate = new List<ExpandoObject>();

            /// Don't replace existing translations? 
            /// Filter down the data rows to those that are empty for at least one of the target languages.
            /// 
            if (!replaceExistingTranslations)
            {
                foreach(Language language in targetLanguages)
                {
                    rowsToTranslate.AddRange(dataRows.Where(dr => string.IsNullOrEmpty(dr.GetValue(language.LanguageTag))));
                }
                rowsToTranslate = rowsToTranslate.Distinct()?.ToList();
            }
            else
            {
                /// Otherwise translate all data rows.
                /// 
                rowsToTranslate = dataRows;
            }

            /// Now translate the source strings recursively.
            /// 
            Translate(rowsToTranslate, targetLanguages, translationId, replaceExistingTranslations, 0);
        }

        private void Translate(List<ExpandoObject> dataRows, IEnumerable<Language> targetLanguages, string translationId, bool replaceExistingTranslations, int iterationId)
        {
            int maxBatchSize = 100;
            int batchStart = maxBatchSize * iterationId;

            /// Check if all strings have been translated.
            /// 
            if (dataRows.Count <= batchStart) return;

            /// Assemble a translation batch of up to maxBatchSize.
            /// 
            maxBatchSize = (dataRows.Count - batchStart) < maxBatchSize? dataRows.Count - batchStart : maxBatchSize;
            List<object> translationBatch = new List<object>();
            for (int i = 0; i < maxBatchSize; i++)
            {
                translationBatch.Add(new { Text = dataRows[batchStart + i].GetValue(SourceLanguage) });
            }

            /// Translate the batch and assign the translated strings to the target languages.
            /// 
            var translatedStrings = TranslateBatch(translationBatch, translationId);
            for (int i = 0; i < maxBatchSize; i++)
            {
                foreach (Language language in targetLanguages)
                {
                    dataRows[batchStart + i].SetValue(language.LanguageTag, translatedStrings[i], replaceExistingTranslations);
                }
            }

            Translate(dataRows, targetLanguages, translationId, replaceExistingTranslations, ++iterationId);
        }

        private List<string> TranslateBatch(List<object> sourceObjects, string targetLanguage)
        {
            List<string> translatedPhrases = new List<string>();

            var requestBody = new JavaScriptSerializer().Serialize(sourceObjects);
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                /// Build the Web request.
                /// 
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri($"{Endpoint}/translate?api-version=3.0&from={SourceLanguage}&to={targetLanguage}");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", Location);

                /// Send the translation request and get the response.
                /// 
                HttpResponseMessage response = client.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result;

                /// Parse the results and add the strings to the translated phrases if there was no error,
                /// i.e. the target language was returned together with the translated string, which is 
                /// not the case if the service gives back an error message.
                /// 
                List<TranslationResult> parsedResults = new JavaScriptSerializer().Deserialize<List<TranslationResult>>(result);
                if (parsedResults != null)
                {
                    for (int n = 0; n < parsedResults.Count; n++)
                    {
                        translatedPhrases.Add((string.IsNullOrEmpty(parsedResults[n].translations[0].to))? "" : parsedResults[n].translations[0].text);
                    }
                }
            }

            return translatedPhrases;
        }
    }
}
