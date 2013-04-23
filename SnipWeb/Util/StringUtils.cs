using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Snip
{
    public static class StringUtils
    {
        public static bool EqualsI(this string first, string other)
        {
            return first.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string OrIfEmpty(this string first, string other)
        {
            if (string.IsNullOrWhiteSpace(first))
            {
                return other;
            }
            return first;
        }

        public static string GetAliasFromQualifiedName(string qualifiedName)
        {
            if (String.IsNullOrEmpty(qualifiedName))
            {
                return qualifiedName;
            }

            string[] tokens = qualifiedName.Split('\\');
            if (tokens.Length == 1)
            {
                return tokens[0];
            }

            return tokens[1];
        }

        public static string GetFileNameSafeString(this string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '-');
            }
            return s;
        }
    }
}