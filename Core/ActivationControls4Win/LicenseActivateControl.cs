using System;
using System.Windows.Forms;

namespace QLicense.Windows.Controls
{
    public partial class LicenseActivateControl : UserControl
    {
        public LicenseActivateControl()
        {
            InitializeComponent();

            ShowMessageAfterValidation = true;
        }

        public string AppName { get; set; }

        public byte[] CertificatePublicKeyData { private get; set; }

        public bool ShowMessageAfterValidation { get; set; }

        public Type LicenseObjectType { get; set; }

        public string LicenseBASE64String => txtLicense.Text.Trim();

        public void ShowUID()
        {
            txtUID.Text = LicenseHandler.GenerateUid(AppName);
        }

        public bool ValidateLicense()
        {
            if (string.IsNullOrWhiteSpace(txtLicense.Text))
            {
                MessageBox.Show("Please input license", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Check the activation string
            var licStatus = LicenseStatus.Undefined;
            var msg = string.Empty;
            var lic = LicenseHandler.ParseLicenseFromBase64String(LicenseObjectType, txtLicense.Text.Trim(),
                CertificatePublicKeyData, out licStatus, out msg);
            switch (licStatus)
            {
                case LicenseStatus.Valid:
                {
                    if (ShowMessageAfterValidation)
                    {
                        MessageBox.Show(msg, "License is valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    return true;
                }

                case LicenseStatus.Cracked:
                case LicenseStatus.Invalid:
                case LicenseStatus.Undefined:
                {
                    if (ShowMessageAfterValidation)
                    {
                        MessageBox.Show(msg, "License is INVALID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return false;
                }

                default:
                    return false;
            }
        }


        private void lnkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtUID.Text);
        }
    }
}