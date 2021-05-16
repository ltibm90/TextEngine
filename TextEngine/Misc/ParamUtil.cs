using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TextEngine.Misc
{
    public class ParamUtil
    {
        public static object MatchParam(object data, Type targetType, out bool result)
        {
            result = false;
            if (data == null) return null;
         
            Type callingType = data.GetType();
            if (targetType == typeof(object) || callingType == targetType)
            {
                result = true;
                return data;
            }
            else if (targetType == typeof(string))
            {
                result = true;
                return data.ToString();
            }
            else if (targetType == typeof(char))
            {
                char c = '\0';
                if (TypeUtil.IsNumericType(data))
                {
                    uint d = (uint)Convert.ChangeType(data, TypeCode.UInt32);
                    if (d < 65536)
                    {
                        c = (char)d;
                    }
                    else
                    {
                        c = d.ToString()[0];
                    }
                }
                else
                {
                    var str = data.ToString();
                    if (str.Length > 0)
                    {
                        c = str[0];
                    }
                }
                result = true;
                return c;
            }
         
            else
            {
                if(TypeUtil.IsDictionary(targetType))
                {
                    if (TypeUtil.IsDictionary(callingType) && (targetType.GenericTypeArguments.Intersect(callingType.GenericTypeArguments).Count() == targetType.GenericTypeArguments.Length))
                    {
                            result = true;
                            return data;
                    }
                    else if(typeof(Dictionary<string, object>).IsAssignableFrom(targetType) && typeof(IDictionary<string, object>).IsAssignableFrom(callingType))
                    {
                        result = true;
                        return new Dictionary<string, object>((IDictionary<string, object>)data);
                    }
                    else if(TypeUtil.IsIDictionary(callingType))
                    {
                        Type genericType = typeof(Dictionary<,>).MakeGenericType(targetType.GetGenericArguments());
                        IDictionary newDict =(IDictionary) Activator.CreateInstance(genericType);
                        int total = 0;
                        
                        foreach (var  item in (IEnumerable) data)
                        { 
                            bool bresult;
                            var name = MatchParam(item.GetType().GetProperty("Key").GetValue(item), targetType.GenericTypeArguments[0], out bresult);
                            if (!bresult) continue;
                            var value = MatchParam(item.GetType().GetProperty("Value").GetValue(item), targetType.GenericTypeArguments[1], out bresult);
                            if (!bresult) continue;
                            newDict.Add(name, value);
                            total++;
                        }
                        result = true;
                        return newDict;

         
                    }

                }
                else if((targetType.IsArray || TypeUtil.IsList(targetType)) && (callingType.IsArray || TypeUtil.IsList(callingType)))
                {
                    if(callingType.IsArray && targetType.IsArray && callingType.GetElementType() == targetType.GetElementType())
                    {
                        result = true;
                        return data;
                    }
                    else if((TypeUtil.IsList(callingType) && TypeUtil.IsList(targetType)) && (callingType.GenericTypeArguments[0] == targetType.GenericTypeArguments[0]))
                    {
                        result = true;
                        return data;
                    }
                    else
                    {
                        bool targetIsList = !targetType.IsArray && TypeUtil.IsList(targetType);
                        IList newArray = null;
                        int datacount = ((IList)data).Count;
                        newArray = (IList)((targetIsList) ? Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType.GenericTypeArguments[0])) : Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType.GetElementType())));
                        foreach (var item in (IEnumerable)data)
                        {
                            bool bresult;
                            object newValue = MatchParam(item, (targetIsList) ? targetType.GenericTypeArguments[0] : targetType.GetElementType(), out bresult);
                            if (!bresult) continue;
                            newArray.Add(newValue);
                        }
                        if(targetIsList)
                        {
                            result = true;
                            return newArray;
                        }
                        result = true;
                        return newArray.GetType().GetMethod("ToArray").Invoke(newArray, null);
                        
                       
                    }
     
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(callingType);
                    try
                    {
                        if (converter.CanConvertTo(targetType))
                        {

                            result = true;
                            return converter.ConvertTo(data, targetType);
                        }

                    }
                    catch
                    {
                    }
                }


            }

            return null;
        }
        public static object[] MatchParams(object[] @params, ParameterInfo[] method_Params)
        {
            List<object> convertedParams = new List<object>();
            for (int i = 0; i < method_Params.Length; i++)
            {
                var cparam = method_Params[i];
                var callingParam = @params.ElementAtOrDefault(i);
                convertedParams.Add(MatchParam(callingParam, cparam.ParameterType, out bool result));
            }
            return convertedParams.ToArray();
        }
    }
}
