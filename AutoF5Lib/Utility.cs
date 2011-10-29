using System;
using System.Collections.Generic;
using System.Text;

namespace AutoF5Lib
{
    public static class Utility
    {
        public static bool ArrayContains(string[] array, string value)
        {
            if (array == null || array.Length == 0)
                return false;

            foreach (var item in array)
            {
                if (string.Equals(item, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
