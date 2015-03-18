#region Copyright
//------------------------------------------------------------------------------
// <copyright file="StateBuilder.Designer.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    #region Using Directives
    using System.Windows.Forms;

    #endregion

    partial class StateBuilderGui
    {
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem installLicenceToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button buttonGenerateCode;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button buttonBrowseFsmFile;
        private Label label2;
        private CheckBox checkBoxNoObserver;
        private Button buttonBrowsePrependFile;
        private Button buttonBrowseOutputDirectory;
        private TextBox textBoxPrependFile;
        private Label label1;

        #region Windows Form Designer generated code

        private System.Windows.Forms.Label _v1Label;
        private System.Windows.Forms.Label _v2Label;
        private System.Windows.Forms.TextBox textBoxOutputDirectory;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._v1Label = new System.Windows.Forms.Label();
            this._v2Label = new System.Windows.Forms.Label();
            this.textBoxOutputDirectory = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.installLicenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonGenerateCode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxInputFiles = new System.Windows.Forms.ComboBox();
            this.buttonBrowseFsmFile = new System.Windows.Forms.Button();
            this.buttonBrowsePrependFile = new System.Windows.Forms.Button();
            this.buttonBrowseOutputDirectory = new System.Windows.Forms.Button();
            this.textBoxPrependFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxNoObserver = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _v1Label
            // 
            this._v1Label.Location = new System.Drawing.Point(6, 26);
            this._v1Label.Name = "_v1Label";
            this._v1Label.Size = new System.Drawing.Size(224, 20);
            this._v1Label.TabIndex = 1;
            this._v1Label.Text = "Finite State Machine";
            this.toolTip1.SetToolTip(this._v1Label, "Select the finite state machine in XML format.");
            // 
            // _v2Label
            // 
            this._v2Label.Location = new System.Drawing.Point(6, 43);
            this._v2Label.Name = "_v2Label";
            this._v2Label.Size = new System.Drawing.Size(224, 20);
            this._v2Label.TabIndex = 3;
            this._v2Label.Text = "Output directory";
            this.toolTip1.SetToolTip(this._v2Label, "Select the directory where the source code will be generated, the default directo" +
                    "ry is the directory where the input file is located.");
            // 
            // textBoxOutputDirectory
            // 
            this.textBoxOutputDirectory.Location = new System.Drawing.Point(9, 66);
            this.textBoxOutputDirectory.Name = "textBoxOutputDirectory";
            this.textBoxOutputDirectory.Size = new System.Drawing.Size(643, 20);
            this.textBoxOutputDirectory.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(793, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.installLicenceToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // installLicenceToolStripMenuItem
            // 
            this.installLicenceToolStripMenuItem.Name = "installLicenceToolStripMenuItem";
            this.installLicenceToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.installLicenceToolStripMenuItem.Text = "Install Licence...";
            this.installLicenceToolStripMenuItem.Click += new System.EventHandler(this.installLicenseButton_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::StateForge.Properties.Resources.Help;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // buttonGenerateCode
            // 
            this.buttonGenerateCode.Location = new System.Drawing.Point(12, 309);
            this.buttonGenerateCode.Name = "buttonGenerateCode";
            this.buttonGenerateCode.Size = new System.Drawing.Size(767, 54);
            this.buttonGenerateCode.TabIndex = 6;
            this.buttonGenerateCode.Text = "Generate Code";
            this.toolTip1.SetToolTip(this.buttonGenerateCode, "Generate the finite state machine source code from its XML description.");
            this.buttonGenerateCode.UseVisualStyleBackColor = true;
            this.buttonGenerateCode.Click += new System.EventHandler(this.buttonGenerateCode_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxInputFiles);
            this.groupBox1.Controls.Add(this.buttonBrowseFsmFile);
            this.groupBox1.Controls.Add(this._v1Label);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 94);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // comboBoxInputFiles
            // 
            this.comboBoxInputFiles.FormattingEnabled = true;
            this.comboBoxInputFiles.Location = new System.Drawing.Point(21, 46);
            this.comboBoxInputFiles.Name = "comboBoxInputFiles";
            this.comboBoxInputFiles.Size = new System.Drawing.Size(631, 21);
            this.comboBoxInputFiles.TabIndex = 10;
            this.comboBoxInputFiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxInputFiles_SelectedIndexChanged);
            // 
            // buttonBrowseFsmFile
            // 
            this.buttonBrowseFsmFile.Location = new System.Drawing.Point(661, 46);
            this.buttonBrowseFsmFile.Name = "buttonBrowseFsmFile";
            this.buttonBrowseFsmFile.Size = new System.Drawing.Size(103, 23);
            this.buttonBrowseFsmFile.TabIndex = 5;
            this.buttonBrowseFsmFile.Text = "Browse ...";
            this.buttonBrowseFsmFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFsmFile.Click += new System.EventHandler(this.openFsmFile_Click);
            // 
            // buttonBrowsePrependFile
            // 
            this.buttonBrowsePrependFile.Location = new System.Drawing.Point(661, 112);
            this.buttonBrowsePrependFile.Name = "buttonBrowsePrependFile";
            this.buttonBrowsePrependFile.Size = new System.Drawing.Size(103, 23);
            this.buttonBrowsePrependFile.TabIndex = 9;
            this.buttonBrowsePrependFile.Text = "Browse ...";
            this.buttonBrowsePrependFile.UseVisualStyleBackColor = true;
            this.buttonBrowsePrependFile.Click += new System.EventHandler(this.buttonBrowsePrependFile_Click);
            // 
            // buttonBrowseOutputDirectory
            // 
            this.buttonBrowseOutputDirectory.Location = new System.Drawing.Point(661, 64);
            this.buttonBrowseOutputDirectory.Name = "buttonBrowseOutputDirectory";
            this.buttonBrowseOutputDirectory.Size = new System.Drawing.Size(103, 23);
            this.buttonBrowseOutputDirectory.TabIndex = 8;
            this.buttonBrowseOutputDirectory.Text = "Browse ...";
            this.buttonBrowseOutputDirectory.UseVisualStyleBackColor = true;
            this.buttonBrowseOutputDirectory.Click += new System.EventHandler(this.buttonBrowseOutputDirectory_Click);
            // 
            // textBoxPrependFile
            // 
            this.textBoxPrependFile.Location = new System.Drawing.Point(9, 112);
            this.textBoxPrependFile.Name = "textBoxPrependFile";
            this.textBoxPrependFile.Size = new System.Drawing.Size(643, 20);
            this.textBoxPrependFile.TabIndex = 6;
            this.textBoxPrependFile.TextChanged += new System.EventHandler(this.textBoxPrependFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = "Choose a file to prepend";
            this.label1.Location = new System.Drawing.Point(6, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Prepend file";
            this.toolTip1.SetToolTip(this.label1, "Prepend a file into the generated code, typically used to add a Copyright. ");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonBrowsePrependFile);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonBrowseOutputDirectory);
            this.groupBox2.Controls.Add(this.checkBoxNoObserver);
            this.groupBox2.Controls.Add(this.textBoxPrependFile);
            this.groupBox2.Controls.Add(this.textBoxOutputDirectory);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this._v2Label);
            this.groupBox2.Location = new System.Drawing.Point(12, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(767, 153);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // checkBoxNoObserver
            // 
            this.checkBoxNoObserver.AutoSize = true;
            this.checkBoxNoObserver.Location = new System.Drawing.Point(9, 19);
            this.checkBoxNoObserver.Name = "checkBoxNoObserver";
            this.checkBoxNoObserver.Size = new System.Drawing.Size(174, 17);
            this.checkBoxNoObserver.TabIndex = 3;
            this.checkBoxNoObserver.Text = "Do not generate observer code";
            this.toolTip1.SetToolTip(this.checkBoxNoObserver, "Do not generate observer code slightly increases performance and decreases code s" +
                    "ize but reduces logging information.");
            this.checkBoxNoObserver.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(793, 22);
            this.statusStrip1.TabIndex = 9;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(198, 17);
            this.toolStripStatusLabel.Text = "Select a finite state machine to build";
            // 
            // StateBuilderGui
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(793, 401);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonGenerateCode);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StateBuilderGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StateBuilder for .Net";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private ToolStripMenuItem exitToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ComboBox comboBoxInputFiles;
        private ToolTip toolTip1;
        private System.ComponentModel.IContainer components;

    }
}
