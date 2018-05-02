using System.ComponentModel;
using System.Xml.Serialization;
using QLicense;

namespace DemoLicense
{
    public class MyLicense : LicenseEntity
    {
        public MyLicense()
        {
            //Initialize app name for the license
            AppName = "DemoWinFormApp";
        }

        [DisplayName("Enable Feature 01")]
        [Category("License Options")]
        [XmlElement("EnableFeature01")]
        [ShowInLicenseInfo(true, "Enable Feature 01", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature01 { get; set; }

        [DisplayName("Enable Feature 02")]
        [Category("License Options")]
        [XmlElement("EnableFeature02")]
        [ShowInLicenseInfo(true, "Enable Feature 02", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature02 { get; set; }


        [DisplayName("Enable Feature 03")]
        [Category("License Options")]
        [XmlElement("EnableFeature03")]
        [ShowInLicenseInfo(true, "Enable Feature 03", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature03 { get; set; }

        public override LicenseStatus DoExtraValidation(out string validationMsg)
        {
            var _licStatus = LicenseStatus.Undefined;
            validationMsg = string.Empty;

            switch (Type)
            {
                case LicenseTypes.Single:
                    //For Single License, check whether UID is matched
                    if (UID == LicenseHandler.GenerateUid(AppName))
                    {
                        _licStatus = LicenseStatus.Valid;
                    }
                    else
                    {
                        validationMsg = "The license is NOT for this copy!";
                        _licStatus = LicenseStatus.Invalid;
                    }

                    break;
                case LicenseTypes.Volume:
                    //No UID checking for Volume License
                    _licStatus = LicenseStatus.Valid;
                    break;
                default:
                    validationMsg = "Invalid license";
                    _licStatus = LicenseStatus.Invalid;
                    break;
            }

            return _licStatus;
        }
    }
}