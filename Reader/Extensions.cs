using System;
using System.Collections.Generic;
using System.Text;

namespace Reader
{
    public static class Extensions
    {
        public static T ThrowIfNull<T>(this T o, string paramName) where T : class
        {
            return o ?? throw new ArgumentNullException(paramName);
        }
    }
}
