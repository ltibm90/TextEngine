﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using TextEngine.Evulator;
using TextEngine.Extensions;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.ParDecoder
{
    public class ParItem : InnerItem
    {
        public ParItem()
        {
            this.InnerItems = new InnerItemsList();

        }
        public ParDecode BaseDecoder { get; set; }


        public string ParName { get; set; }
        public override bool IsParItem()
        {
            return true;
        }
        public override bool IsObject()
        {
            return this.ParName == "{";
        }
        public bool IsArray(object exobject)
        {
            return !PhpFuctions.is_array(exobject) && this.IsArray();
        }
        public override bool IsArray()
        {
            return this.ParName == "[";
            //return this.ParName == "[" && ((this.Segments.Count == 0));
        }

        public InnerItem GetParentUntil(string name)
        {
            var parent = this.Parent;
            while (parent != null)
            {
                if (!(parent is ParItem paritem))
                {
                    parent = parent.Parent;
                    continue;
                }
                if (paritem.ParName == name) break;
                parent = parent.Parent;
            }
            return parent;
        }

        public ComputeResult Compute(object vars = null, InnerItem sender = null, object localvars = null)
        {
            var cr = new ComputeResult();
            object lastvalue = null;
            InnerItem xoperator = null;
            InnerItem previtem = null;
            InnerItem waititem = null;
            InnerItem waititem2 = null;
            string waitop = "";
            object waitvalue = null;
            string waitop2 = "";
            object waitvalue2 = null;
            string waitkey = "";
            bool unlemused = false;
            bool stopdoubledot = false;
            int minuscount = 0;
            string assigment = "";
            PropObject lastPropObject = null;
            PropObject waitAssigmentObject = null;
            int totalOp = 0;
            if(this.IsObject())
            {
                cr.Result.AddObject(new ExpandoObject());
            }
            for (int i = 0; i < this.InnerItems.Count; i++)
            {
                object currentitemvalue = null;
                InnerItem current = this.InnerItems[i];
                ParItem paritem = null;
                if(current.IsParItem())
                {
                    paritem = current as ParItem;
                }
                if(stopdoubledot)
                {
                    if(current.IsOperator && current.Value.ToString() == ":")
                    {
                        break;
                    }
                }
                InnerItem next = null;
                string nextop = "";
                if (i + 1 < this.InnerItems.Count) next = this.InnerItems[i + 1];
                if (next != null && next.IsOperator)
                {
                    nextop = next.Value.ToString();
                }
                if (current.IsParItem())
                {
                    var subresult = paritem.Compute(vars, this, localvars);
                    string prevvalue = "";
                    bool previsvar = false;
                    if(previtem != null && !previtem.IsOperator && previtem.Value != null)
                    {
                        previsvar = previtem.InnerType == InnerType.TYPE_VARIABLE;
                        prevvalue = previtem.Value.ToString();
                        
                    }
                    object varnew = null;
                    bool checkglobal = true;
                    if (lastvalue != null)
                    {
                        checkglobal = false;
                        varnew = lastvalue;
                    }
                    else
                    {
                        varnew = vars;
                    }
                    if (prevvalue != "")
                    {

                        if (paritem.ParName == "(")
                        {
                            if(this.BaseDecoder.Attributes.Flags.HasFlag(PardecodeFlags.PDF_AllowMethodCall))
                            {
                                bool iscalled = false;
                                if (paritem.BaseDecoder != null && paritem.BaseDecoder.SurpressError)
                                {
                                    try
                                    {
                                        currentitemvalue = ComputeActions.CallMethod(prevvalue, subresult.Result.GetObjects(), varnew, out iscalled, localvars, this.BaseDecoder, checkglobal);
                                    }
                                    catch
                                    {

                                        currentitemvalue = null;
                                    }
                                }
                                else
                                {
                                    currentitemvalue = ComputeActions.CallMethod(prevvalue, subresult.Result.GetObjects(), varnew, out iscalled, localvars, this.BaseDecoder, checkglobal);
                                }
                            }
                            else
                            {
                                //currentitemvalue = null;
                            }
                        }
                        else if(paritem.ParName == "[")
                        {
                            if(this.BaseDecoder.Attributes.Flags.HasFlag(PardecodeFlags.PDF_AllowArrayAccess))
                            {
                                lastPropObject = ComputeActions.GetProp(prevvalue, varnew); ;
                                var prop = lastPropObject.Value;
                                if (prop != null)
                                {
                                    if (PhpFuctions.is_array(prop))
                                    {
                                        int indis = (int)Convert.ChangeType(subresult.Result[0], TypeCode.Int32);
                                        var aritem = prop as IList;

                                        currentitemvalue = aritem[indis];
                                        lastPropObject.IndisParams = new object[] { indis};
                                        lastPropObject.PropType = PropType.Indis;
                                        lastPropObject.Indis = aritem;
                                    }
                                    else if (PhpFuctions.is_indis(prop))
                                    {
                                        var indisProp = prop.GetType().GetProperty("Item");
                                        var newParams = ParamUtil.MatchParams(subresult.Result.GetObjects(), indisProp.GetIndexParameters());
                                        currentitemvalue = indisProp.GetValue(prop, newParams);
                                        lastPropObject.IndisParams = newParams;
                                        lastPropObject.PropType = PropType.Property;
                                        lastPropObject.PropertyInfo = indisProp;
                                        lastPropObject.Indis = prop;
                                    }
                                    else
                                    {
                                        int indis = (int)Convert.ChangeType(subresult.Result[0], TypeCode.Int32);
                                        currentitemvalue = ((string)prop)[indis];
                                        lastPropObject = null;
                                    }
                                }
                            }
                            else
                            {
                               
                                //currentitemvalue = null;
                            }

                        }
                    }
                    else
                    {
                        if(paritem.ParName == "(")
                        {
                            currentitemvalue = subresult.Result[0];
                        }
                        else if(paritem.ParName == "[")
                        {
                            if(subresult.Result.KeysIncluded())
                            {
                                currentitemvalue = subresult.Result.ToDictionary();
                            }
                            else
                            {
                                currentitemvalue = subresult.Result.GetObjects();
                            }
                          
                        }
                        else if(paritem.ParName == "{")
                        {
                            currentitemvalue = subresult.Result.First();
                        }
                    }

                }
                else
                {
                    if(!current.IsOperator && current.InnerType == InnerType.TYPE_VARIABLE &&  next != null && next.IsParItem())
                    {
                        currentitemvalue = null;
                    }
                    else
                    {
                        if(previtem != null && previtem.IsOperator)
                        {
                            if(current.Value.ToString() == "+")
                            {
                                continue;
                            }
                            else if (current.Value.ToString() == "-")
                            {
                                minuscount++;
                                continue;
                            }
                         
                        
                        }
                        currentitemvalue = current.Value;
                    }
                    if (current.InnerType == InnerType.TYPE_VARIABLE && (next == null || !next.IsParItem()) && (xoperator == null || xoperator.Value.ToString() != ".") )
                    {
                        if (currentitemvalue == null || currentitemvalue.ToString() == "null")
                        {
                            currentitemvalue = null;
                        }
                        else if (currentitemvalue.ToString() == "false")
                        {
                            currentitemvalue = false;
                        }
                        else if (currentitemvalue.ToString() == "true")
                        {
                            currentitemvalue = true;
                        }
                        else if (!this.IsObject())
                        {

                            lastPropObject = ComputeActions.GetPropValue(current, vars, localvars);
                            currentitemvalue = lastPropObject.Value;
                        }
                    }
                }
                if(unlemused)
                {
                    currentitemvalue = !PhpFuctions.not_empty(currentitemvalue);
                    unlemused = false;
                }
                if (current.IsOperator)
                {
                    totalOp++;
                    if (current.Value.ToString() == "!")
                    {
                        unlemused = !unlemused;
                        previtem = current;
                        continue;
                    }
                    if ((this.IsParItem() && current.Value.ToString() == ",") || (this.IsArray() && current.Value.ToString() == "=>" && (waitvalue == null || waitvalue.ToString() == "")) || (this.IsObject() && current.Value.ToString() == ":" && (waitvalue == null || waitvalue.ToString() == "") ))
                    {
                       
                        if (waitop2 != "")
                        {
                            if(minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                            lastvalue = ComputeActions.OperatorResult(waitvalue2, lastvalue, waitop2);
                            waitvalue2 = null;
                            waitop2 = "";
                            minuscount = 0;
                        }
                        if (waitop != "")
                        {
                            if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                            lastvalue = ComputeActions.OperatorResult(waitvalue, lastvalue, waitop);
                            waitvalue = null;
                            waitop = "";
                            minuscount = 0;
                        }
                        if (current.Value.ToString() == ",")
                        {
                            if(this.IsObject())
                            {
                               var exp = cr.Result.First<IDictionary<string, object>>();
                                exp.Add(waitkey, lastvalue);
                            }
                            else
                            {
                                cr.Result.AddObject(waitkey, lastvalue);
                            }

                            waitkey = "";
                           

                        }
                        else
                        {
                            waitkey = lastvalue.ToString();
                        }
                        lastvalue = null;
                        xoperator = null;
                        previtem = current;
                        continue;
                    }
                    string opstr = current.Value.ToString();
                    if(waitAssigmentObject == null && (opstr == "=" || opstr == "+=" || opstr == "-=" || opstr == "*=" || opstr == "/=" || opstr == "^=" || opstr == "|=" 
                        || opstr == "&=" || opstr == "<<=" || opstr == ">>="))
                    {
                        if (totalOp <= 1 && (this.BaseDecoder.Attributes.Flags & PardecodeFlags.PDF_AllowAssigment) != 0)
                        {
                            waitAssigmentObject = lastPropObject;
                            assigment = opstr;

                            xoperator = null;
                            previtem = null;

                        }
                        else
                        {
                            xoperator = null;
                            previtem = null;
                        }
                        continue;
                    }


                    if (opstr == "||" || /*opstr == "|" ||*/ opstr == "or" || opstr == "&&" || /*opstr == "&" ||*/ opstr == "and" || opstr == "?")
                    {
                        if (waitop2 != "")
                        {
                            if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                            lastvalue = ComputeActions.OperatorResult(waitvalue2, lastvalue, waitop2);
                            waitvalue2 = null;
                            waitop2 = "";
                            minuscount = 0;

                        }
                        if (waitop != "")
                        {
                            if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                            lastvalue = ComputeActions.OperatorResult(waitvalue, lastvalue, waitop);
                            waitvalue = null;
                            waitop = "";
                            minuscount = 0;
                        }

                        bool state = PhpFuctions.not_empty(lastvalue);
                        xoperator = null;
                        if (opstr == "?")
                        {
                            if (state)
                            {
                                stopdoubledot = true;
                            }
                            else
                            {
                                for (int j = i + 1; j < this.InnerItems.Count; j++)
                                {
                                    var item = this.InnerItems[j];
                                    if (item.IsOperator && item.Value.ToString() == ":")
                                    {
                                        i = j;
                                        break;
                                    }
                                }
                            }
                            lastvalue = null;
                            previtem = current;
                            continue;


                        }



                        if (opstr == "||" || /*opstr == "|" ||*/ opstr == "or")
                        {
                            if (state)
                            {
                                lastvalue = true;
                                /*if (opstr != "|")
                                {*/
                                    if (this.IsObject())
                                    {
                                        var exp = cr.Result.First<IDictionary<string, object>>();
                                        exp.Add(waitkey, true);
                                    }
                                    else
                                    {
                                        cr.Result.AddObject(waitkey, true);
                                    }
                                    return cr;
                                //}
                            }
                            else
                            {
                                lastvalue = false;
                            }
                        }
                        else
                        {
                            if (!state)
                            {
                                lastvalue = false;
                                /*if (opstr != "&")
                                {*/
                                    if (this.IsObject())
                                    {
                                        var exp = cr.Result.First<IDictionary<string, object>>();
                                        exp.Add(waitkey, false);
                                    }
                                    else
                                    {
                                        cr.Result.AddObject(waitkey, false);
                                    }
                                    return cr;
                                //}
                            }
                            else
                            {
                                lastvalue = true;
                            }
                        }
                        xoperator = null;
                    }
                    else
                    {
                        xoperator = current;
                    }

                    previtem = current;
                    continue;
                }
                else
                {

                    if (xoperator != null)
                    {
                        if ( ComputeActions.PriotiryStop.Contains(xoperator.Value.ToString()))
                        {
                            if(waitop2 != "")
                            {
                                if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                                lastvalue = ComputeActions.OperatorResult(waitvalue2, lastvalue, waitop2);
                                waitvalue2 = null;
                                waitop2 = "";
                                minuscount = 0;
                            }
                            if(waitop != "")
                            {
                                if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                                lastvalue = ComputeActions.OperatorResult(waitvalue, lastvalue, waitop);
                                waitvalue = null;
                                waitop = "";
                                minuscount = 0;
                            }
                        }

                        if (next != null && next.IsParItem())
                        {
                            if (xoperator.Value.ToString() == ".")
                            {
                                if(currentitemvalue != null && !string.IsNullOrEmpty(currentitemvalue.ToString()))
                                {
                                    lastPropObject = ComputeActions.GetProp(currentitemvalue.ToString(), lastvalue);
                                    lastvalue = lastPropObject.Value;
                                }
                            }
                            else
                            {
                                if(waitop == "")
                                {
                                    waitop = xoperator.Value.ToString();
                                    waititem = current;
                                    waitvalue = lastvalue;
                                }
                                else if(waitop2 == "")
                                {
                                    waitop2 = xoperator.Value.ToString();
                                    waititem2 = current;
                                    waitvalue2 = lastvalue;
                                }
                                lastvalue = null;
                            }
                            xoperator = null;
                            previtem = current;
                            continue;
                        }
                        if (xoperator.Value.ToString() == ".")
                        {
                            totalOp--;
                            if(this.BaseDecoder.Attributes.Flags.HasFlag( PardecodeFlags.PDF_AllowSubMemberAccess))
                            {
                                lastPropObject = ComputeActions.GetProp(currentitemvalue.ToString(), lastvalue);
                                lastvalue = lastPropObject.Value;
                            }
                            else
                            {
                                //lastvalue = null;
                            }
                        }
                        else if (nextop != "." && ((xoperator.Value.ToString() != "+" && xoperator.Value.ToString() != "-") || nextop == "" || (ComputeActions.PriotiryStop.Contains(nextop))))
                        {
                            if (minuscount % 2 == 1) currentitemvalue = ComputeActions.OperatorResult(currentitemvalue, -1, "*");
                            var opresult = ComputeActions.OperatorResult(lastvalue, currentitemvalue, xoperator.Value.ToString());
                            lastvalue = opresult;
                            minuscount = 0;
                        }
                        else
                        {
                            if(waitop == "")
                            {
                                waitop = xoperator.Value.ToString();
                                waititem = current;
                                waitvalue = lastvalue;
                                lastvalue = currentitemvalue;
                            }
                            else if(waitop2 == "")
                            {
                                waitop2 = xoperator.Value.ToString();
                                waititem2 = current;
                                waitvalue2 = lastvalue;
                                lastvalue = currentitemvalue;
                            }
                        
                            continue;
                        }
                    }
                    else
                    {
                        lastvalue = currentitemvalue;
                    }


                }

                previtem = current;
            }

            if (waitop2 != "")
            {
                if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                lastvalue = ComputeActions.OperatorResult(waitvalue2, lastvalue, waitop2);
                waitvalue2 = null;
                waitop2 = "";
                minuscount = 0;
            }
            if (waitop != "")
            {
                if (minuscount % 2 == 1) lastvalue = ComputeActions.OperatorResult(lastvalue, -1, "*");
                lastvalue = ComputeActions.OperatorResult(waitvalue, lastvalue, waitop);
                waitvalue = null;
                waitop = "";
                minuscount = 0;
            }
            if (waitAssigmentObject != null )
            {
                AssignResult assignResult = null;
                if(waitAssigmentObject.PropType != PropType.Empty && waitAssigmentObject.PropertyInfo != null)
                {
                    try
                    {
                        assignResult = ComputeActions.AssignObjectValue(waitAssigmentObject, assigment, lastvalue);
                    }
                    catch
                    {

                    }
                }
                switch (this.BaseDecoder.Attributes.AssignReturnType)
                {
                    case ParItemAssignReturnType.PIART_RETURN_NULL:
                        lastvalue = null;
                        break;
                    case ParItemAssignReturnType.PIART_RETRUN_BOOL:
                        lastvalue = (bool)assignResult;
                        break;
                    case ParItemAssignReturnType.PIART_RETURN_ASSIGNVALUE_OR_NULL:
                        if (!assignResult) lastvalue = null;
                        else lastvalue = assignResult.AssignedValue;
                        break;
                    case ParItemAssignReturnType.PIART_RETURN_ASSIGN_VALUE:
                        if (assignResult) lastvalue = assignResult.AssignedValue;
                        break;
                }
            }
            if (this.IsObject())
            {
                var exp = cr.Result.First<IDictionary<string, object>>();
                exp.Add(waitkey, lastvalue);
            }
            else
            {
                cr.Result.AddObject(waitkey, lastvalue);
            }
            return cr;
        }
    }
}
