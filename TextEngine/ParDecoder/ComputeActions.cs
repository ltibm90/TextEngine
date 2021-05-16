using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using TextEngine.Extensions;
using TextEngine.Misc;

namespace TextEngine.ParDecoder
{

    public class ComputeActions
    {
        public static List<string> PriotiryStop = new List<string>()
        {
            "and",
            "&&",
            "||",
            "==",
            "=",
            ">",
            "<",
            ">=",
            "!=",
            "<=",
            "or",
            "+",
            "-",
            ",",
            "=>",
            "?",
            ":"
        };
        public static object OperatorResult(object item1, object item2, string @operator)
        {
            if (item1 == null && item2 == null)
            {
                return null;
            }
            if ((@operator == "+" || @operator == "-") && item1 == null && item2 != null && TypeUtil.IsNumericType(item2))
            {
                var leftitem = 0d;
                var rightitem = (double)Convert.ChangeType(item2, typeof(double));
                if (@operator == "+")
                {
                    return leftitem + rightitem;
                }
                else
                {
                    return leftitem - rightitem;
                }
            }
            if ((@operator == "||" || @operator == "or" || @operator == "&&"|| @operator == "and") | ((!TypeUtil.IsNumericType(item1) || !TypeUtil.IsNumericType(item2)) && (@operator == "|" || @operator == "&")))
            {
                bool lefstate = PhpFuctions.not_empty(item1);
                bool rightstate = PhpFuctions.not_empty(item2);
                if (@operator == "||" || @operator == "|" || @operator == "or")
                {
                    if (lefstate != rightstate)
                    {
                        return true;
                    }
                    return lefstate;
                }
                else
                {
                    if (lefstate && rightstate)
                    {
                        return true;
                    }
                    return false;
                }
            }
            if ((item1 is string && item2 == null) || item2 is string && item1 == null)
            {
                if (item1 == null) return item2;
                return item1;
            }

            if (@operator == "==" || @operator == "=" || @operator == "!=" && (item1 == null || item2 == null))
            {
                if (item1 == null)
                {
                    if (@operator == "==" || @operator == "=")
                    {
                        return item2 == null;
                    }
                    else

                    {
                        return item2 != null;
                    }
                }
                else if (item2 == null)
                {
                    if (@operator == "==" || @operator == "=")
                    {
                        return item1 == null;
                    }
                    else

                    {
                        return item1 != null;
                    }
                }
            }
            if (item1 is string && @operator == "+")
            {
                return item1.ToString() + item2;
            }
            else if (item2 is string && @operator == "+")
            {
                return item1 + item2.ToString();
            }
            if (item1 is bool && item2 is bool)
            {
                var leftitem = (bool)item1;
                var rightitem = (bool)item2;
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return leftitem == rightitem;
                    case "!=":
                        return leftitem != rightitem;
                }
            }
            else if ((item1 is DateTime && TypeUtil.IsNumericType(item2)) || (item2 is DateTime && TypeUtil.IsNumericType(item1)))
            {

                if (item1 is DateTime)
                {
                    var leftitem = (DateTime)item1;
                    var rightitem = (double)item2;
                    switch (@operator)
                    {
                        case "+":

                            return leftitem.AddDays(rightitem);
                        case "-":
                            return leftitem.AddDays(-1d * rightitem);
                    }
                }
                else
                {
                    var leftitem = (double)item1;
                    var rightitem = (DateTime)item2;
                    switch (@operator)
                    {
                        case "+":

                            return rightitem.AddDays(leftitem);
                        case "-":
                            return rightitem.AddDays(-1d * leftitem);
                    }
                }
            }
            else if (item1 is DateTime && item2 is DateTime)
            {
                var leftitem = (DateTime)item1;
                var rightitem = (DateTime)item2;
                int cmpres = leftitem.CompareTo(rightitem);
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return cmpres == 0;
                    case "!=":
                        return cmpres != 0;
                    case "<":

                        return cmpres == -1;
                    case "<=":
                        return cmpres == 0 || cmpres == -1;
                    case ">":
                        return cmpres == 1;
                    case ">=":
                        return cmpres == 0 || cmpres == 1;
                    default:
                        break;
                }
            }
            else if (item1 is string && item2 is string)
            {
                var leftitem = (string)item1;
                var rightitem = (string)item2;
                int cmpres = leftitem.CompareTo(rightitem);
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return cmpres == 0;
                    case "!=":
                        return cmpres != 0;
                    case "<":

                        return cmpres == -1;
                    case "<=":
                        return cmpres == 0 || cmpres == -1;
                    case ">":
                        return cmpres == 1;
                    case ">=":
                        return cmpres == 0 || cmpres == 1;
                    default:
                        break;
                }
            }

            else
            {
                if (item2 == null) item2 = "";
                if (item1 == null) item1 = "";
                if (TypeUtil.IsNumericType(item1) && TypeUtil.IsNumericType(item2) || TypeUtil.IsNumericType(item1) && item2.ToString().IsNumeric() || TypeUtil.IsNumericType(item2) && item1.ToString().IsNumeric())
                {
                    var leftitem = (double)Convert.ChangeType(item1, typeof(double));
                    var rightitem = (double)Convert.ChangeType(item2, typeof(double));
                    switch (@operator)
                    {
                        case "==":
                        case "=":
                            return leftitem == rightitem;
                        case "!=":
                            return leftitem != rightitem;
                        case "<":
                            return leftitem < rightitem;
                        case "<=":
                            return leftitem <= rightitem;
                        case ">":
                            return leftitem > rightitem;
                        case ">=":
                            return leftitem >= rightitem;
                        case "+":
                            return leftitem + rightitem;
                        case "-":
                            return leftitem - rightitem;
                        case "*":
                            return leftitem * rightitem;
                        case "/":
                        case "div":
                            return leftitem / rightitem;
                        case "%":
                        case "mod":
                            return leftitem % rightitem;
                        case "^":
                            return Math.Pow(leftitem, rightitem);
                        case "&":
                            return (long)leftitem & (long)rightitem;
                        case "|":
                            return (long)leftitem | (long)rightitem;
                        default:
                            break;
                    }
                }
            }
            if (item1 == null) return item2;
            return item1;
        }

        public static object CallMethodSingle(object @object, string name, object[] @params, out bool iscalled)
        {
            iscalled = false;
            if (@object is MultiObject varsList)
            {
                for (int i = 0; i < varsList.Count; i++)
                {
                    if (varsList.Get(i) == null) continue;
                    var res = CallMethodSingle(varsList.Get(i), name, @params, out iscalled);
                    if (iscalled) return res;
                }
                return null;
            }

            MethodInfo method = null;
            if(@object is  IDictionary<string, object> dict)
            {
                if (!dict.TryGetValue(name, out object obj)) return null;
                if(obj is Delegate d)
                {
                    return CallMethodDirect(d.Target, d.Method, @params, out iscalled, true);
                }
                return null;
            }
            else
            {
                Type obj_type = @object.GetType();
                method = obj_type.GetMethodByNameWithParams(name, @params);
                if(method == null)
                {
                    var prop = obj_type.GetProperty(name);
                    if(prop != null && prop.CanRead)
                    {
                        var value = prop.GetValue(@object, null);
                        if(value is Delegate d)
                        {

                            return CallMethodDirect(d.Target, d.Method, @params, out iscalled, true);
                        }
                    }
                }
            }

            //var method = obj_type.GetMethod(name);

            return CallMethodDirect(@object, method, @params, out iscalled);

        }
        public static object CallMethodDirect(object @object, MethodInfo method, object[] @params, out bool iscalled, bool isdelegate = false)
        {
            iscalled = false;
            if (method == null) return null;
            var convertedParams = ParamUtil.MatchParams(@params, method.GetParameters());
            if (method != null)
            {
                if (method.IsPublic || isdelegate)
                {
                    iscalled = true;
                    return method.Invoke(@object, convertedParams.ToArray());
                }
            }
            return null;
        }
        /**  @param $item InnerItem */
        public static object CallMethod(string name, object[] @params, object vars, out bool iscalled, object localvars = null, ParDecode sender = null)
        {
            int dpos = name.IndexOf("::");
            iscalled = false;
            if (sender != null && dpos >= 0)
            {
                var clsname = name.Substring(0, dpos);
                var method = name.Substring(dpos + 2);
                if (sender.GlobalFunctions.Contains(clsname + "::") || sender.GlobalFunctions.Contains(name))
                {
                    var clsttype = sender.StaticTypes[clsname];
                    if (clsttype != null)
                    {
                        var cm = clsttype.GetType().GetMethod(method);
                        if (cm != null)
                        {
                            iscalled = true;
                            return cm.Invoke(null, @params);
                        }
                    }
                }
            }

            return CallMethodSingle(vars, name, @params, out iscalled);
        }

        public static PropObject GetPropValue(InnerItem item, object vars, object localvars = null)
        {
            PropObject res = null;
            var name = item.Value.ToString();
            if (localvars != null)
            {
                res = GetProp(name, localvars);
            }
            if (res == null || res.PropType == PropType.Empty)
            {
                res = GetProp(name, vars);
            }
            return res;
        }
        public static PropObject GetPropInArray(string item, MultiObject varsList)
        {
            for (int i = 0; i < varsList.Count; i++)
            {
                if (varsList.Get(i) == null) continue;
                var res = GetProp(item, varsList.Get(i));
                if (res != null && res.PropType != PropType.Empty) return res;
            }
            return null;
        }
        public static PropObject GetProp(string item, object vars)
        {
            var propObj = new PropObject();
            if (vars is MultiObject varsList)
            {
                return GetPropInArray(item, varsList);
            }

            if (vars == null) return propObj;
            Type varsType = vars.GetType();
            if (vars is KeyValueGroup il)
            {
                for (int i = il.Count - 1; i >= 0; i--)
                {
                    if (il[i] is KeyValues<object> kv)
                    {
                        var m = GetProp(item, kv);
                        if (m.PropType != PropType.Empty) return m;
                    }
                }
            }
            else if(TypeUtil.IsIDictionary(varsType))
            {
                var intfaceType = varsType.GetInterfaces().Where(m => m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IDictionary<,>)).FirstOrDefault();
                if ((!intfaceType.GenericTypeArguments[0].IsValueType && intfaceType.GenericTypeArguments[0] != typeof(string))) return propObj;
                object matchedParam = ParamUtil.MatchParam(item, intfaceType.GenericTypeArguments[0], out bool result);
                if (!result) return propObj;
                if ((bool) intfaceType.GetMethod("ContainsKey").Invoke(vars, new object[] { matchedParam }))
                {
                    var value = intfaceType.GetProperty("Item").GetValue(vars, new object[] { matchedParam });
                    propObj.Value = value;
                    propObj.PropType = PropType.Dictionary;
                    propObj.Indis = matchedParam;
                    propObj.PropertyInfo = vars;
                }
   
               

            }

            //else if (vars is IDictionary<string, object> dict)
            //{
            //    TypeUtil.IsIDictionary
            //    if (dict.TryGetValue(item, out object nobj))
            //    {
            //        propObj.Value = nobj;
            //        propObj.PropType = PropType.Dictionary;
            //        propObj.Indis = item;
            //        propObj.PropertyInfo = dict;
            //    }
            //}
            else if (vars is KeyValues<object> obj)
            {
                int id = obj.GetIdByName(item);
                if(id >= 0)
                {
                    propObj.Value = obj[id];
                    propObj.PropType = PropType.KeyValues;
                    propObj.Indis = item;
                    propObj.PropertyInfo = obj;
                }
            

            }
            else
            {
                var vtype = vars.GetType();
                var prop = vtype.GetProperty(item);
                if (prop != null)
                {
                    propObj.PropertyInfo = prop;
                    propObj.Value = prop.GetValue(vars);
                    propObj.PropType = PropType.Property;
                    propObj.Indis = vars;
                    return propObj;
                }
            }

            return propObj;
        }
        public static bool IsObjectOrArray(object item)
        {
            return item != null && item is ExpandoObject || item is Dictionary<string, object>;
        }
        public static AssignResult AssignObjectValue(PropObject probObj, string op, object value)
        {
            var ar = new AssignResult();
            if (probObj == null || probObj.Indis == null) return ar;
            if(op.Length == 2)
            {
               value = ComputeActions.OperatorResult(probObj.Value, value ,op[0].ToString());
            }
            bool matchResult = false;
            if (probObj.PropType == PropType.Property)
            {
                PropertyInfo pi = (PropertyInfo)probObj.PropertyInfo;
                if (pi.CanWrite)
                {
                    var targObj = ParamUtil.MatchParam(value, pi.PropertyType, out matchResult);
                    if (!matchResult) return ar;
                    if(probObj.IndisParams != null) pi.SetValue(probObj.Indis, targObj, probObj.IndisParams);
                   else pi.SetValue(probObj.Indis, targObj);
                    ar.AssignedValue = targObj;
                    ar.Success = true;
                    return ar;
                }
            }
            else if(probObj.PropType == PropType.Indis)
            {
                if (probObj.IndisParams == null || probObj.IndisParams.Length == 0) return ar;
                var item = probObj.Indis as IList;
                if (item == null) return ar;
                Type listtype = item.GetType();
                if(listtype.GenericTypeArguments.Length > 0)
                {
                    int indis = Convert.ToInt32(probObj.IndisParams[0]);
                    var targObj = ParamUtil.MatchParam(value, listtype.GenericTypeArguments[0], out matchResult);
                    if (!matchResult) return ar;
                    item[indis] = targObj;
                    ar.AssignedValue = targObj;
                    ar.Success = true;
                    return ar;
                }
            }
            else if(probObj.PropType == PropType.Dictionary)
            {
                if (probObj.Indis == null) return ar;
                var intfaceType = probObj.PropertyInfo.GetType().GetInterfaces().Where(m => m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IDictionary<,>)).FirstOrDefault();
                var targObj = ParamUtil.MatchParam(value, intfaceType.GenericTypeArguments[1], out matchResult);
                if (!matchResult) return ar;
                intfaceType.GetProperty("Item").SetValue(probObj.PropertyInfo, targObj , new object[] {probObj.Indis});
                ar.AssignedValue = targObj;
                ar.Success = true;
                return ar;
            }

            return ar;
        }

    }
}
