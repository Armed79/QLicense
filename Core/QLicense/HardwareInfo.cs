using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace QLicense
{
    internal class HardwareInfo
    {
        /// <summary>
        ///     Get volume serial number of drive C
        /// </summary>
        /// <returns></returns>
        private static string GetDiskVolumeSerialNumber()
        {
            try
            {
                var disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                disk.Get();
                return disk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Get CPU ID
        /// </summary>
        /// <returns></returns>
        private static string GetProcessorId()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                var mbsList = mbs.Get();
                string id = string.Empty;
                foreach (var mo in mbsList)
                {
                    id = mo["ProcessorId"].ToString();
                    break;
                }

                return id;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Get motherboard serial number
        /// </summary>
        /// <returns></returns>
        private static string GetMotherboardId()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                var mbsList = mbs.Get();
                string id = string.Empty;
                foreach (var mo in mbsList)
                {
                    id = mo["SerialNumber"].ToString();
                    break;
                }

                return id;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
        /// </summary>
        /// <returns></returns>
        public static string GenerateUid(string appName)
        {
            //Combine the IDs and get bytes
            string id = string.Concat(appName, GetProcessorId(), GetMotherboardId(), GetDiskVolumeSerialNumber());
            byte[] byteIds = Encoding.UTF8.GetBytes(id);

            //Use MD5 to get the fixed length checksum of the ID string
            var sha = new SHA1CryptoServiceProvider();
            byte[] checksum = sha.ComputeHash(byteIds);

            //Convert checksum into 4 ulong parts and use BASE36 to encode both
            string part1Id = Base36.Encode(BitConverter.ToUInt32(checksum, 0));
            string part2Id = Base36.Encode(BitConverter.ToUInt32(checksum, 4));
            string part3Id = Base36.Encode(BitConverter.ToUInt32(checksum, 8));
            string part4Id = Base36.Encode(BitConverter.ToUInt32(checksum, 12));

            //Concat these 4 part into one string
            return $"{part1Id}-{part2Id}-{part3Id}-{part4Id}";
        }

        public static byte[] GetUidInBytes(string uid)
        {
            //Split 4 part Id into 4 ulong
            string[] ids = uid.Split('-');

            if (ids.Length != 4)
            {
                throw new ArgumentException("Wrong UID");
            }

            //Combine 4 part Id into one byte array
            byte[] value = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[0])), 0, value, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[1])), 0, value, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[2])), 0, value, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[3])), 0, value, 24, 8);

            return value;
        }

        public static bool ValidateUidFormat(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
            {
                return false;
            }

            string[] ids = uid.Split('-');

            return ids.Length == 4;

        }
    }
}