using System.IO;
using System.Text;
using System.Windows.Forms;

namespace QLicense.Windows.Controls
{
    public partial class LicenseStringContainer : UserControl
    {
        public LicenseStringContainer()
        {
            InitializeComponent();
        }

        public string LicenseString
        {
            get => txtLicense.Text;
            set => txtLicense.Text = value;
        }

        private void lnkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLicense.Text)) Clipboard.SetText(txtLicense.Text);
        }

        private void lnkSaveToFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlgSaveFile.FileName, txtLicense.Text.Trim(), Encoding.UTF8);
        }
    }
}