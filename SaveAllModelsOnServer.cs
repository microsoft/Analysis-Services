#r "Microsoft.AnalysisServices.Core.dll"
using ToM = Microsoft.AnalysisServices.Tabular;

string folderPath = @"C:\Desktop\MyFolder"; // Folder where the files are saved
string saveType = "B"; // Use 'B' for saving to .bim files, use 'F' for saving to folder structure

// If saving datasets from Power BI Premium, enter your Service Principal credentials in the 3 parameters below:
string appID = "";
string tenantID = "";
string appSecret = "";

string serverName = Model.Database.TOMDatabase.Server.ToString();
string cmdText = @"start /wait /d ""C:\Program Files (x86)\Tabular Editor"" TabularEditor.exe " + @"""";
bool pbiPrem = false;

if (Model.DefaultPowerBIDataSourceVersion == PowerBIDataSourceVersion.PowerBI_V3)
{
    pbiPrem = true;
}
// Update cmdText for Power BI Premium datasets (v3)
if (pbiPrem)
{
    cmdText = cmdText + @"Provider=MSOLAP;Data Source=powerbi://api.powerbi.com/v1.0/myorg/" + serverName + ";User ID=app:" + appID + "@" + tenantID + ";Password=" + appSecret + @"""";
}
else
{
    cmdText = cmdText + serverName + @"""";   
}

foreach (var x in Model.Database.TOMDatabase.Server.Databases)
{
    string dbName = x.ToString();
    string fullCmdText = cmdText + @" """ + dbName + @""" -" + saveType + " " + @"""" + folderPath + @"\" + dbName;
    
    if (saveType == "B")
    {
        fullCmdText = fullCmdText + @".bim""";
    }

    if (pbiPrem && appID.Length == 0)
    {
        Error("Must enter the Application ID in the appID parameter.");
        return;
    }
    else if (pbiPrem && tenantID.Length == 0)
    {
        Error("Must enter the Tenant ID in the tenantID parameter.");
        return;
    }
    else if (pbiPrem && appSecret.Length == 0)
    {
        Error("Must enter the Application Secret in the appSecret parameter.");
        return;
    }
    else if (saveType != "B" && saveType != "F")
    {
        Error("The saveType paramter must be a value of 'B' or 'F' only.");
        return;
    }
    else
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd");
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = fullCmdText;
        process.StartInfo = startInfo;    
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.StandardInput.WriteLine(fullCmdText);
        process.StandardInput.Flush();
        process.StandardInput.Close();
        process.WaitForExit();
    }
}