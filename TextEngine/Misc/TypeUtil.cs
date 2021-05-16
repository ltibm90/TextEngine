using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TextEngine.Misc
{
    public static class TypeUtil
    {
        public static bool IsDictionary(Type t)
        { 
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
        public static bool IsIDictionary(Type t)
        {
            if (t is IDictionary) return true;
            return t.GetInterfaces().Any(m => m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }
        public static bool IsList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
        public static bool IsNumericType(object item)
        {
            if (item == null) return false;
            Type type = null;
            if (item is Type tp)
            {
                type = tp;
            }
            else
            {
                type = item.GetType();
            }
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }
    }
}
