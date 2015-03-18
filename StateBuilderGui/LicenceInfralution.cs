#region Copyright
//------------------------------------------------------------------------------
// <copyright file="LicenceInfralution.cs" company="StateForge">
//      Copyright (c) 2010 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge
{
    #region Using Directives
    using System;
    using System.IO;
    using Infralution.Licensing;
    #endregion

    class LicenceInfralution
    {
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// The name of the file to store the license key in - the sub directory is created
        /// automatically by ILS
        /// </summary>
        public string LicenseFile { get; set; }

        public string LicenseParameters { get; set; }

        public int DaysTrial { get; set; }

        public int ExtendedTrialDays { get; set; }

        public int DaysInUse
        {
            get
            {
                return evaluationMonitor.DaysInUse;
            }
        }

        public int DaysRemaining
        {
            get
            {
                if (DaysTrial > DaysInUse)
                {
                    return DaysTrial - DaysInUse;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int UsageCount
        {
            get
            {
                return evaluationMonitor.UsageCount;
            }
        }

        public DateTime FirstUseDate
        {
            get
            {
                return evaluationMonitor.FirstUseDate;
            }
        }


        private EvaluationMonitor evaluationMonitor;

        /// <summary>
        /// The installed license if any
        /// </summary>
        EncryptedLicense _license;

        /// <summary>
        /// License Validation Parameters copied from the License Key Generator 
        /// </summary>
        public LicenceInfralution(string companyName, string productName, string licenseParameters)
        {
            DaysTrial = 30;
            ExtendedTrialDays = 15;
            CompanyName = companyName;
            ProductName = productName;
            LicenseFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + companyName + Path.DirectorySeparatorChar + productName + Path.DirectorySeparatorChar + "LicensedApp.lic";
            LicenseParameters = licenseParameters;
            // check if there is a valid license for the application.
            EncryptedLicenseProvider provider = new EncryptedLicenseProvider();
            _license = provider.GetLicense(LicenseParameters, LicenseFile);
           
            // Create a new RegistryEvaluationMonitor that gives information about days in use, usage count and first date use.
            this.evaluationMonitor = new RegistryEvaluationMonitor(LicenseFile);
            // Reset the trial period
            //this.evaluationMonitor.Reset(true);
        }

        public bool IsLicensed()
        {
            return _license != null;
        }

        /// <summary>
        /// Find out if a valid licence is installed, 
        /// if not, display an EvaluationDialog where user can choose to install the license, continue to use the software, or quit
        /// </summary>
        /// <returns>false if user declines to use this program,</returns>
        public bool processLicence(string[] args)
        {
            // if there is no installed license then display the evaluation dialog until
            // the user installs a license or selects Exit or Continue
            //ts.TraceEvent(TraceEventType.Verbose, 1, "{0} days in use, first use date {1}, usage count {2}", evaluationMonitor.DaysInUse, evaluationMonitor.FirstUseDate, evaluationMonitor.UsageCount);
            while (_license == null)
            {
                Console.WriteLine("{0} days in use, first use date {1}, usage count {2}",
                                  this.evaluationMonitor.DaysInUse,
                                  this.evaluationMonitor.FirstUseDate,
                                  this.evaluationMonitor.UsageCount);

                if ((DaysRemaining > 0) && (args.Length > 0))
                {
                    // When argument are passed to the app, do not showing the licence dialog, e.g:  when building the examples under VS
                    break;
                }
               
                EvaluationDialog evaluationDialog = new EvaluationDialog(evaluationMonitor, ProductName);
                evaluationDialog.TrialDays = DaysTrial;
                evaluationDialog.ExtendedTrialDays = ExtendedTrialDays;

                //Console.WriteLine("{0} days trial extendable to {1}",
                //            evaluationDialog.TrialDays,
                //            evaluationDialog.ExtendedTrialDays);

                EvaluationDialogResult dialogResult = evaluationDialog.ShowDialog();
                if (dialogResult == EvaluationDialogResult.Exit) return false;    // exit the app
                if (dialogResult == EvaluationDialogResult.Continue) break;
                
                if (dialogResult == EvaluationDialogResult.InstallLicense)
                {
                    EncryptedLicenseInstallForm licenseForm = new EncryptedLicenseInstallForm();
                    _license = licenseForm.ShowDialog(LicenseFile, null);
                }
            }
            return true;
        }

        /// <summary>
        /// Handle a click on the Install License button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void installLicenseButton_Click(object sender, EventArgs e)
        {
            EncryptedLicenseInstallForm licenseForm = new EncryptedLicenseInstallForm();
            _license = licenseForm.ShowDialog(LicenseFile, _license);
            //UpdateLicense();
        }
    }
}
