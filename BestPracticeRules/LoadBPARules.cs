System.Net.WebClient w = new System.Net.WebClient(); 

string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
string downloadLoc = path+@"\TabularEditor\BPARules.json";
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/BPARules.json";
string dlMessage = "Downloaded BPARules.json. Please restart Tabular Editor.";

if (System.IO.File.Exists(downloadLoc))
{
    if (System.Windows.Forms.MessageBox.Show("Would you like to overwrite the existing BPARules.json file?","Overwrite Existing BPA Rules",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
    {
        System.Windows.Forms.MessageBox.Show(dlMessage,"Overwrite Existing BPA Rules",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Information);
    }
    else
    {
        System.Windows.Forms.MessageBox.Show("Did not download BPARules.json.","Overwrite Existing BPA Rules",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Information);
    }
}
else
{
    System.Windows.Forms.MessageBox.Show(dlMessage,"Download BPA Rules",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Information);
    w.DownloadFile(url, downloadLoc);
}