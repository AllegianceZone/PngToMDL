namespace mdlThing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Configuration;



    public class Form1 : Form
    {
        private Button btnClear;
        private Button btnSelectFiles;
        private IContainer components;
        private ContextMenuStrip contextMenuStripFileList;
        private SortedDictionary<string, FileInfoAndSettings> fileList = new SortedDictionary<string, FileInfoAndSettings>();
        private Label label1;
        private Label label2;
        private ToolStripMenuItem mnuItemcreateTextMDLForSelectedToolStripMenuItem;
        private ToolStripMenuItem mnuItemcreateTextMDLForThisFileToolStripMenuItem;
        private ToolStripMenuItem mnuItemremoveSelectedToolStripMenuItem;
        private ToolStripMenuItem mnuItemremoveToolStripMenuItem;
        private ToolStripMenuItem mnuItemView;
        private OpenFileDialog openFileDialog1;
        private SplitContainer splitContainer1;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private RichTextBox txtLog;
        private Button btnOptions;
        private ContextMenuStrip contextMenuStripOptions;
        private ToolStripMenuItem toolStripMenuItemUseTexturesFolder;
        private ToolStripMenuItem toolStripMenuItemUseImagesFolder;
        private Button btnConvertAll;
        private ListView lstFiles;
        private ColumnHeader colFile;
        private ColumnHeader colTransparent;
        private ToolStripMenuItem changeTransparencyToolStripMenuItem;

        private static string fileFormatsFilter = "FreeImage Supported Files | *.BMP;*.DDS;*.EXR;*.GIF;*.HDR;*.ICO;*.IFF;*.JBIG;*.JNG;*.JPEG;*.JIF;*.JPG;*.KOALA;*.MNG;*.PCX;*.PBM;*.PGM;*.PNG;*.PPM;*.PSD;*.RAS;*.SGI;*.TARGA;*.TIFF;*.WBMP;*.XBM;*.XPM";

        public Form1()
        {
            this.InitializeComponent();
            toolStripMenuItemUseTexturesFolder.Checked = Properties.Settings.Default.useTexturesFolder;
            toolStripMenuItemUseImagesFolder.Checked = Properties.Settings.Default.useImagesFolder;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.fileList.Clear();
            this.txtLog.Clear();
            this.updateList();
        }

        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "Select Files";
            this.openFileDialog1.FileName = "";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Filter = fileFormatsFilter;
            this.openFileDialog1.ShowDialog(this);
            int count = this.fileList.Count;
            StringBuilder sb = new StringBuilder();
            foreach (string str in this.openFileDialog1.FileNames)
            {
                if (File.Exists(str))
                {
                    FileInfo info = new FileInfo(str);
                    try
                    {
                        this.fileList.Add(str,new FileInfoAndSettings(info));
                        sb.AppendLine("Added file:///" + str.Replace(@"\", "/").Replace(" ", "%20"));
                        
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
            this.txtLog.AppendText(sb.ToString());
            this.txtLog.AppendText("Added " + ((this.fileList.Count - count)).ToString() + " Files" + Environment.NewLine);
            this.txtLog.SelectionStart = this.txtLog.Text.Length - 1;
            this.txtLog.ScrollToCaret();
            this.updateList();
            if (this.lstFiles.Items.Count > 0)
            {
                this.lstFiles.SelectedIndices.Clear();
            }
        }

        private void createTextMDL(string file,bool useTexturesFolder,bool useImagesFolder,bool isTransparent)
        {
            string str = (Path.GetFileNameWithoutExtension(file) + "bmp.mdl").Replace("bmpbmp","bmp");
            string outputPath = Path.Combine(Application.StartupPath, "Output");
            string imagesPath = "";
            string appendFolder = "";
            if (useTexturesFolder)
            {
                appendFolder = "Textures";
                if (useImagesFolder)
                {
                    appendFolder = appendFolder +"\\" +"Images";
                }
                imagesPath = Path.Combine(outputPath, appendFolder);
            }
            else if (useImagesFolder)
            {
                appendFolder = "Images";
                imagesPath = Path.Combine(outputPath, appendFolder);
            }
            else
            {
                imagesPath = outputPath;
            }
            
            string path = Path.Combine(outputPath, str);
            string str5 = Path.Combine(imagesPath, Path.GetFileName(file));
            try
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (File.Exists(str5))
                {
                    File.Delete(str5);
                }
                File.Copy(file, str5);
                StringBuilder builder = new StringBuilder();
                builder.Append("use \"model\";" + Environment.NewLine);

                builder.AppendFormat("{0}bmp = ImportImageFromFile(\"{1}\", {2});" + Environment.NewLine, Path.GetFileNameWithoutExtension(file), str5.Replace(outputPath + @"\", "").Replace('\\', '/'), isTransparent.ToString().ToLower());

                File.WriteAllText(path, builder.ToString());
                this.txtLog.AppendText(">>Created " + path + " and copied image file to: " + str5 + Environment.NewLine);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            base.Icon = Properties.Resources.furous;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripFileList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemremoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemremoveSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeTransparencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelectFiles = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.colFile = new System.Windows.Forms.ColumnHeader();
            this.colTransparent = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.contextMenuStripOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemUseTexturesFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUseImagesFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConvertAll = new System.Windows.Forms.Button();
            this.contextMenuStripFileList.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripFileList
            // 
            this.contextMenuStripFileList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemView,
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem,
            this.mnuItemremoveToolStripMenuItem,
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem,
            this.mnuItemremoveSelectedToolStripMenuItem,
            this.changeTransparencyToolStripMenuItem});
            this.contextMenuStripFileList.Name = "contextMenuStripFileList";
            this.contextMenuStripFileList.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStripFileList.Size = new System.Drawing.Size(251, 136);
            // 
            // mnuItemView
            // 
            this.mnuItemView.Name = "mnuItemView";
            this.mnuItemView.Size = new System.Drawing.Size(250, 22);
            this.mnuItemView.Text = "View";
            this.mnuItemView.Click += new System.EventHandler(this.mnuItemView_Click);
            // 
            // mnuItemcreateTextMDLForThisFileToolStripMenuItem
            // 
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Name = "mnuItemcreateTextMDLForThisFileToolStripMenuItem";
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Text = "Create Text MDL for this file";
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Click += new System.EventHandler(this.mnuItemcreateTextMDLForThisFileToolStripMenuItem_Click);
            // 
            // mnuItemremoveToolStripMenuItem
            // 
            this.mnuItemremoveToolStripMenuItem.Name = "mnuItemremoveToolStripMenuItem";
            this.mnuItemremoveToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.mnuItemremoveToolStripMenuItem.Text = "Remove";
            this.mnuItemremoveToolStripMenuItem.Click += new System.EventHandler(this.mnuItemremoveToolStripMenuItem_Click);
            // 
            // mnuItemcreateTextMDLForSelectedToolStripMenuItem
            // 
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Name = "mnuItemcreateTextMDLForSelectedToolStripMenuItem";
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Text = "Create Text MDL for Selected files";
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Click += new System.EventHandler(this.mnuItemcreateTextMDLForSelectedToolStripMenuItem_Click);
            // 
            // mnuItemremoveSelectedToolStripMenuItem
            // 
            this.mnuItemremoveSelectedToolStripMenuItem.Name = "mnuItemremoveSelectedToolStripMenuItem";
            this.mnuItemremoveSelectedToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.mnuItemremoveSelectedToolStripMenuItem.Text = "Remove Selected files";
            this.mnuItemremoveSelectedToolStripMenuItem.Click += new System.EventHandler(this.mnuItemremoveSelectedToolStripMenuItem_Click);
            // 
            // changeTransparencyToolStripMenuItem
            // 
            this.changeTransparencyToolStripMenuItem.Name = "changeTransparencyToolStripMenuItem";
            this.changeTransparencyToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.changeTransparencyToolStripMenuItem.Text = "Change Transparency";
            this.changeTransparencyToolStripMenuItem.Click += new System.EventHandler(this.changeTransparencyToolStripMenuItem_Click);
            // 
            // btnSelectFiles
            // 
            this.btnSelectFiles.Location = new System.Drawing.Point(12, 9);
            this.btnSelectFiles.Name = "btnSelectFiles";
            this.btnSelectFiles.Size = new System.Drawing.Size(75, 19);
            this.btnSelectFiles.TabIndex = 1;
            this.btnSelectFiles.Text = "Select Files";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(457, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(47, 19);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(512, 106);
            this.txtLog.TabIndex = 3;
            this.txtLog.Text = "";
            this.txtLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtLog_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(396, 396);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Made by Your_Persona";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 38);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstFiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtLog);
            this.splitContainer1.Size = new System.Drawing.Size(512, 355);
            this.splitContainer1.SplitterDistance = 245;
            this.splitContainer1.TabIndex = 5;
            // 
            // lstFiles
            // 
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFile,
            this.colTransparent});
            this.lstFiles.ContextMenuStrip = this.contextMenuStripFileList;
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.GridLines = true;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(512, 245);
            this.lstFiles.TabIndex = 1;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_Click);
            this.lstFiles.Click += new System.EventHandler(this.lstFiles_Click);
            // 
            // colFile
            // 
            this.colFile.Text = "File";
            this.colFile.Width = 439;
            // 
            // colTransparent
            // 
            this.colTransparent.Text = "Transparent";
            this.colTransparent.Width = 80;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Right click selected items to interact";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(295, 9);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(65, 19);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeselectAll.Location = new System.Drawing.Point(366, 9);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(85, 19);
            this.btnDeselectAll.TabIndex = 2;
            this.btnDeselectAll.Text = "Deselect All";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(121, 7);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(75, 23);
            this.btnOptions.TabIndex = 8;
            this.btnOptions.Text = "Options";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // contextMenuStripOptions
            // 
            this.contextMenuStripOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemUseTexturesFolder,
            this.toolStripMenuItemUseImagesFolder});
            this.contextMenuStripOptions.Name = "contextMenuStripOptions";
            this.contextMenuStripOptions.ShowCheckMargin = true;
            this.contextMenuStripOptions.ShowImageMargin = false;
            this.contextMenuStripOptions.Size = new System.Drawing.Size(177, 48);
            // 
            // toolStripMenuItemUseTexturesFolder
            // 
            this.toolStripMenuItemUseTexturesFolder.Checked = true;
            this.toolStripMenuItemUseTexturesFolder.CheckOnClick = true;
            this.toolStripMenuItemUseTexturesFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItemUseTexturesFolder.Name = "toolStripMenuItemUseTexturesFolder";
            this.toolStripMenuItemUseTexturesFolder.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItemUseTexturesFolder.Text = "Use Textures Folder";
            this.toolStripMenuItemUseTexturesFolder.CheckedChanged += new System.EventHandler(this.toolStripMenuItemUseTexturesFolder_CheckedChanged);
            // 
            // toolStripMenuItemUseImagesFolder
            // 
            this.toolStripMenuItemUseImagesFolder.Checked = true;
            this.toolStripMenuItemUseImagesFolder.CheckOnClick = true;
            this.toolStripMenuItemUseImagesFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItemUseImagesFolder.Name = "toolStripMenuItemUseImagesFolder";
            this.toolStripMenuItemUseImagesFolder.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItemUseImagesFolder.Text = "Use Images Folder";
            this.toolStripMenuItemUseImagesFolder.CheckedChanged += new System.EventHandler(this.toolStripMenuItemUseImagesFolder_CheckedChanged);
            // 
            // btnConvertAll
            // 
            this.btnConvertAll.Location = new System.Drawing.Point(223, 9);
            this.btnConvertAll.Name = "btnConvertAll";
            this.btnConvertAll.Size = new System.Drawing.Size(66, 19);
            this.btnConvertAll.TabIndex = 10;
            this.btnConvertAll.Text = "Convert All";
            this.btnConvertAll.UseVisualStyleBackColor = true;
            this.btnConvertAll.Click += new System.EventHandler(this.btnConvertAll_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(516, 410);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnConvertAll);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSelectFiles);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnSelectAll);
            this.Name = "Form1";
            this.Text = "Mdl Thing 1.3.1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStripFileList.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void launchFile(string filename)
        {
            string[] strArray = filename.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string fileExt = strArray[strArray.Length - 1];
            string fileToOpen = filename;
            launchFile(fileToOpen, fileExt);
        }

        private static void launchFile(string fileToOpen, string fileExt)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(fileToOpen);
                Process.Start(startInfo);
            }
            catch (Exception exception)
            {
                if (exception.Message == "No application is associated with the specified file for this operation")
                {
                    try
                    {
                        ProcessStartInfo info2 = new ProcessStartInfo(@"C:\WINDOWS\system32\rundll32.exe");
                        info2.Arguments = @" C:\WINDOWS\system32\shell32.dll, OpenAs_RunDLL " + fileExt;
                        Process.Start(info2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }                    
                }
            }
        }

        private void lstFiles_Click(object sender, EventArgs e)
        {
            this.mnuItemView.Visible = false;
            this.mnuItemremoveToolStripMenuItem.Visible = false;
            this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Visible = false;
            this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Visible = false;
            this.mnuItemremoveSelectedToolStripMenuItem.Visible = false;
            if (this.lstFiles.Items.Count > 0)
            {
                if (this.lstFiles.SelectedItems.Count > 1)
                {
                    this.mnuItemcreateTextMDLForSelectedToolStripMenuItem.Visible = true;
                    this.mnuItemremoveSelectedToolStripMenuItem.Visible = true;
                }
                else if (this.lstFiles.SelectedItems.Count == 1)
                {
                    this.mnuItemView.Visible = true;
                    this.mnuItemremoveToolStripMenuItem.Visible = true;
                    this.mnuItemcreateTextMDLForThisFileToolStripMenuItem.Visible = true;
                }
            }
        }

        private void mnuItemcreateTextMDLForSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.lstFiles.SelectedItems.Count; i++)
            {
                string file = this.lstFiles.SelectedItems[i].Text;
                this.createTextMDL(fileList[file].Fi.ToString(),
                     Properties.Settings.Default.useTexturesFolder, Properties.Settings.Default.useImagesFolder,
                     fileList[file].Transparent);
            }
        }

        private void changeTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.lstFiles.SelectedItems.Count; i++)
            {
                string file = this.lstFiles.SelectedItems[i].Text;
                fileList[file].Transparent = !fileList[file].Transparent;
                this.lstFiles.SelectedItems[i].SubItems[1].Text = fileList[file].Transparent.ToString(); 
            }
        }

        private void mnuItemcreateTextMDLForThisFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = lstFiles.SelectedItems[0].Text;
            this.createTextMDL(fileList[file].Fi.ToString(),
                      Properties.Settings.Default.useTexturesFolder, Properties.Settings.Default.useImagesFolder,
                      fileList[file].Transparent);
        }

        private void mnuItemremoveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.lstFiles.SelectedItems.Count; i++)
            {
                this.removeItem(this.lstFiles.SelectedItems[i].ToString());
            }
        }

        private void mnuItemremoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileToRemove = this.lstFiles.SelectedItems[0].Text;
            this.removeItem(fileToRemove);
        }



        private void mnuItemView_Click(object sender, EventArgs e)
        {
            string filename = this.lstFiles.SelectedItems[0].Text;
            this.launchFile(filename);
        }

        private void removeItem(string fileToRemove)
        {
            this.txtLog.AppendText("Removed file:///" + fileToRemove.Replace(@"\", "/").Replace(" ", "%20") + Environment.NewLine);
            this.fileList.Remove(fileToRemove);
            this.updateList();
        }

        private void txtLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string linkText = e.LinkText;
            this.launchFile(linkText);
        }

        private void updateList()
        {
            this.lstFiles.Items.Clear();
            foreach (KeyValuePair<string, FileInfoAndSettings> pair in this.fileList)
            {
                ListViewItem lvi = new ListViewItem(pair.Key);
                lvi.SubItems.Add(pair.Value.Transparent.ToString());
                this.lstFiles.Items.Add(lvi);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            this.lstFiles.SelectedIndices.Clear();
            for (int i = 0; i < lstFiles.Items.Count; i++)
            {
                lstFiles.SelectedIndices.Add(i);
            }
            lstFiles.Focus();
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            this.lstFiles.SelectedIndices.Clear();
            lstFiles.Focus();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Button sndr = sender as Button;
            if (sndr != null)
            {
                Point p = new Point(15,15);
                contextMenuStripOptions.Show(sndr,p, ToolStripDropDownDirection.BelowRight);
            }
        }

        private void toolStripMenuItemUseTexturesFolder_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.useTexturesFolder = toolStripMenuItemUseTexturesFolder.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemUseImagesFolder_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.useImagesFolder = toolStripMenuItemUseImagesFolder.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnConvertAll_Click(object sender, EventArgs e)
        {
            btnSelectAll_Click(sender, e);
            mnuItemcreateTextMDLForSelectedToolStripMenuItem_Click(sender, e);
        }


    }


    public class FileInfoAndSettings
    {
        FileInfo fi;
        bool transparent = true;

        public bool Transparent
        {
            get { return transparent; }
            set { transparent = value; }
        }

        public FileInfo Fi
        {
            get { return fi; }
            set { fi = value; }
        }

        public FileInfoAndSettings(FileInfo fii)
        {
            this.Constructor(fii, true);
        }
        public FileInfoAndSettings(FileInfo fii,bool isTransparent)
        {            
            this.Constructor(fii, isTransparent);
        }       
        private void Constructor(FileInfo fii, bool isTransparent)
        {
            this.Fi = fii;
            this.Transparent = isTransparent;
        }
    }
}

