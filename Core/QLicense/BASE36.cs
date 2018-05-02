using System;
using System.Text;

namespace QLicense
{
    internal class Base36
    {
        private const string charList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly char[] charArray = charList.ToCharArray();

        public static long Decode(string input)
        {
            long result = 0;
            double pow = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                char c = input[i];
                int pos = charList.IndexOf(c);
                if (pos > -1)
                {
                    result += pos * (long) Math.Pow(charList.Length, pow);
                }
                else
                {
                    return -1;
                }

                pow++;
            }

            return result;
        }

        public static string Encode(ulong input)
        {
            var sb = new StringBuilder();
            do
            {
                sb.Append(charArray[input % (ulong) charList.Length]);
                input /= (ulong) charList.Length;
            } while (input != 0);

            return Reverse(sb.ToString());
        }

        private static string Reverse(string s)
        {
            char[] revCharArray = s.ToCharArray();
            Array.Reverse(revCharArray);
            return new string(revCharArray);
        }
    }
}