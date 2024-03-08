Make sure to also check out the [PowerBI.com blog post](https://powerbi.microsoft.com/en-us/blog/best-practice-rules-to-improve-your-models-performance/ "PowerBI.com blog post") on this topic!

Check out the [PowerBI.com blog post on v1.1](https://powerbi.microsoft.com/en-us/blog/best-practice-rules-to-improve-your-models-performance-and-design-v1-1/ "PowerBI.com blog post on v1.1").

Check out [this ElegantBI.com blog post](https://www.elegantbi.com/post/bestpracticerulesavings) to learn how to quantify the savings of following specific Best Practice Rules.

[![image](https://user-images.githubusercontent.com/29556918/131373174-19f31ecb-67b3-4515-83be-f6687d442053.jpg)](https://www.elegantbi.com/post/bestpracticerulesavings)

Check out [this ElegantBI.com blog post](https://www.elegantbi.com/post/exportbparesults) to learn how to export the Best Practice Analyzer results into Excel.

[![image](https://user-images.githubusercontent.com/29556918/136688637-f638405e-0bda-462d-8d95-00a0037000c6.jpg)](https://www.elegantbi.com/post/exportbparesults)

Lastly, check out [this YouTube video](https://www.youtube.com/watch?v=5pu9FoTUpys) for an in-depth walkthrough of the Best Practice Rules and [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s [Best Practice Analyzer](https://docs.tabulareditor.com/Best-Practice-Analyzer.html "Best Practice Analyzer").

<a href="https://www.youtube.com/watch?v=5pu9FoTUpys
" target="_blank"><img src="http://i3.ytimg.com/vi/5pu9FoTUpys/hqdefault.jpg" 
alt="IMAGE ALT TEXT HERE" width="240" height="180" border="10" /></a>

## Purpose 

Running this collection of rules inside [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s [Best Practice Analyzer](https://docs.tabulareditor.com/Best-Practice-Analyzer.html "Best Practice Analyzer") will inform you of potential issues to fix or improvements to be made with regard to performance optimization and model design.

## Compatibility

These best practice rules are compatible with any incarnation of tabular models - be it a model in Power BI Desktop, Power BI Dataset, Power BI Premium, SQL Server Analysis Services, or Azure Analysis Services. The only [requirement](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules#requirements) is using [Tabular Editor](https://tabulareditor.com/ "Tabular Editor") (version 2 or 3).

## Feedback

We would love to hear feedback on how this tool has helped your organization. Please email feedback to: pbibestpractice@microsoft.com.

If you find any issues or have any requests for new rules, please [submit an issue](https://github.com/microsoft/Analysis-Services/issues "submit an issue") within this repository. Just prefix the issue with "BPARules" to make it easier to track.

## Rule Details

The rules are divided into categories (i.e. Performance, DAX Expressions, Error Prevention, Formatting, Maintenance etc.) for easier viewing. Additionally, each rule has a description and many of the rules also have a reference article/video. Reading the rule description and article will provide context as to why the rule is important and why one should follow it. The rule descriptions are accessible by navigating to 'Tools' -> 'Manage BPA Rules...' -> 'Rules for the local user' -> Click on a rule -> Click 'Edit rule...'.

<img width="400" alt="RuleDescription35" src="https://user-images.githubusercontent.com/29556918/132206247-b2f73948-b976-4351-9830-a62f4145f263.png">

## Setup (automated)

Following these steps will automatically load the Best Practice Rules into your local Tabular Editor. Note that this will overwrite the existing BPARules.json file (if you are already have one) so be sure to back up your existing rules file.

1. Open [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
2. Connect to a model.
3. Run the following code in the [Advanced Scripting](https://docs.tabulareditor.com/Advanced-Scripting.html "Advanced Scripting") window.

```C#  
System.Net.WebClient w = new System.Net.WebClient(); 

string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/BPARules.json";
string version = System.Windows.Forms.Application.ProductVersion.Substring(0,1);
string downloadLoc = path+@"\TabularEditor\BPARules.json";

if (version == "3")
{
    downloadLoc = path+@"\TabularEditor3\BPARules.json";
}

w.DownloadFile(url, downloadLoc);
```

*Note: If you want to load the rules in Italian, Japanese or Spanish, replace the url parameter in the code above with the appropriate code below.*
```C#
// Italian
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/Italian/BPARules.json";

// Japanese
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/Japanese/BPARules.json";

// Spanish
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/Spanish/BPARules.json";
```

4. Close and reopen [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
5. Connect to a model.
6. Select 'Tools' from the File menu and select 'Best Practice Analyzer'.
7. Click the Refresh icon (in blue).

## Setup (manual)

1. [Download the BPARules.json file from this repository (right-click and select "Save link as...")](https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/BPARules.json).
2. Within the Start Menu, type %localappdata% and click Enter.
3. Navigate to the 'TabularEditor' folder (if using Tabular Editor 3, navigate to the TabularEditor3 folder).
4. Copy the rules file (.json) and paste it into the folder from Step 3.*
5. Open [Tabular Editor](https://tabulareditor.com/ "Tabular Editor") and connect to your model.
6. Select 'Tools' from the File menu and select 'Best Practice Analyzer'.
7. Click the Refresh icon (in blue).

## Notes

* The following rules require running an additional script before running the Best Practice Analyzer

  * Avoid bi-directional relationships against high-cardinality columns *
  * Large tables should be partitioned *
  * Reduce usage of long-length columns with high cardinality *^
  * Split date and time ***
  * Fix referential integrity violations *
  
  *These rules use [Vertipaq Analyzer](https://www.sqlbi.com/tools/vertipaq-analyzer/) data. There are 2 methods to load this data into Tabular Editor:
 
  1. Load Vertipaq Analyzer data directly from a server ([instructions](https://www.elegantbi.com/post/vertipaqintabulareditor)) ([script](https://github.com/m-kovalsky/Tabular/blob/master/VertipaqAnnotations.cs)).
  
  2. Load Vertipaq Analyzer data from .vpax file ([instructions](https://www.elegantbi.com/post/vpaxtotabulareditor)) ([script](https://github.com/m-kovalsky/Tabular/blob/master/VpaxToTabularEditor.cs)).
  
  ^Run this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_LongLengthColumns.cs "script") while live-connected to the model.
  
  ***Run this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_SplitDateAndTime.cs "script") while live-connected to the model.
  
## Requirements

[Tabular Editor](https://tabulareditor.com/ "Tabular Editor") version 2.16.1 or higher.

## Languages

* English
* [Italian](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Italian)
* [Japanese](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Japanese)
* [Spanish](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Spanish)

*Note: If you would like to volunteer to translate the Best Practice Rules into another language, please contact us at pbibestpractice@microsoft.com.*

## Version History

* 2023-06-20 Version 1.2.6
    * New Rules
        *  [Error Prevention] Set IsAvailableInMdx to true on necessary columns
        *  [Performance] Avoid using many-to-many relationships on tables used for dynamic row level security
    * Updated Rules
        * [Maintenance] Remove unnecessary columns
          * Rule now accounts for columns/tables used in Object Level Security (OLS)  
* 2023-02-14 Version 1.2.5
    * New Rules
        * [Error Prevention] Avoid invalid characters in names
	  * [Error Prevention] Avoid invalid characters in descriptions
	  * [Naming Conventions] Trim object names ([#193](https://github.com/microsoft/Analysis-Services/pull/193))
    * Updated Rules
        * [DAX Expressions] Avoid using '1-x/y)' syntax
	     * Updated recommendation to use accurate calculation ([#192](https://github.com/microsoft/Analysis-Services/issues/192))
	* [Maintenance] Remove data sources not referenced by any partitions
	     * Updated rule logic to account for M source expressions and M queries ([#197](https://github.com/microsoft/Analysis-Services/issues/197))
* 2023-01-16 Version 1.2.4
    * Fixed a bug in the Set IsAvailableInMdx Rule [#190](https://github.com/microsoft/Analysis-Services/issues/190)
* 2023-01-09 
    * The Best Practice Rules are now available in [Spanish](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Spanish)!
* 2022-11-22 Version 1.2.3
    * The Best Practice Rules are now available in [Japanese](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Japanese)!
    * New Rules
        * [DAX Expressions] The EVALUATEANDLOG function should not be used in production models ([#163](https://github.com/microsoft/Analysis-Services/issues/163))
    * Updated Rules
        * [Performance] Check if dynamic row level security (RLS) is necessary
	      * Updated the scope of the rule to Table Permissions ([#178](https://github.com/microsoft/Analysis-Services/issues/178))
        * [Performance] Minimize Power Query transformations
	      * Removed 'select' from the Native.Query function portion of the rule
        * [Performance] Remove redundant columns in related tables
	       * Added description
        * [Formatting] Provide format string for "Date" columns
	       * Updated description
        * [DAX Expressions] No two measures should have the same definition
	       * Added description
        * [Error Prevention] Data columns must have a source column
	       * Updated description
* 2022-06-06 Version 1.2.2
    * New Rules
        * [DAX Expressions] Avoid using the '1-(x/y)' syntax
        * [Error Prevention] Avoid the USERELATIONSHIP function and RLS against the same table
        * [Error Prevention] Relationship columns should be of the same data type
    * Updated Rules
        * [Formatting] Percentage formatting
        	* Fixed a bug within the fix expression   
* 2021-10-21 
    * The Best Practice Rules are now available in [Italian](https://github.com/microsoft/Analysis-Services/tree/master/BestPracticeRules/Italian)!
* 2021-08-18 Version 1.2.1
    * New Rules
        * [Maintenance] Fix referential integrity violations ([blog post](https://www.elegantbi.com/post/findblankrows)) 
* 2021-07-26 Version 1.2
    * New Rules
        * [Error Prevention] Avoid structured data sources with provider partitions ([blog post](https://www.elegantbi.com/post/convertdatasources))
    * Modified Rules
        * [DAX Expressions] Column references should be fully qualified
			* Scope: Added 'Table Permissions'
        *  [Formatting] Hide fact table columns
			* Simplified rule logic
		* [Performance] Check if dynamic row level security (RLS) is necessary
			* Simplified rule logic
		* [Performance] Limit row level security (RLS) logic
			* Simplified rule logic
		* [Formatting] Add data category for columns
			* Fixed rule logic
		* [Performance] Measures using time intelligence and model is using Direct Query
			* Simplified rule logic
		* [Performance] Reduce usage of calculated columns that use the RELATED function
			* Simplified rule logic
		* [Performance] Reduce usage of long-length columns with high cardinality
		    * Updated rule logic to use Int64
* 2021-06-13 Version 1.1.2
    * Modified Rules
        * [DAX Expressions] Use the DIVIDE function for division
        	* Updated the rule logic to not mistake comments for division
* 2021-05-26 Version 1.1.1
    * Modified Rules
        * [DAX Expressions] Inactive relationships that are never activated
        	* Expanded the scope to include Calculation Items ([#110](https://github.com/microsoft/Analysis-Services/issues/110)) 
* 2021-05-20 Version 1.1 (make sure to read the [blog post](https://powerbi.microsoft.com/en-us/blog/best-practice-rules-to-improve-your-models-performance-and-design-v1-1/ "blog post"))
    * New Rules
        * [DAX Expressions] Filter column values with proper syntax
        * [DAX Expressions] Fllter measure values by columns, not tables
        * [DAX Expressions] Inactive relationships that are never activated
        * [Maintenance] Perspectives with no objects
        * [Maintenance] Calculation groups with no calculation items
    * Modified Rules
        * [Naming Conventions] Partition name should match table name for single partition tables
        	* Added Fix Expression (must use Tabular Editor 2.16.1 or higher)
        * [Error Prevention] Calculated columns must have an expression
        	* New name: Expression-reliant objects must have an expression
        * [Maintenance] Objects with no description
        	* New name: Visible objects with no description
    * Removed Rules
        * [DAX Expressions] No two measures should have the same definition
* 2021-02-03 Version 1.0
