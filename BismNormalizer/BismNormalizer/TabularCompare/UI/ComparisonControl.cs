using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
using EnvDTE80;
using BismNormalizer.TabularCompare.Core;
using System.Runtime.InteropServices;
using System.Linq;

namespace BismNormalizer.TabularCompare.UI
{
    public enum CompareState { NotCompared, Compared, Validated };

    /// <summary>
    /// The main BISM Normalizer comparison control, containing the differences grid, and source/target object definition text boxes.
    /// </summary>
    public partial class ComparisonControl : UserControl
    {
        #region Private variables

        private ComparisonInfo _comparisonInfo;
        private Comparison _comparison;
        private ContextMenu _menuComparisonGrid = new ContextMenu();

        private BismNormalizerPackage _bismNormalizerPackage;
        private EditorPane _editorPane;

        private const string _bismNormalizerCaption = "BISM Normalizer";
        private CompareState _compareState = CompareState.NotCompared;

        #endregion

        #region DiffVariables

        // this is the diff object;
        DiffMatchPatch _diff = new DiffMatchPatch();

        // these are the diffs
        List<Diff> _diffs;

        // chunks for formatting the two RTBs:
        List<Chunk> _chunklistSource;
        List<Chunk> _chunklistTarget;

        // color list:
        Color[] _backColors = new Color[3] { ColorTranslator.FromHtml("#e2f6c5"), ColorTranslator.FromHtml("#ffd6d5"), Color.White, };
        Color[] _backColorsMerge = new Color[3] { ColorTranslator.FromHtml("#e2f6c5"), Color.LightGray, Color.White, };

        public struct Chunk
        {
            public int StartPosition;
            public int Length;
            public Color BackColor;
        }

        #endregion

        #region DPI

        private float _dpiScaleFactor = 1;
        private void Rescale()
        {
            this._dpiScaleFactor = HighDPIUtils.GetDpiFactor();
            if (this._dpiScaleFactor == 1) return;
            float fudgedDpiScaleFactor = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;

            this.Scale(new SizeF(fudgedDpiScaleFactor, fudgedDpiScaleFactor));
            
            this.Font = new Font(this.Font.FontFamily,
                                 this.Font.Size * fudgedDpiScaleFactor,
                                 this.Font.Style);
            //foreach (Control c in HighDPIUtils.GetChildInControl(this)) //.OfType<Button>())
            //{
            //    if (c is SplitContainer)
            //    {
            //        c.Font = new Font(c.Font.FontFamily,
            //                          c.Font.Size * scaleFactor,
            //                          c.Font.Style);
            //    }
            //}
            pnlHeader.Font = new Font(pnlHeader.Font.FontFamily,
                                      pnlHeader.Font.Size * fudgedDpiScaleFactor,
                                      pnlHeader.Font.Style);
            scDifferenceResults.Font = new Font(scDifferenceResults.Font.FontFamily,
                                                scDifferenceResults.Font.Size * fudgedDpiScaleFactor,
                                                scDifferenceResults.Font.Style);

            // set up splitter distance/widths/visibility
            spltSourceTarget.SplitterDistance = Convert.ToInt32(Convert.ToDouble(spltSourceTarget.Width) * 0.5);
            scDifferenceResults.SplitterDistance = Convert.ToInt32(Convert.ToDouble(scDifferenceResults.Height) * 0.74);
            scObjectDefinitions.SplitterDistance = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Width) * 0.5);
            scDifferenceResults.IsSplitterFixed = false;

            pnlHeader.Height = Convert.ToInt32(toolStrip1.Height * 2.3); // Convert.ToInt32(pnlHeader.Height * fudgedDpiScaleFactor * 0.68);
            txtSource.Width = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel1.Width) * 0.8);
            txtSource.Left = Convert.ToInt32(txtSource.Left * fudgedDpiScaleFactor * 0.9);
            txtTarget.Width = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel2.Width) * 0.8);
            txtTarget.Left = Convert.ToInt32(txtTarget.Left * fudgedDpiScaleFactor * 0.9);
            txtSourceObjectDefinition.Width = scObjectDefinitions.Panel1.Width;
            txtSourceObjectDefinition.Height = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel1.Height) * 0.86);
            txtTargetObjectDefinition.Width = scObjectDefinitions.Panel2.Width;
            txtTargetObjectDefinition.Height = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel2.Height) * 0.86);

            if (_dpiScaleFactor > 1) HighDPIUtils.ScaleStreamedImageListByDpi(TreeGridImageList);

            treeGridComparisonResults.ResetColumnWidths(fudgedDpiScaleFactor);
            if (_comparison != null && _bismNormalizerPackage.ValidationOutput != null)
            {
                _bismNormalizerPackage.ValidationOutput.Rescale(fudgedDpiScaleFactor);
            }
        }

        #endregion

        public ComparisonControl()
        {
            InitializeComponent();
        }

        public void LoadFile(string fileName)
        {
            try
            {
                if (File.ReadAllText(fileName) == "")
                {
                    //Blank file not saved to yet
                    return;
                }
                _comparisonInfo = ComparisonInfo.DeserializeBsmnFile(fileName, "BISM Normalizer");

                PopulateSourceTargetTextBoxes();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Error loading file {fileName}\n{exc.Message}\n\nPlease save over this file with a new version.", _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetNotComparedState();
            }
        }

        public void SaveFile(string fileName)
        {
            try
            { 
                this.RefreshSkipSelections();

                XmlSerializer writer = new XmlSerializer(typeof(ComparisonInfo));
                StreamWriter file = new System.IO.StreamWriter(fileName);
                writer.Serialize(file, _comparisonInfo);
                file.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Error saving file {fileName}\n{exc.Message}", _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public EventHandler ComparisonChanged;

        private void TriggerComparisonChanged()
        {
            EventHandler handler = ComparisonChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public BismNormalizerPackage BismNormalizerPackage
        {
            get { return _bismNormalizerPackage; }
            set { _bismNormalizerPackage = value; }
        }

        public EditorPane ComparisonEditorPane
        {
            get { return _editorPane; }
            set { _editorPane = value; }
        }

        private void BismNormalizer_Load(object sender, EventArgs e)
        {
            _comparisonInfo = new ComparisonInfo();
            GetFromAutoCompleteSource();
            GetFromAutoCompleteTarget();
            treeGridComparisonResults.SetupForComparison();
            treeGridComparisonResults.SetObjectDefinitionsCallBack(PopulateObjectDefinitions);
            treeGridComparisonResults.SetCellEditCallBack(TriggerComparisonChanged);

            _menuComparisonGrid.MenuItems.Add("Skip selected objects", new EventHandler(Skip_Select));
            _menuComparisonGrid.MenuItems.Add("Create selected objects Missing in Target", new EventHandler(Create_Select));
            _menuComparisonGrid.MenuItems.Add("Delete selected objects Missing in Source", new EventHandler(Delete_Select));
            _menuComparisonGrid.MenuItems.Add("Update selected objects with Different Definitions", new EventHandler(Update_Select));

            //hdpi
            Rescale();
        }

        private bool ShowConnectionsForm()
        {
            if (this.CompareState != CompareState.NotCompared)
            {
                //just in case user has some selections, store them to the SkipSelections collection
                this.RefreshSkipSelections();
            }

            Connections connForm = new Connections();
            connForm.Dte = _bismNormalizerPackage.Dte;
            connForm.ComparisonInfo = _comparisonInfo;
            connForm.StartPosition = FormStartPosition.CenterParent;
            connForm.DpiScaleFactor = _dpiScaleFactor;
            connForm.ShowDialog();
            if (connForm.DialogResult == DialogResult.OK)
            {
                SetNotComparedState();
                return true;
            }
            else return false;
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void GetFromAutoCompleteSource()
        {
            string serverNameSource = ReverseArray<string>(Settings.Default.SourceServerAutoCompleteEntries.Substring(0,
                Settings.Default.SourceServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()))[0]; //.Reverse().ToArray();
            //_connectionInfoSource = new ConnectionInfo(serverNameSource, Settings.Default.SourceCatalog);
        }

        private void GetFromAutoCompleteTarget()
        {
            string serverNameTarget = ReverseArray<string>(Settings.Default.TargetServerAutoCompleteEntries.Substring(0,
                Settings.Default.TargetServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()))[0];
            //_connectionInfoTarget = new ConnectionInfo(serverNameTarget, Settings.Default.TargetCatalog);
        }

        internal static T[] ReverseArray<T>(T[] array)
        {
            T[] newArray = null;
            int count = array == null ? 0 : array.Length;
            if (count > 0)
            {
                newArray = new T[count];
                for (int i = 0, j = count - 1; i < count; i++, j--)
                {
                    newArray[i] = array[j];
                }
            }
            return newArray;
        }

        private void SetAutoComplete()
        {
            if (!_comparisonInfo.ConnectionInfoSource.UseProject)
            {
                if (Settings.Default.SourceServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoSource.ServerName + "|") > -1)
                {
                    Settings.Default.SourceServerAutoCompleteEntries =
                        Settings.Default.SourceServerAutoCompleteEntries.Remove(
                            Settings.Default.SourceServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoSource.ServerName + "|"),
                            (_comparisonInfo.ConnectionInfoSource.ServerName + "|").Length);
                }
                Settings.Default.SourceServerAutoCompleteEntries += _comparisonInfo.ConnectionInfoSource.ServerName + "|";
                Settings.Default.SourceCatalog = _comparisonInfo.ConnectionInfoSource.DatabaseName;

                Settings.Default.Save();
                GetFromAutoCompleteSource();
            }

            if (!_comparisonInfo.ConnectionInfoTarget.UseProject)
            {
                if (Settings.Default.TargetServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoTarget.ServerName + "|") > -1)
                {
                    Settings.Default.TargetServerAutoCompleteEntries =
                        Settings.Default.TargetServerAutoCompleteEntries.Remove(
                            Settings.Default.TargetServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoTarget.ServerName + "|"),
                            (_comparisonInfo.ConnectionInfoTarget.ServerName + "|").Length);
                }
                Settings.Default.TargetServerAutoCompleteEntries += _comparisonInfo.ConnectionInfoTarget.ServerName + "|";
                Settings.Default.TargetCatalog = _comparisonInfo.ConnectionInfoTarget.DatabaseName;

                Settings.Default.Save();
                GetFromAutoCompleteTarget();
            }
        }

        internal void SetNotComparedState()
        {
            _compareState = CompareState.NotCompared;

            if (_comparison != null)
            {
                _comparison.Disconnect();
            }
            treeGridComparisonResults.Unloading = true;
            treeGridComparisonResults.Nodes.Clear();
            treeGridComparisonResults.Unloading = false;

            if (_bismNormalizerPackage.Dte != null)
            {
                _bismNormalizerPackage.Dte.StatusBar.Text = "";
            }
            txtSourceObjectDefinition.Text = "";
            txtTargetObjectDefinition.Text = "";
            //txtSource.Text = "";
            //txtTarget.Text = "";

            btnCompareTabularModels.Enabled = true;
            ddSelectActions.Enabled = false;
            mnuHideSkipObjects.Enabled = false;
            mnuShowSkipObjects.Enabled = false;
            mnuSkipAllObjectsMissingInSource.Enabled = false;
            mnuDeleteAllObjectsMissingInSource.Enabled = false;
            mnuSkipAllObjectsMissingInTarget.Enabled = false;
            mnuCreateAllObjectsMissingInTarget.Enabled = false;
            mnuSkipAllObjectsWithDifferentDefinitions.Enabled = false;
            mnuUpdateAllObjectsWithDifferentDefinitions.Enabled = false;
            btnValidateSelection.Enabled = false;
            btnUpdate.Enabled = false;
            btnGenerateScript.Enabled = false;
            btnReportDifferences.Enabled = false;

            ClearMessages();

            //Just in case did an AMO comparison and messed up the fonts
            txtSourceObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txtTargetObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        private void SetComparedState()
        {
            _compareState = CompareState.Compared;
            btnCompareTabularModels.Enabled = true;
            ddSelectActions.Enabled = true;
            mnuHideSkipObjects.Enabled = true;
            mnuShowSkipObjects.Enabled = true;
            mnuSkipAllObjectsMissingInSource.Enabled = true;
            mnuDeleteAllObjectsMissingInSource.Enabled = true;
            mnuSkipAllObjectsMissingInTarget.Enabled = true;
            mnuCreateAllObjectsMissingInTarget.Enabled = true;
            mnuSkipAllObjectsWithDifferentDefinitions.Enabled = true;
            mnuUpdateAllObjectsWithDifferentDefinitions.Enabled = true;
            btnValidateSelection.Enabled = true;
            btnUpdate.Enabled = false;
            btnGenerateScript.Enabled = false;
            btnReportDifferences.Enabled = true;
        }

        private void SetValidatedState()
        {
            _compareState = CompareState.Validated;
            btnUpdate.Enabled = true;
            btnGenerateScript.Enabled = true;
        }

        public CompareState CompareState => _compareState;

        public ComparisonInfo ComparisonInfo => _comparisonInfo;

        public void ClearMessages()
        {
            if (_comparison != null && _bismNormalizerPackage.ValidationOutput != null)
            {
                _bismNormalizerPackage.ValidationOutput.ClearMessages(this.ComparisonEditorPane.Window.Handle.ToInt32());
            }
        }

        private void Skip_Select(object sender, EventArgs e)
        {
            treeGridComparisonResults.SkipItems(true);
        }

        private void Create_Select(object sender, EventArgs e)
        {
            treeGridComparisonResults.CreateItems(true);
        }

        private void Update_Select(object sender, EventArgs e)
        {
            treeGridComparisonResults.UpdateItems(true);
        }

        private void Delete_Select(object sender, EventArgs e)
        {
            treeGridComparisonResults.DeleteItems(true);
        }

        private void treeGridComparisonResults_MouseUp(object sender, MouseEventArgs e)
        {
            // Load context menu on right mouse click
            if (e.Button == MouseButtons.Right)
            {
                _menuComparisonGrid.Show(treeGridComparisonResults, new Point(e.X, e.Y));
            }
        }

        private void PopulateObjectDefinitions(string objDefSource, string objDefTarget, ComparisonObjectType objType, ComparisonObjectStatus objStatus)
        {
            if (_comparison.CompatibilityLevel >= 1200)
            {
                try
                {
                    IterateJson(txtSourceObjectDefinition, objDefSource);
                    IterateJson(txtTargetObjectDefinition, objDefTarget);
                }
                catch (Exception)
                {
                    txtSourceObjectDefinition.Text = "";
                    txtSourceObjectDefinition.Text = objDefSource;
                    txtTargetObjectDefinition.Text = "";
                    txtTargetObjectDefinition.Text = objDefTarget;
                }
            }
            else
            {
                FormatAmoDefinitions(objDefSource, objDefTarget, objType);
            }

            #region Difference Highlighting

            if (   objStatus == ComparisonObjectStatus.DifferentDefinitions ||
                  (objStatus == ComparisonObjectStatus.SameDefinition && objType == ComparisonObjectType.Perspective && _comparisonInfo.OptionsInfo.OptionMergePerspectives) ||
                  (objStatus == ComparisonObjectStatus.SameDefinition && objType == ComparisonObjectType.Culture && _comparisonInfo.OptionsInfo.OptionMergeCultures)
               )
            {
                _diffs = _diff.diff_main(objDefSource, objDefTarget);
                _diff.diff_cleanupSemantic(_diffs);
                //_diff.diff_cleanupSemanticLossless(_diffs);
                //_diff.diff_cleanupEfficiency(_diffs);

                //Are we merging perspectives/cultures?
                if (    (objType == ComparisonObjectType.Perspective && _comparisonInfo.OptionsInfo.OptionMergePerspectives) ||
                        (objType == ComparisonObjectType.Culture && _comparisonInfo.OptionsInfo.OptionMergeCultures)
                   )
                {
                    _chunklistSource = CollectChunks(source: true, backColors: _backColorsMerge);
                    _chunklistTarget = CollectChunks(source: false, backColors: _backColorsMerge);

                    //If same definition with merge perspectives/cultures option, just want to highlight differences in target that will not be applied, so do not paint chunks for source
                    if (objStatus == ComparisonObjectStatus.DifferentDefinitions)
                    {
                        PaintChunks(txtSourceObjectDefinition, _chunklistSource);
                    }
                    PaintChunks(txtTargetObjectDefinition, _chunklistTarget);
                }
                else
                {
                    _chunklistSource = CollectChunks(source: true, backColors: _backColors);
                    _chunklistTarget = CollectChunks(source: false, backColors: _backColors);

                    PaintChunks(txtSourceObjectDefinition, _chunklistSource);
                    PaintChunks(txtTargetObjectDefinition, _chunklistTarget);
                }
            }

            #endregion

            //select 1st characters so not scrolled at bottom
            if (txtSourceObjectDefinition.Text != "")
            {
                txtSourceObjectDefinition.SelectionStart = 0;
                txtSourceObjectDefinition.SelectionLength = 0;
                txtSourceObjectDefinition.ScrollToCaret();
            }
            if (txtTargetObjectDefinition.Text != "")
            {
                txtTargetObjectDefinition.SelectionStart = 0;
                txtTargetObjectDefinition.SelectionLength = 0;
                txtTargetObjectDefinition.ScrollToCaret();
            }
        }

        #region Text formatting private methods

        private void IterateJson(RichTextBox textBox, string text)
        {
            System.Diagnostics.Debug.WriteLine("In ColorCodeJson for {0}", textBox.Name);

            textBox.Text = "";

            if (String.IsNullOrEmpty(text))
                return;

            int start = 0;
            int end = 0;
            bool inString = false;

            while ((end = text.IndexOf('"', start + 1)) != -1)
            {
                int length = end - start;

                //following to ensure close bracket gets same color
                if (start > 0)
                {
                    if (inString)
                        length += 1;
                    else
                    {
                        start += 1;
                        length -= 1;
                    }
                }

                Color color = Color.Black;

                if (inString)
                {
                    if (text.Substring(start + length, 1) == ":")
                        color = Color.SteelBlue;
                    else
                        color = Color.Brown;
                }

                AppendText(textBox, color, text.Substring(start, length));

                start = end;
                inString = !inString;
            }

            //close out the last string
            start += 1;
            AppendText(textBox, Color.Black, text.Substring(start, text.Length - start));
        }

        private void AppendText(RichTextBox textBox, Color color, string text)
        {
            int start = textBox.TextLength;
            textBox.AppendText(text);
            int end = textBox.TextLength;

            // Textbox may transform chars, so (end-start) != text.Length
            textBox.Select(start, end - start);
            {
                textBox.SelectionColor = color;
                // could set box.SelectionBackColor, box.SelectionFont too.
            }
            textBox.SelectionLength = 0; // clear
        }

        private List<Chunk> CollectChunks(bool source, Color[] backColors)
        {
            RichTextBox textBox = new RichTextBox();
            textBox.Text = "";

            List<Chunk> chunkList = new List<Chunk>();
            foreach (Diff diff in _diffs)
            {
                if (!source && diff.operation == Operation.DELETE)
                    continue;  // **
                if (source && diff.operation == Operation.INSERT)
                    continue;  // **

                Chunk chunk = new Chunk();

                int length = textBox.TextLength;
                textBox.AppendText(diff.text);

                chunk.StartPosition = length;
                chunk.Length = diff.text.Length;
                chunk.BackColor = backColors[(int)diff.operation];
                chunkList.Add(chunk);
            }
            return chunkList;

        }

        private void PaintChunks(RichTextBox textBox, List<Chunk> theChunks)
        {
            foreach (Chunk chunk in theChunks)
            {
                textBox.Select(chunk.StartPosition, chunk.Length);
                textBox.SelectionBackColor = chunk.BackColor;
            }
        }

        private void FormatAmoDefinitions(string objDefSource, string objDefTarget, ComparisonObjectType objType)
        {
            ClearObjDefFormatting(txtSourceObjectDefinition);
            ClearObjDefFormatting(txtTargetObjectDefinition);

            txtSourceObjectDefinition.Text = objDefSource;
            txtSourceObjectDefinition.SelectAll();
            txtSourceObjectDefinition.SelectionFont = new Font("Lucida Console", 9, FontStyle.Regular);
            if (objType == ComparisonObjectType.Table)
            {
                SetObjDefFontBold("Base Columns:", txtSourceObjectDefinition);
                SetObjDefFontBold("Calculated Columns:", txtSourceObjectDefinition);
                SetObjDefFontBold("Columns:", txtSourceObjectDefinition);
                SetObjDefFontBold("Hierarchies:", txtSourceObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
                SetObjDefFontBold("Partitions:", txtSourceObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Measure)
            {
                SetObjDefFontBold("Expression:", txtSourceObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Kpi)
            {
                SetObjDefFontBold("Expression:", txtSourceObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
                SetObjDefFontBold("Goal:", txtSourceObjectDefinition);
                SetObjDefFontBold("Status:", txtSourceObjectDefinition);
                SetObjDefFontBold("Trend:", txtSourceObjectDefinition);
                SetObjDefFontBold("Status Graphic:", txtSourceObjectDefinition);
                SetObjDefFontBold("Trend Graphic:", txtSourceObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Role)
            {
                SetObjDefFontBold("Permissions:", txtSourceObjectDefinition);
                SetObjDefFontBold("Row Filters:", txtSourceObjectDefinition);
                SetObjDefFontBold("Members:", txtSourceObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Perspective) //Cultures not supported by AMO version
            {
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Action)
            {
                SetObjDefFontBold("Expression:", txtSourceObjectDefinition);
                SetObjDefFontBold("Drillthrough Columns:", txtSourceObjectDefinition);
                SetObjDefFontBold("Report Parameters:", txtSourceObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
            }

            txtTargetObjectDefinition.Text = objDefTarget;
            txtTargetObjectDefinition.SelectAll();
            txtTargetObjectDefinition.SelectionFont = new Font("Lucida Console", 9, FontStyle.Regular);
            if (objType == ComparisonObjectType.Table)
            {
                SetObjDefFontBold("Base Columns:", txtTargetObjectDefinition);
                SetObjDefFontBold("Calculated Columns:", txtTargetObjectDefinition);
                SetObjDefFontBold("Columns:", txtTargetObjectDefinition);
                SetObjDefFontBold("Hierarchies:", txtTargetObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
                SetObjDefFontBold("Partitions:", txtTargetObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Measure)
            {
                SetObjDefFontBold("Expression:", txtTargetObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Kpi)
            {
                SetObjDefFontBold("Expression:", txtTargetObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
                SetObjDefFontBold("Goal:", txtTargetObjectDefinition);
                SetObjDefFontBold("Status:", txtTargetObjectDefinition);
                SetObjDefFontBold("Trend:", txtTargetObjectDefinition);
                SetObjDefFontBold("Status Graphic:", txtTargetObjectDefinition);
                SetObjDefFontBold("Trend Graphic:", txtTargetObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Role)
            {
                SetObjDefFontBold("Permissions:", txtTargetObjectDefinition);
                SetObjDefFontBold("Row Filters:", txtTargetObjectDefinition);
                SetObjDefFontBold("Members:", txtTargetObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Perspective) //Cultures not supported by AMO version
            {
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
            }
            else if (objType == ComparisonObjectType.Action)
            {
                SetObjDefFontBold("Expression:", txtTargetObjectDefinition);
                SetObjDefFontBold("Drillthrough Columns:", txtTargetObjectDefinition);
                SetObjDefFontBold("Report Parameters:", txtTargetObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
            }
        }

        private void ClearObjDefFormatting(RichTextBox txt)
        {
            txt.SelectAll();
            txt.SelectionFont = new Font(txt.SelectionFont.Name, 9, FontStyle.Regular);
            txt.SelectionBackColor = Color.White;
        }

        private void SetObjDefFontBold(string searchString, RichTextBox txt)
        {
            int startSelect;
            startSelect = txt.Text.IndexOf(searchString);
            if (startSelect != -1)
            {
                txt.Select(startSelect, searchString.Length);
                txt.SelectionFont = new Font(txt.SelectionFont.Name, 10, FontStyle.Bold);
            }
        }

        #endregion

        private void InitializeAndCompareTabularModels()
        {
            try
            {
                string sourceTemp = txtSource.Text;
                string targetTemp = txtTarget.Text;

                if (!ShowConnectionsForm()) return;

                PopulateSourceTargetTextBoxes();
                if (sourceTemp != txtSource.Text || targetTemp != txtTarget.Text)
                {
                    // New connections
                    TriggerComparisonChanged();
                    _comparisonInfo.SkipSelections.Clear();
                }

                Cursor.Current = Cursors.WaitCursor;
                this.CompareTabularModels();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void CompareTabularModels()
        {
            if (_bismNormalizerPackage.Dte != null)
            {
                _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - comparing models ...";
            }

            bool userCancelled;
            _comparison = ComparisonFactory.CreateComparison(_comparisonInfo, out userCancelled);

            if (!userCancelled)
            {
                _comparison.ValidationMessage += HandleValidationMessage;
                _comparison.ResizeValidationHeaders += HandleResizeValidationHeaders;
                _comparison.DatabaseDeployment += HandleDatabaseDeployment;
                _comparison.Connect();
                SetAutoComplete();
                _comparison.CompareTabularModels();

                treeGridComparisonResults.Comparison = _comparison;
                treeGridComparisonResults.DataBindComparison();

                SetComparedState();
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - finished comparing models";
                }
                //MessageBox.Show("Finished comparing models", _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshSkipSelections()
        {
            if (_compareState != CompareState.NotCompared && _comparison != null)
            {
                treeGridComparisonResults.RefreshDiffResultsFromGrid();
                _comparison.RefreshSkipSelectionsFromComparisonObjects();
            }
        }

        private void PopulateSourceTargetTextBoxes()
        {
            txtSource.Text = (_comparisonInfo.ConnectionInfoSource.UseProject ? "Project: " + _comparisonInfo.ConnectionInfoSource.ProjectName : "Database: " + _comparisonInfo.ConnectionInfoSource.ServerName + ";" + _comparisonInfo.ConnectionInfoSource.DatabaseName);
            txtTarget.Text = (_comparisonInfo.ConnectionInfoTarget.UseProject ? "Project: " + _comparisonInfo.ConnectionInfoTarget.ProjectName : "Database: " + _comparisonInfo.ConnectionInfoTarget.ServerName + ";" + _comparisonInfo.ConnectionInfoTarget.DatabaseName);

        }

        #region UI click handlers

        private void btnCompareTabularModels_Click(object sender, EventArgs e)
        {
            InitializeAndCompareTabularModels();
        }

        private void mnuHideSkipObjects_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(true);
        }

        private void mnuHideSkipObjectsWithSameDefinition_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(true, sameDefinitionFilter: true);
        }

        private void mnuShowSkipObjects_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(false);
        }

        private void mnuSkipAllObjectsMissingInSource_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.SkipItems(false, ComparisonObjectStatus.MissingInSource);
            SetComparedState();
        }

        private void mnuDeleteAllObjectsMissingInSource_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(false);
            treeGridComparisonResults.DeleteItems(false);
            SetComparedState();
        }

        private void mnuSkipAllObjectsMissingInTarget_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.SkipItems(false, ComparisonObjectStatus.MissingInTarget);
            SetComparedState();
        }

        private void mnuCreateAllObjectsMissingInTarget_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(false);
            treeGridComparisonResults.CreateItems(false);
            SetComparedState();
        }

        private void mnuSkipAllObjectsWithDifferentDefinitions_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.SkipItems(false, ComparisonObjectStatus.DifferentDefinitions);
            SetComparedState();
        }

        private void mnuUpdateAllObjectsWithDifferentDefinitions_Click(object sender, EventArgs e)
        {
            treeGridComparisonResults.ShowHideNodes(false);
            treeGridComparisonResults.UpdateItems(false);
            SetComparedState();
        }

        private void btnValidateSelection_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bismNormalizerPackage.ValidationOutput == null)
                {
                    //this should set up the tool window and then can refer to _bismNormalizerPackage.ValidationOutput
                    _bismNormalizerPackage.InitializeToolWindowInternal(_dpiScaleFactor);
                }
                else
                {
                    _bismNormalizerPackage.ShowToolWindow();
                }

                Cursor.Current = Cursors.WaitCursor;
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - validating ...";
                }
                treeGridComparisonResults.RefreshDiffResultsFromGrid();
                _bismNormalizerPackage.ValidationOutput.ClearMessages(this.ComparisonEditorPane.Window.Handle.ToInt32());
                _bismNormalizerPackage.ValidationOutput.SetImageList(TreeGridImageList);
                _comparison.ValidateSelection();
                SetValidatedState();

                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - finished validating";
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update target {(_comparisonInfo.ConnectionInfoTarget.UseProject ? "project" : "database")}?", _bismNormalizerCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - committing changes ...";
                }

                this.RefreshSkipSelections();
                bool update = _comparison.Update();
                
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - finished committing changes";
                }

                SetNotComparedState();
                if (update && MessageBox.Show($"Updated {(_comparisonInfo.ConnectionInfoTarget.UseProject ? "project " + _comparisonInfo.ConnectionInfoTarget.ProjectName : "database " + _comparisonInfo.ConnectionInfoTarget.DatabaseName)}.\n\nDo you want to refresh the comparison?", _bismNormalizerCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.CompareTabularModels();
                }
            }

            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnGenerateScript_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - creating script ...";
                }

                string script = _comparison.ScriptDatabase(); //doing this here in case errors before opening file in VS
                Document file = NewXmlaFile(_comparison.CompatibilityLevel >= 1200, (_comparisonInfo.ConnectionInfoTarget.UseProject ? _comparisonInfo.ConnectionInfoTarget.ProjectName : _comparisonInfo.ConnectionInfoTarget.DatabaseName));
                if (file != null)
                {
                    TextSelection selection = (TextSelection)file.Selection;
                    selection.SelectAll();
                    selection.Insert(script);
                    selection.GotoLine(1);

                    return;
                }

                //If we get here, there was a problem generating the xmla file (maybe file item templates not installed), so offer saving to a file instead
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (_comparison.CompatibilityLevel >= 1200)
                {
                    saveFile.Filter = "JSON Files|*.json|Text Files|*.txt|All files|*.*";
                }
                else
                {
                    saveFile.Filter = "XMLA Files|*.xmla|Text Files|*.txt|All files|*.*";
                }
                saveFile.CheckFileExists = false;
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, _comparison.ScriptDatabase());

                    if (_bismNormalizerPackage.Dte != null)
                    {
                        _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - finished generating script";
                    }
                    MessageBox.Show("Created script\n" + saveFile.FileName, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                _bismNormalizerPackage.Dte.StatusBar.Text = "";
            }
        }

        private Document NewXmlaFile(bool jsonEditor, string targetName)
        {
            try
            {
                //Generate next file name (if try to get NewFile method to do this by leaving filename param blank, the name will not have custom name and will not have xmla extension)
                int maxFileNameNumber = 1;
                int fileNameNumber;
                string fileName = targetName + "_UpdateScript";
                foreach (Window window in _bismNormalizerPackage.Dte.Windows)
                {
                    if (window.Document != null &&
                        window.Caption != null &&
                        window.Caption.EndsWith(".xmla") &&
                        window.Caption.Replace(".xmla", "").Length > fileName.Length &&
                        window.Caption.Substring(0, fileName.Length) == fileName && 
                        Int32.TryParse(window.Caption.Replace(".xmla", "").Remove(0, fileName.Length), out fileNameNumber)
                       )
                    {
                        if (fileNameNumber >= maxFileNameNumber)
                        {
                            maxFileNameNumber = fileNameNumber + 1;
                        }
                    }
                }

                fileName += Convert.ToString(maxFileNameNumber) + (jsonEditor ? ".json" : ".xmla");
                return _bismNormalizerPackage.Dte.ItemOperations.NewFile(Name: fileName, ViewKind: Constants.vsViewKindCode).Document;
            }
            catch
            {
                return null;
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Options optionsForm = new Options();
            optionsForm.ComparisonInfo = _comparisonInfo;
            optionsForm.StartPosition = FormStartPosition.CenterParent;
            optionsForm.DpiScaleFactor = _dpiScaleFactor;
            optionsForm.ShowDialog();
            if (optionsForm.DialogResult == DialogResult.OK)
            {
                TriggerComparisonChanged();
                if (_compareState != CompareState.NotCompared)
                {
                    SetNotComparedState();
                    _bismNormalizerPackage.Dte.StatusBar.Text = "Comparison invalidated. Please re-run the comparison.";
                }
            }
        }

        private void btnReportDifferences_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - generating report ...";
                }
                pnlProgressBar.Visible = true;
                pnlProgressBar.BringToFront();
                pnlProgressBar.Refresh();
                _comparison.ReportDifferences(progressBar);

                if (_bismNormalizerPackage.Dte != null)
                {
                    _bismNormalizerPackage.Dte.StatusBar.Text = "BISM Normalizer - finished generating report";
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, _bismNormalizerCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                pnlProgressBar.Visible = false;
                Cursor.Current = Cursors.Default;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Shift | Keys.Alt | Keys.C))
            {
                this.InitializeAndCompareTabularModels();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.SetNotComparedState();
            this.ClearMessages();
            base.OnHandleDestroyed(e);
        }

        private void txtSourceObjectDefinition_vScroll(Message message)
        {
            message.HWnd = txtTargetObjectDefinition.Handle;
            txtTargetObjectDefinition.PubWndProc(ref message);
        }

        private void txtTargetObjectDefinition_vScroll(Message message)
        {
            message.HWnd = txtSourceObjectDefinition.Handle;
            txtSourceObjectDefinition.PubWndProc(ref message);
        }

        private void txtSourceObjectDefinition_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown ||
                e.KeyCode == Keys.PageUp ||
                (e.Modifiers == Keys.Control && e.KeyCode == Keys.End) ||
                (e.Modifiers == Keys.Control && e.KeyCode == Keys.Home)
               )
            {
                txtTargetObjectDefinition.SelectionStart = txtSourceObjectDefinition.SelectionStart;
                txtTargetObjectDefinition.ScrollToCaret();
            }
        }

        private void txtTargetObjectDefinition_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown ||
                e.KeyCode == Keys.PageUp ||
                (e.Modifiers == Keys.Control && e.KeyCode == Keys.End) ||
                (e.Modifiers == Keys.Control && e.KeyCode == Keys.Home)
               )
            {
                txtSourceObjectDefinition.SelectionStart = txtTargetObjectDefinition.SelectionStart;
                txtSourceObjectDefinition.ScrollToCaret();
            }
        }

        #endregion

        private void HandleValidationMessage(object sender, ValidationMessageEventArgs e)
        {
            BismNormalizerPackage.ValidationOutput.ShowStatusMessage(
                ComparisonEditorPane.Window.Handle.ToInt32(),
                ComparisonEditorPane.Name,
                e.Message,
                e.ValidationMessageType,
                e.ValidationMessageStatus);
        }

        private void HandleResizeValidationHeaders(object sender, EventArgs e)
        {
            BismNormalizerPackage.ValidationOutput.ResizeValidationHeaders();
        }

        private void HandleDatabaseDeployment(object sender, DatabaseDeploymentEventArgs e)
        {
            Deployment deployForm = new Deployment();
            deployForm.Comparison = _comparison;
            deployForm.ComparisonInfo = _comparisonInfo;
            deployForm.DpiScaleFactor = _dpiScaleFactor;
            deployForm.StartPosition = FormStartPosition.CenterParent;
            deployForm.ShowDialog();
            e.DeploymentSuccessful = (deployForm.DialogResult == DialogResult.OK);
        }

        private void treeGridComparisonResults_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!(e.Exception is ArgumentException)) //ignore ArgumentException because happens on hpi scaling
            {
                throw new Exception(e.Exception.Message, e.Exception);
            }
        }
    }
}
