using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public class NoResxMatchesException : Exception
    {
        public string ReferenceResx { get; set; }
        public string TranslationResx { get; set; }

        public NoResxMatchesException(string referenceResx, string translationResx)
        {
            ReferenceResx = referenceResx;
            TranslationResx = translationResx;
        }
    }
}
