using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace QLicense.Windows.Controls
{
    public partial class LicenseInfoControl : UserControl
    {
        public LicenseInfoControl()
        {
            InitializeComponent();
        }

        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public void ShowTextOnly(string text)
        {
            txtLicInfo.Text = text.Trim();
        }

        public void ShowLicenseInfo(LicenseEntity license)
        {
            ShowLicenseInfo(license, string.Empty);
        }


        public void ShowLicenseInfo(LicenseEntity license, string additionalInfo)
        {
            try
            {
                var _sb = new StringBuilder(512);

                var _typeLic = license.GetType();
                var _props = _typeLic.GetProperties();

                object _value = null;
                var _formatedValue = string.Empty;
                foreach (var _p in _props)
                    try
                    {
                        var _showAttr =
                            (ShowInLicenseInfoAttribute) Attribute.GetCustomAttribute(_p,
                                typeof(ShowInLicenseInfoAttribute));
                        if (_showAttr != null && _showAttr.ShowInLicenseInfo)
                        {
                            _value = _p.GetValue(license, null);
                            _sb.Append(_showAttr.DisplayAs);
                            _sb.Append(": ");

                            //Append value and apply the format   
                            if (_value != null)
                            {
                                switch (_showAttr.DataFormatType)
                                {
                                    case ShowInLicenseInfoAttribute.FormatType.String:
                                        _formatedValue = _value.ToString();
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.Date:
                                        if (_p.PropertyType == typeof(DateTime) &&
                                            !string.IsNullOrWhiteSpace(DateFormat))
                                            _formatedValue = ((DateTime) _value).ToString(DateFormat);
                                        else
                                            _formatedValue = _value.ToString();
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.DateTime:
                                        if (_p.PropertyType == typeof(DateTime) &&
                                            !string.IsNullOrWhiteSpace(DateTimeFormat))
                                            _formatedValue = ((DateTime) _value).ToString(DateTimeFormat);
                                        else
                                            _formatedValue = _value.ToString();
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.EnumDescription:
                                        var _name = Enum.GetName(_p.PropertyType, _value);
                                        if (_name != null)
                                        {
                                            var _fi = _p.PropertyType.GetField(_name);
                                            var _dna = (DescriptionAttribute) Attribute.GetCustomAttribute(_fi,
                                                typeof(DescriptionAttribute));
                                            if (_dna != null)
                                                _formatedValue = _dna.Description;
                                            else
                                                _formatedValue = _value.ToString();
                                        }
                                        else
                                        {
                                            _formatedValue = _value.ToString();
                                        }

                                        break;
                                }

                                _sb.Append(_formatedValue);
                            }

                            _sb.Append("\r\n");
                        }
                    }
                    catch
                    {
                        //Ignore exeption
                    }


                if (string.IsNullOrWhiteSpace(additionalInfo)) _sb.Append(additionalInfo.Trim());

                txtLicInfo.Text = _sb.ToString();
            }
            catch (Exception ex)
            {
                txtLicInfo.Text = ex.Message;
            }
        }
    }
}