## Purpose 

Running this collection of rules inside [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s [Best Practice Analyzer](https://docs.tabulareditor.com/Best-Practice-Analyzer.html "Best Practice Analyzer") will inform you of potential issues to fix or improvements to be made with regard to performance optimization and model design.

## Setup (automated)

Following these steps will automatically load the Best Practice Rules into your local Tabular Editor. Note that this will overwrite the existing BPARules.json file (if you are already have one).

1. Open [Tabular Editor](https://tabulareditor.com/ "Tabular Editor").
2. Connect to a model.
3. Run the following code in the Advanced Scripting window.

        System.Net.WebClient w = new System.Net.WebClient();

        string userName = System.Environment.UserName;
        string url = "https://raw.githubusercontent.com/m-kovalsky/BestPracticeRules/main/BPARules.json";
        string downloadLoc = @"C:\Users\"+userName+@"\AppData\Local\TabularEditor\BPARules.json";

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

  * Avoid bi-directional relationships against high-cardinality columns*
  * Large tables should be partitioned*
  * Reduce usage of long-length columns with high cardinality**
  * Split date and time*
  
  *Use this [script](https://www.elegantbi.com/post/vertipaqintabulareditor "Script") as documented [here](https://www.elegantbi.com/post/vertipaqintabulareditor "Instructions").
  
  **Use this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_LongLengthColumns.cs "script") in the same fashion as the note above.
  
  ***Use this [script](https://github.com/m-kovalsky/Tabular/blob/master/BestPracticeRule_SplitDateAndTime.cs "script") in the same fashion as the note above.
  
## Requirements

[Tabular Editor](https://tabulareditor.com/ "Tabular Editor") version 2.12.1 or higher
