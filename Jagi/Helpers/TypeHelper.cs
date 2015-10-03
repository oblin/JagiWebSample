using System;

namespace Jagi.Helpers
{
    public static class TypeHelper
    {
        public static bool IsNumericOrNull(this Type type)
        {
            if (type.IsNumeric()
                || type.Equals(typeof(int?))
                || type.Equals(typeof(decimal?))
                || type.Equals(typeof(float?))
                || type.Equals(typeof(double?))
                )
                return true;

            return false;
        }

        public static bool IsNumeric(this Type type)
        {
            if (type.Equals(typeof(int))
                || type.Equals(typeof(decimal))
                || type.Equals(typeof(float))
                || type.Equals(typeof(double))
                )
                return true;

            return false;
        }

        public static bool HasFloatingPoint(this Type type)
        {
            if (type.Equals(typeof(decimal)) || type.Equals(typeof(decimal?))
                || type.Equals(typeof(float)) || type.Equals(typeof(float?))
                || type.Equals(typeof(double)) || type.Equals(typeof(double?))
                )
                return true;

            return false;
        }
    }
}