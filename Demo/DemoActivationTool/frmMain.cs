using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using DemoLicense;
using QLicense;
using QLicense.Windows.Controls;

namespace DemoActivationTool
{
    public partial class frmMain : Form
    {
        private byte[] _certPubicKeyData;
        private readonly SecureString _certPwd = new SecureString();

        public frmMain()
        {
            InitializeComponent();

            _certPwd.AppendChar('d');
            _certPwd.AppendChar('e');
            _certPwd.AppendChar('m');
            _certPwd.AppendChar('o');
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //Read public key from assembly
            var _assembly = Assembly.GetExecutingAssembly();
            using (var _mem = new MemoryStream())
            {
                _assembly.GetManifestResourceStream("DemoActivationTool.LicenseSign.pfx").CopyTo(_mem);

                _certPubicKeyData = _mem.ToArray();
            }

            //Initialize the path for the certificate to sign the XML license file
            licSettings.CertificatePrivateKeyData = _certPubicKeyData;
            licSettings.CertificatePassword = _certPwd;

            //Initialize a new license object
            licSettings.License = new MyLicense();
        }

        private void licSettings_OnLicenseGenerated(object sender, LicenseGeneratedEventArgs e)
        {
            //Event raised when license string is generated. Just show it in the text box
            licString.LicenseString = e.LicenseBASE64String;
        }


        private void btnGenSvrMgmLic_Click(object sender, EventArgs e)
        {
            //Event raised when "Generate License" button is clicked. 
            //Call the core library to generate the license
            licString.LicenseString = LicenseHandler.GenerateLicenseBase64String(
                new MyLicense(),
                _certPubicKeyData,
                _certPwd);
        }
    }
}