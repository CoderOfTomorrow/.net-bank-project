using System;

namespace Endava_Project.Server.Helpers
{
    public static class StringGenerator
    {

        public static string Generator(int stringLength)
        {
            Random rnd = new Random();
            const string allowedChars = "0123456789";

            char[] chars = new char[stringLength];
            int setLength = allowedChars.Length;

            for (int i = 0; i < stringLength; ++i)
            {
                chars[i] = allowedChars[rnd.Next(setLength)];
            }

            string randomString = new string(chars, 0, stringLength);

            return randomString;
        }
    }
}
