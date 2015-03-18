#region Copyright
//------------------------------------------------------------------------------
// <copyright file="AboutForm.cs" company="StateForge">
//      Copyright (c) 2010-2012 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Reflection;

    public partial class AboutForm : Form
    {
        private StateBuilderGui gui;
        public AboutForm(StateBuilderGui gui)
        {
            InitializeComponent();
            this.gui = gui;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            var assemblyInfo = new AssemblyInfo<StateBuilderGui>();
            title.Text = assemblyInfo.Product;
            title.Text += "  " + assemblyInfo.Version;
            description.Text = assemblyInfo.Description;
            copyright.Text = assemblyInfo.Copyright;
            if (gui.licenceInfralution.IsLicensed() == true)
            {
                licenceInUse.Text = "This product is licensed";
            }
            else
            {
                licenceInUse.Text = "A trial version is being used: " + gui.licenceInfralution.DaysRemaining + " remaining days";

            }
        }

        private void StateForgeWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = "http://www.stateforge.com";
            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}
