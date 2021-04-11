using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUtilities {
    public static class StringUtil {
        public static bool ContainsUpper(string toCheck)
        {
            foreach(var letter in toCheck)
            {
                if (letter > 64 && letter < 91)
                    return true;
            }
            return false;
        }

        public static bool ContainsLower(string toCheck)
        {
            foreach (var letter in toCheck)
            {
                if (letter > 96 && letter < 123)
                    return true;
            }
            return false;
        }

        public static bool ContainsNumber(string toCheck)
        {
            foreach (var letter in toCheck)
            {
                if (letter > 47 && letter < 58)
                    return true;
            }
            return false;
        }

        public static bool ContainsSpecial(string toCheck)
        {
            foreach (var letter in toCheck)
            {
                if (letter > 32 && letter < 48)
                    return true;
                if (letter > 57 && letter < 65)
                    return true;
                if (letter > 90 && letter < 97)
                    return true;
                if (letter > 122 && letter < 127)
                    return true;
            }
            return false;
        }
    }
}
