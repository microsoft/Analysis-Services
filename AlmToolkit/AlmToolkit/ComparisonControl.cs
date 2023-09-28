using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using BismNormalizer;
using BismNormalizer.TabularCompare;
using BismNormalizer.TabularCompare.UI;
using BismNormalizer.TabularCompare.Core;
using System.Runtime.InteropServices;
using System.Linq;

namespace AlmToolkit
{
    public enum CompareState { NotCompared, Compared, Validated };

    /// <summary>
    /// The main comparison control, containing the differences grid, and source/target object definition text boxes.
    /// </summary>
    public partial class ComparisonControl : UserControl
    {
        #region Private variables

        private ComparisonInfo _comparisonInfo;
        private Comparison _comparison;
        private ContextMenu _menuComparisonGrid = new ContextMenu();
        private CompareState _compareState = CompareState.NotCompared;

        #endregion

        #region Public properties

        public ComparisonInfo ComparisonInfo
        {
            get { return _comparisonInfo; }
            set { _comparisonInfo = value; }
        }

        public Comparison Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }

        public CompareState CompareState
        {
            get { return _compareState; }
            set { _compareState = value; }
        }

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
            scDifferenceResults.Font = new Font(scDifferenceResults.Font.FontFamily,
                                                scDifferenceResults.Font.Size * fudgedDpiScaleFactor,
                                                scDifferenceResults.Font.Style);

            // set up splitter distance/widths/visibility
            scDifferenceResults.SplitterDistance = Convert.ToInt32(Convert.ToDouble(scDifferenceResults.Height) * 0.74);
            scObjectDefinitions.SplitterDistance = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Width) * 0.5);
            scDifferenceResults.IsSplitterFixed = false;

            txtSourceObjectDefinition.Width = scObjectDefinitions.Panel1.Width;
            txtSourceObjectDefinition.Height = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel1.Height) * 0.86);
            txtTargetObjectDefinition.Width = scObjectDefinitions.Panel2.Width;
            txtTargetObjectDefinition.Height = Convert.ToInt32(Convert.ToDouble(scObjectDefinitions.Panel2.Height) * 0.86);

            if (_dpiScaleFactor > 1) HighDPIUtils.ScaleStreamedImageListByDpi(TreeGridImageList);

            treeGridComparisonResults.ResetColumnWidths(fudgedDpiScaleFactor);
        }

        #endregion

        #region Methods

        public ComparisonControl()
        {
            InitializeComponent();
        }

        private void ComparisonControl_Load(object sender, EventArgs e)
        {
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

        public void TriggerComparisonChanged()
        {
            EventHandler handler = ComparisonChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        public void SetNotComparedState()
        {
            _compareState = CompareState.NotCompared;

            treeGridComparisonResults.Unloading = true;
            treeGridComparisonResults.Nodes.Clear();
            treeGridComparisonResults.Unloading = false;

            txtSourceObjectDefinition.Text = "";
            txtTargetObjectDefinition.Text = "";
            //txtSource.Text = "";
            //txtTarget.Text = "";

            //Just in case did an AMO comparison and messed up the fonts
            txtSourceObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txtTargetObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        public void SetComparedState()
        {
            _compareState = CompareState.Compared;
        }

        public void SetValidatedState()
        {
            _compareState = CompareState.Validated;
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

                // NG: Evaluate if this needs to be added
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
            else if (objType == ComparisonObjectType.CalculationItem)
            {
                SetObjDefFontBold("Expression:", txtSourceObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtSourceObjectDefinition);
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
            else if (objType == ComparisonObjectType.CalculationItem)
            {
                SetObjDefFontBold("Expression:", txtTargetObjectDefinition);
                SetObjDefFontBold("Format & Visibility:", txtTargetObjectDefinition);
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

        public void DataBindComparison()
        {
            treeGridComparisonResults.Comparison = _comparison;
            treeGridComparisonResults.DataBindComparison();
            SetComparedState();
        }

        public void RefreshSkipSelections()
        {
            if (_compareState != CompareState.NotCompared && _comparison != null)
            {
                treeGridComparisonResults.RefreshDiffResultsFromGrid();
                _comparison.RefreshSkipSelectionsFromComparisonObjects();
            }
        }

        public void ShowHideNodes(bool hide, bool sameDefinitionFilter = false)
        {
            treeGridComparisonResults.ShowHideNodes(hide, sameDefinitionFilter);
        }

        /// <summary>
        /// Sets Action property of objects to Skip within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        /// <param name="comparisonStatus"></param>
        public void SkipItems(bool selectedOnly, ComparisonObjectStatus comparisonObjectStatus = ComparisonObjectStatus.Na) //Na because won't take null cos it's an enum
        {
            treeGridComparisonResults.SkipItems(selectedOnly, comparisonObjectStatus);
        }

        /// <summary>
        /// Sets Action property of objects to Delete within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void DeleteItems(bool selectedOnly)
        {
            treeGridComparisonResults.DeleteItems(selectedOnly);
        }

        /// <summary>
        /// Sets Action property of objects to Create within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void CreateItems(bool selectedOnly)
        {
            treeGridComparisonResults.CreateItems(selectedOnly);
        }

        /// <summary>
        /// Sets Action property of objects to Update within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void UpdateItems(bool selectedOnly)
        {
            treeGridComparisonResults.UpdateItems(selectedOnly);
        }

        public void RefreshDiffResultsFromGrid()
        {
            treeGridComparisonResults.RefreshDiffResultsFromGrid();
        }

        #endregion

        #region Event handlers

        public EventHandler ComparisonChanged;

        #region To Delete

        //private Document NewXmlaFile(bool jsonEditor, string targetName)
        //{
        //    try
        //    {
        //        //Generate next file name (if try to get NewFile method to do this by leaving filename param blank, the name will not have custom name and will not have xmla extension)
        //        int maxFileNameNumber = 1;
        //        int fileNameNumber;
        //        string fileName = targetName + "_UpdateScript";
        //        foreach (Window window in _bismNormalizerPackage.Dte.Windows)
        //        {
        //            if (window.Document != null &&
        //                window.Caption != null &&
        //                window.Caption.EndsWith(".xmla") &&
        //                window.Caption.Replace(".xmla", "").Length > fileName.Length &&
        //                window.Caption.Substring(0, fileName.Length) == fileName && 
        //                Int32.TryParse(window.Caption.Replace(".xmla", "").Remove(0, fileName.Length), out fileNameNumber)
        //               )
        //            {
        //                if (fileNameNumber >= maxFileNameNumber)
        //                {
        //                    maxFileNameNumber = fileNameNumber + 1;
        //                }
        //            }
        //        }

        //        fileName += Convert.ToString(maxFileNameNumber) + (jsonEditor ? ".json" : ".xmla");
        //        return _bismNormalizerPackage.Dte.ItemOperations.NewFile(Name: fileName, ViewKind: Constants.vsViewKindCode).Document;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #endregion

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

        private void treeGridComparisonResults_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!(e.Exception is ArgumentException)) //ignore ArgumentException because happens on hpi scaling
            {
                throw new Exception(e.Exception.Message, e.Exception);
            }
        }

        #endregion
    }
}
