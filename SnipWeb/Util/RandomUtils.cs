using System;
using System.Threading;

namespace Snip
{
    public static class RandomUtils
    {
        public static Random GlobalRandom = new Random(Environment.TickCount);
        private static ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(CreateRandom);

        private static Random CreateRandom()
        {
            lock (GlobalRandom)
            {
                return new Random(GlobalRandom.Next());
            }
        }

        private static readonly string allowedCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnpqrstuvqxyz123456789";

        public static string GetString(int length = 8)
        {
            string randomString = string.Empty;

            for (int i = 0; i < length; i++)
            {
                randomString += allowedCharacters[threadRandom.Value.Next(allowedCharacters.Length)];
            }

            return randomString;
        }
    }
}