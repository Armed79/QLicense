using System;
using System.IO;
using System.Windows.Forms;
using DemoLicense;

namespace DemoWinFormApp
{
    public partial class frmActivation : Form
    {
        public frmActivation()
        {
            InitializeComponent();
        }

        public byte[] CertificatePublicKeyData { private get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to cancel?", string.Empty, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes) Close();
        }

        private void frmActivation_Load(object sender, EventArgs e)
        {
            //Assign the application information values to the license control
            licActCtrl.AppName = "DemoWinFormApp";
            licActCtrl.LicenseObjectType = typeof(MyLicense);
            licActCtrl.CertificatePublicKeyData = CertificatePublicKeyData;
            //Display the device unique ID
            licActCtrl.ShowUID();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Call license control to validate the license string
            if (licActCtrl.ValidateLicense())
            {
                //If license if valid, save the license string into a local file
                File.WriteAllText(Path.Combine(Application.StartupPath, "license.lic"), licActCtrl.LicenseBASE64String);

                MessageBox.Show("License accepted, the application will be close. Please restart it later",
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
        }
    }
}