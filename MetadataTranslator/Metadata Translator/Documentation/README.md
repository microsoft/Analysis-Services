# Metadata Translator

Metadata Translator helps to streamline the localization of Power BI data models. The tool can automatically translate the captions, descriptions, and display folder names of tables, columns, measures, and hierarchies by using the machine translation technology of Azure Cognitive Services. It also lets you export and import translations via Comma Separated Values (.csv) files for convenient bulk editing in Excel or a localization tool.

![MetadataTranslator](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/MetadataTranslator.png)

> Note
>
> Metadata Translator can add translations to datasets hosted in SQL Server Analysis Services, Azure Analysis Services, and Power BI. However, Power BI only supports multiple locales for dataset hosted on Power BI Premium, Premium per User, or a Power BI Embedded SKU. Datasets on shared capacity cannot take advantage of metadata translations yet.

## Installation

Metadata Translator can be installed by using a Windows Installer Package (.msi) file. The Metadata Translator solution includes a Metadata Translator Setup project to build this .msi file. 

<img src="https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Installing%20Metadata%20Translator.png" alt="Installing Metadata Translator" style="zoom:50%;" />

Start the installation by launching the .msi file, accept the default settings or change the installation folder if desired, and then confirm the remaining prompts to complete the installation. To remove Metadata Translator, launch the .msi file again and choose Remove Metadata Translator, or use Add/Remove Programs.

## Starting Metadata Translator

Metadata Translator integrates with Power BI Desktop as an external tool. To start it, open a Power BI Desktop (.pbix) file, switch to the External Tools ribbon, and click on Metadata Translator. Metadata Translator automatically connects to the dataset insides the .pbix file and lets you apply changes to this dataset.

<img src="https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Launching%20Metadata%20Translator.png" alt="Launching Metadata Translator" style="zoom:38%;" />

## Configuration settings

Metadata Translator uses the cloud-based machine translation services of Azure Cognitive Services and requires a subscription key and an endpoint URL to call the Translator REST API. See [Create a Translator resource](https://docs.microsoft.com/azure/cognitive-services/Translator/translator-how-to-signup) in the product documentation. To get started, you'll need an active Azure account. If you don't have one, you can create a free 12-month subscription. For the purposes of Metadata Translator, it is also sufficient to create a free Translator instance in most cases. See [Cognitive Services pricing—Translator](https://azure.microsoft.com/pricing/details/cognitive-services/translator/) for more details.

To register a Translator instance in Metadata Translator, click on the Settings button in the toolbar and provide the required information, which is generated when you provision the Translator resource in the Azure portal. Enable the Overwrite Translation checkbox if you want to replace all existing translations every time you click on Translate.

<img src="https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Settings.png" alt="Settings" style="zoom:38%;" />

> **Note**
>
> Configuring Metadata Translator to use Azure Cognitive Services is optional. The configuration is recommended but not required if you prefer to translate the strings manually.

## Choosing languages

Metadata Translator enables you to choose from over 300 cultures in all languages that Power BI supports. Click on Languages to pick the cultures you want to add to your dataset. Although there is theoretically no limit regarding the number of cultures you can add, it is important to keep in mind that every translation increases the size of the dataset.

![Adding languages](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Adding%20languages.png)

For every language selected in the Language pane, Metadata Translator adds a column to the translation grid. Deselecting a language removes the corresponding translation grid column. 

> **Note**
>
> You cannot remove the default language of the dataset, which is the language marked with an asterisk in the translation grid. 

## Performing a machine translation

Having added a new language, click on the Translate button in the toolbar to fill the corresponding column in the translation grid with the translated strings. If you haven’t registered a translation service endpoint yet, Metadata Translator automatically opens the Settings pane for you to provide the required configuration settings. See Configuration settings earlier in this readme.

![Translate](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Translate.png)

## Reviewing translated strings

The translation grid organizes the translation based on the translated property. Choose Caption, Description, or Display Folder in the toolbar. If the current dataset does not include any descriptions or display folders, the corresponding radio buttons are greyed out.

## Editing translated strings

The translation grid also lets you add translated strings and edit any existing strings manually. Just double-click the desired grid cell and make the changes.

> **Note**
>
> You cannot edit the strings of the default language. The default language, marked with an asterisk, is read-only in the translation grid. 

## Exporting to .CSV files

A dataset typically includes hundreds of strings. For convenient bulk editing in Excel or a localization tool, click on Import/Export to open the Import/Export pane, and then click Export. Be aware that Metadata Translators overwrites any existing files in the export folder without warning. It is recommended to create a new folder for each export to avoid accidental overwrites.![Export](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Export.png)

> **Note**
>
> Metadata Translators exports each culture into a separate .csv file based on the locale identifier (LCID). 

## Importing from .CSV files

To import translated strings from a .csv file, make sure the file name(s) correspond(s) to the locale identifier (LCID) of the target language(s). Click on Import/Export to open the Import/Export pane, and then click Import. Choose the files in the Open dialog box and click Open. 

![Import](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Import.png)

If you import a .csv file for a culture that you haven’t added to the dataset yet, Metadata Translator automatically adds the culture for you and displays the corresponding column in the translation grid with the imported strings.

> **Note**
>
> During the import operation, Metadata Translators first expects a full match of the default strings including their ordering. If the ordering is different, Metadata Translators switches to case-sensitive string matching. Any strings that don’t have an exact match are ignored and the corresponding cells remain empty in the translation grid. 

##  Applying translations to a dataset

Metadata Translator detects the default language and all translations in the dataset and reads corresponding the captions, descriptions, and display folder names on startup. As you work with the translation grid, add or remove cultures, perform machine translations, or import translated strings, the changes only affect the data in Metadata Translator. To apply the changes to the dataset, click on the Apply button in the toolbar, and then save the .pbix file in Power BI Desktop to persist the changes.

![Applying translations](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Applying%20translations.png)

## Connecting to an online dataset

Metadata Translator can also connect to data models hosted in SQL Server Analysis Services, Azure Analysis Services, and the Power BI service so you can add translations to an online dataset. Simply start Metadata Translator directly from the installation folder, type the full dataset connection string in the input form, and then click OK to connect.

![Connection string to a PowerBI dataset](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/ConnectionStringToAPowerBIDataset.png)

> Note

> You must provide the full connection string. This is especially important to keep in mind when connecting to a dataset in the Power BI service. The connection string that Power BI displays on the dataset settings page does not include the Data Source property name. It is an incomplete connection string, such as *powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks*. Make sure to add "Data Source=" in front of it. The screenshot above shows the full connection string: *data source=powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks*.

## Command-line operations

Metadata Translator supports command-line operations through a thin console application called MTCmd.exe so that you can import and export translations in an automated way. You can find MTCmd.exe in the Metadata Translator installation folder. Run MTCmd /? to display available command-line options and operations, as in the following screenshot.

![Metadata Translator command-line app](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/MT%20command-line%20app.png)

### Connecting to a dataset

In order to connect to a dataset hosted in SQL Server Analysis Services, Azure Analysis Services, or the Power BI service, you must specify the full connection string by using the --connection-string parameter (or -cs). See also the previous "*Connecting to an online dataset*" section. The --connection-string parameter is mandatory for all export and import operations.

### Exporting translations

To export existing translations from a dataset, you must specify full path to an export folder by using the --export-folder (-ef) parameter. For example, the following command exports all translations from an AdventureWorks dataset hosted in Power BI into a folder called ExportedTranslations:

`MTCmd -cs "powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks" -ef C:\ExportedTranslations`

> Note
>
> The specified export folder must exist prior to running the command-line app. MTCmd.exe does not create the export folder. However, if any files exist in the export folder, MTCmd.exe might overwrite them without warning. MTCmd.exe exports each culture into a separate .csv file based on the locale identifier (LCID). 

### Generating new translations

The -ef option exports existing translations from a dataset. If you want to add a new translation, you can specify the corresponding locale identifier by using the --locale-id (-lcid) option. For example, the following command generates a translation file in the aforementioned export folder for the locale 'de-DE':

`MTCmd -cs "powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks" -ef C:\ExportedTranslations -lcid "de-DE" `

> Note
>
> If a translation exists for the specified locale identifier, MTCmd exports the existing translated strings. If a translation does not exist, MTCmd generates a translation file without translated strings. You can then add the translations by using a localization tool and import the file as explained in the next section to add the translations to the dataset. For a list of supported locale identifiers, refer to the [supportedlanguages.json]('https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata Translator/Resources/supportedlanguages.json') file under Resources in the Metadata Translator project folder.

### Importing translations

To import translations from a .csv file, you must specify full path to the import file by using the --import-file (-if) parameter. The file name must correspond to the locale identifier (LCID) of the target language. You must also specify the --mode (-m) parameter. Valid options are Import or Overwrite. Import applies translations for strings that have not been translated yet in the dataset. Overwrite, as the name implies, overwrites any existing translations in the dataset. Both, Import and Overwrite create new translations if you import a locale that does not yet exist in the dataset.

The following command imports German translations from a .csv file called de-DE.csv into an AdventureWorks dataset hosted in Power BI, overwriting any existing German strings in the dataset:

`MTCmd -cs "powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks" -if C:\Translations\de-DE.csv -m Overwrite`

> Note
>
> MTCmd.exe only imports one translation file at a time. To import multiple languages, run MTCmd.exe in a loop.

### Working with resx files

While the graphical user interface of Metadata Translator only works with csv files, the command-line tool supports an additional option to work with XML resource (.resx) files as well. Resx files rely on a well-defined XML schema, including a header followed by data in name/value pairs. These files are often used for software localization. Among other things, Visual Studio provides a convenient interface for creating and maintaining resx files. And with command-line support for resx files in Metadata Translator, you can reuse these software translations by applying them to your Power BI datasets. 

#### Csv versus Resx

Conceptually, resx files are slightly more difficult to work with than csv files because software localization and dataset localization use different approaches. In software projects structured for localization, UI elements are typically assigned an identifier, which then maps to an actual value in a resx file. For each supported language, the software project includes a separate resx file (or set of resx files). In this sense, there is no default locale. The identifiers establish the relationship or string mapping, and all supported languages are equal.

In Power BI datasets, on the other hand, named objects don't have identifiers. They hold the strings of the default locale so that measures, relationships, and other metadata elements can use meaningful references and clients connecting without specifying a locale on the connection string get meaningful information, such as human-readable table and column names. Moreover, there is no reliable way to associate the metadata objects in a dataset with a persisted unique identifier. For this reason, Metadata Translator strongly favors csv files because csv permits a direct relationship between the strings of the default locale and the translated strings of an additional locale without the help of a relationship base on an identifier.

The following figure illustrates the differences between csv and resx files for a dataset with a default locale of *en-US* and an additional locale of *it-IT*. Exporting the strings into a csv file produces a single it-IT.csv file containing both the original (default locale) strings and the translated strings. On the other hand, exporting the strings into resx produces two files, one for each locale with a globally unique identifier establishing the relationship between them. If you lost the en-US.resx file, for example, you practically also lost the Italian translations because you no longer have a mapping to the strings of the default locale.

![csv versus resx](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/csv%20versus%20resx.png)

#### Exporting into resx files

Exporting translations from a dataset into resx files is very similar to exporting into csv files. Just specify the ExportResx as the mode of operation. For example, the following command exports all translations into resx files from an AdventureWorks dataset hosted in Power BI into a folder called ExportedTranslations:

`MTCmd -cs "powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks" -ef C:\ExportedTranslations -m ExportResx`

> Note
>
> The specified export folder must exist prior to running the command. MTCmd.exe does not create the export folder. However, if any files exist in the export folder, MTCmd.exe might overwrite them without warning. MTCmd.exe exports each locale into a separate .csv file based on the locale identifier (LCID), including the default locale.

#### Repeatedly exporting into resx files

It is important to note that Metadata Translator generates new globally unique identifiers (GUIDs) every time you export translations from a dataset. There is no reliable way for Metadata Translator to persist previously generated GUIDs with their corresponding metadata objects in the model. It is therefore not recommended to export translations into an existing set of resx files. 

For illustration, imagine the following scenario:

1. You export the translations from a dataset that supports the locales en-US, it-IT, and es-ES. The default locale is en-US. 
2. Metadata Translator creates to following files in the export folder: en-US.resx, it-IT.resx, and es-ES.resx. All three files use the same GUIDs to establish the string relationships. In other words, these three files from a translation set of resx files.
3. You export the Italian translations again by using the -lcid parameter (-lcid it-IT). You use the same export folder. Metadata Translator overwrites the en-US.resx and it-IT.resx files.
4. The existing es-ES.resx file is now orphaned because the GUIDs in the new en-US.resx file, representing the default locale of this dataset, no longer match. 
5. To bring this translation set of resx files back into a consistent state, you must export the full set of locales again. 

> Note
>
> To avoid orphaned resx files, it is a good idea to always export all locales together. If you must export individual languages, make sure you export them into a separate (empty) folder to avoid possibly overwriting an existing resx file of the default locale.

#### Importing translations from resx files

Importing translations from resx files is practically identical to importing from csv files. Simply specify the resx import file by using the --import-file (-if) parameter. Metadata Translator detects the import mode based in the .resx file name extension. For more details, refer to the section "Importing translations" earlier in this document. 

The following command imports German translations from a resx file called de-DE.resx into an AdventureWorks dataset hosted in Power BI, overwriting any existing German strings in the dataset:

`MTCmd -cs "powerbi://api.powerbi.com/v1.0/myorg/AdventureWorksSource;initial catalog=AdventureWorks" -if C:\Translations\de-DE.resx -m Overwrite`

> Note
>
> For a resx import operation to succeed, Metadata Translator requires the resx file of the dataset's default locale to be located in the same folder as the translation resx file. For example, if the above command attempts to import German translations into a dataset with the default locale of en-US, you must have a matching en-US.resx file in the same folder as the de-DE.resx file.

#### Importing translations from external resx files

There is no real difference between resx files generated by Metadata Translator vs. other tools. The GUIDs that Metadata Translator generates merely provide a convenient way to establish unique relationships between strings, but the Name values don't have to be GUIDs necessarily. Metadata Translator can use any kind of name to find a matching translation as long as the name is unique. The resx XML schema enforces uniqueness for the Name property.

Metadata Translator performs the following steps during a resx import operation:

1. Iterate over all the name/value pairs from the resx file of the default locale.
2. Use the name to lookup the corresponding value string in the translation resx file.
3. Use the value strings from the default and translated locales to construct a set of string pairs dynamically.
4. Use the same logic as if the string pairs were from a csv file to apply the translations to the dataset.



## CSV Format for translation files

Metadata Translator translation files use a straightforward schema with only three columns: Type,Original,Translation. Do not delete or add columns or change the column ordering because Metadata Translator will not process translation files that don't follow this schema. Refer to the following table for more details.

| Column      | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| Type        | An optional value to specify the metadata property type to which the translation should apply when Metadata Translator switches to case-sensitive string matching. <br />Supported values are: <br />    Caption<br />    Description<br />    DisplayFolder<br />If you specify an invalid property type, the translation might be ignored. Leave this column blank if you want to apply a translation to all property types across the board. |
| Original    | A string in the default locale of the dataset. Metadata Translator compares the caption, description, or display folder name of a metadata object with this string using case-sensitive string matching to determine if the translation applies to this object. Make sure the captions, descriptions, and display folder names of your metadata objects match the strings in the Original column exactly. |
| Translation | A translated string in the locale of the csv file, as identified by the csv file name, such as de-DE.csv. |

> Note
>
> Ideally, strings in the Original column of a translation file fully match of the captions, descriptions, and display folder names of the metadata objects, including their ordering. However, you can also maintain a global set of translation files that apply to multiple different datasets. In this case, Metadata Translators uses case-sensitive string matching to determine which translation to apply. If you compile translation files by using a separate tool and don't know the property type, leave the Type column empty, but make sure the column exists. 

## Additional features

Metadata Translator v1.3 does not support editing the default language strings because the external tools integration feature of Power BI Desktop does not support these operations yet. 

Metadata Translator v1.3 does not yet support translating synonyms for Power BI Q&A, mainly because Power BI Q&A currently only supports answering natural language queries asked in English. Although there is a preview available for Spanish, the current support policy is English only (see [Supported languages and countries/regions for Power BI - Power BI | Microsoft Docs](https://docs.microsoft.com/power-bi/fundamentals/supported-languages-countries-regions#whats-translated)).

For additional feature requests, create a new item under [Issues](https://github.com/microsoft/Analysis-Services/issues).

