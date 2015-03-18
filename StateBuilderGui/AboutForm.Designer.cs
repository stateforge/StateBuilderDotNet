#region Copyright
//------------------------------------------------------------------------------
// <copyright file="AboutForm.Designer.cs" company="StateForge">
//      Copyright (c) 2010-2012 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.description = new System.Windows.Forms.Label();
            this.StateForgeWebSite = new System.Windows.Forms.LinkLabel();
            this.licenceInUse = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.copyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(15, 66);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(341, 13);
            this.description.TabIndex = 0;
            this.description.Text = "A finite state machine code generator for .Net languages";
            this.description.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StateForgeWebSite
            // 
            this.StateForgeWebSite.AutoSize = true;
            this.StateForgeWebSite.Location = new System.Drawing.Point(132, 97);
            this.StateForgeWebSite.Name = "StateForgeWebSite";
            this.StateForgeWebSite.Size = new System.Drawing.Size(104, 13);
            this.StateForgeWebSite.TabIndex = 1;
            this.StateForgeWebSite.TabStop = true;
            this.StateForgeWebSite.Text = "www.stateforge.com";
            this.StateForgeWebSite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StateForgeWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StateForgeWebSite_LinkClicked);
            // 
            // licenceInUse
            // 
            this.licenceInUse.Location = new System.Drawing.Point(12, 158);
            this.licenceInUse.Name = "licenceInUse";
            this.licenceInUse.Size = new System.Drawing.Size(344, 27);
            this.licenceInUse.TabIndex = 2;
            this.licenceInUse.Text = "License";
            this.licenceInUse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(110, 206);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(148, 23);
            this.close.TabIndex = 3;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.button1_Click);
            // 
            // title
            // 
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(12, 23);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(344, 25);
            this.title.TabIndex = 4;
            this.title.Text = "StateBuilderDotNet 1.0.0.0";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // copyright
            // 
            this.copyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyright.Location = new System.Drawing.Point(15, 129);
            this.copyright.Name = "copyright";
            this.copyright.Size = new System.Drawing.Size(341, 13);
            this.copyright.TabIndex = 5;
            this.copyright.Text = "Copyright (c) 2010-2012 StateForge";
            this.copyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 255);
            this.Controls.Add(this.copyright);
            this.Controls.Add(this.title);
            this.Controls.Add(this.close);
            this.Controls.Add(this.licenceInUse);
            this.Controls.Add(this.StateForgeWebSite);
            this.Controls.Add(this.description);
            this.Name = "AboutForm";
            this.Text = "AboutForm";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label description;
        private System.Windows.Forms.LinkLabel StateForgeWebSite;
        private System.Windows.Forms.Label licenceInUse;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label copyright;
    }
}