# Best Practice Rules

Make sure to also check out the [PowerBI.com blog post](https://powerbi.microsoft.com/en-us/blog/best-practice-rules-to-improve-your-models-performance/ "PowerBI.com blog post") on this topic!

## Purpose 

Running this collection of rules inside [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s [Best Practice Analyzer](https://docs.tabulareditor.com/Best-Practice-Analyzer.html "Best Practice Analyzer") will inform you of potential issues to fix or improvements to be made with regard to performance optimization and model design.

## Feedback

We would love to hear feedback on how this tool has helped your organization. Please email feedback to: pbibestpractice@microsoft.com.

If you find any issues or have any requests for new rules, please [submit an issue](https://github.com/microsoft/Analysis-Services/issues "submit an issue") within this repository. Just prefix the issue with "BPARules" to make it easier to track.

## Setup (automated)

Following these steps will automatically load the Best Practice Rules into your local Tabular Editor. Note that this will overwrite the existing BPARules.json file (if you are already have one).

1. Open [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
2. Connect to a model.
3. Run the following code in the Advanced Scripting window.

        System.Net.WebClient w = new System.Net.WebClient(); 
        
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/BPARules.json";
        string downloadLoc = path+@"\TabularEditor\BPARules.json";
        
        w.DownloadFile(url, downloadLoc);

4. Close and reopen [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
5. Connect to a model.
6. Select 'Tools' from the File menu and select 'Best Practice Analyzer'.
7. Click the Refresh icon (in blue).

## Setup (manual)

1. Open [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
2. Download the BPARules.json file from GitHub (in this repository).
3. Within the Start Menu, type %localappdata% and click Enter.
4. Navigate to the 'TabularEditor' folder.
5. Copy the rules file (.json) and paste it into the TabularEditor folder.*
6. Open Tabular Editor and connect to your model.
7. Select 'Tools' from the File menu and select 'Best Practice Analyzer'.
8. Click the Refresh icon (in blue).

## Notes

* The following rules require running an additional script before running the Best Practice Analyzer

  * Avoid bi-directional relationships against high-cardinality columns *
  * Large tables should be partitioned *
  * Reduce usage of long-length columns with high cardinality *^
  * Split date and time ***
  
  *These rules use [Vertipaq Analyzer](https://www.sqlbi.com/tools/vertipaq-analyzer/) data. There are 2 methods to load this data into Tabular Editor:
 
  1. Load Vertipaq Analyzer data directly from a server ([instructions](https://www.elegantbi.com/post/vertipaqintabulareditor)) ([script](https://github.com/m-kovalsky/Tabular/blob/master/VertipaqAnnotations.cs)).
  
  2. Load Vertipaq Analyzer data from .vpax file ([instructions](https://www.elegantbi.com/post/vpaxtotabulareditor)) ([script](https://github.com/m-kovalsky/Tabular/blob/master/VpaxToTabularEditor.cs)).
  
  ^Run this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_LongLengthColumns.cs "script") while live-connected to the model.
  
  ***Run this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_SplitDateAndTime.cs "script") while live-connected to the model.
  
## Requirements

[Tabular Editor](https://tabulareditor.com/ "Tabular Editor") version 2.12.1 or higher.
