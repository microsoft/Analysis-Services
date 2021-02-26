# Metadata Translator

Metadata Translator helps to streamline the localization of Power BI data models. The tool can automatically translate the captions, descriptions, and display folder names of tables, columns, measures, and hierarchies by using the machine translation technology of Azure Cognitive Services. It also lets you export and import translations via Comma Separated Values (.csv) files for convenient bulk editing in Excel or a localization tool.

![MetadataTranslator](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/MetadataTranslator.png)

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

 

## Applying translations to a dataset

Metadata Translator detects the default language and all translations in the dataset and reads corresponding the captions, descriptions, and display folder names on startup. As you work with the translation grid, add or remove cultures, perform machine translations, or import translated strings, the changes only affect the data in Metadata Translator. To apply the changes to the dataset, click on the Apply button in the toolbar, and then save the .pbix file in Power BI Desktop to persist the changes.

![Applying translations](https://github.com/microsoft/Analysis-Services/blob/master/MetadataTranslator/Metadata%20Translator/Documentation/Images/Applying%20translations.png)

# Additional features

Metadata Translator v1.0 does not support connections to data models hosted in SQL Server Analysis Services, Azure Analysis Services, or the Power BI service yet. A future version will provide this capability to add translations to an online dataset.

It is also planned to add support for editing the default language strings as soon as the external tools integration feature of Power BI Desktop supports these operations. 

For additional feature requests, create a new item under [Issues](https://github.com/microsoft/Analysis-Services/issues).
