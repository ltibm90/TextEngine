using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;
using TextEngine.Extensions;

namespace TextEngine.ParDecoder
{
    public class ParDecode
    {

        public EvulatorTypes StaticTypes { get; private set; } = new EvulatorTypes();
        public List<string> GlobalFunctions { get; set; } = new List<string>();

        public Func<PardecodeFlags> OnGetFlags { get; set; }
        public Predicate<PardecodeFlags> OnSetFlags { get; set; }
       

        public string Text { get; set; }
        private int TextLength { get { return this.Text.Length; } }
        private int pos;
        public ParItem items;
        private PardecodeFlags flags;
        public PardecodeFlags Flags
        {
            get
            {
                if (this.OnGetFlags != null) return this.OnGetFlags();
                return this.flags;
            }
            set
            {
                if (this.OnSetFlags != null && this.OnSetFlags(value)) return;
                this.flags = value;
            }
        }
        public ParItemAssignReturnType AssignReturnType { get; set; }


        public bool SurpressError { get; set; }
        public ParItem Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        public ParDecode(string text)
        {
            this.AssignReturnType = ParItemAssignReturnType.PIART_RETRUN_BOOL;
            this.Text = text;
            this.Items = new ParItem();
            this.Items.ParName = "(";
            this.Items.BaseDecoder = this;
            this.Flags = PardecodeFlags.PDF_AllowMethodCall | PardecodeFlags.PDF_AllowSubMemberAccess | PardecodeFlags.PDF_AllowArrayAccess;
        }

        public void Decode()
        {
            InnerItem parentItem = this.Items;
            for (int i = 0; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                if (cur == '(' || cur == '[' || cur == '{')
                {
                    var item = new ParItem
                    {
                        Parent = parentItem,
                        ParName = cur.ToString(),
                        BaseDecoder = this
                    };
                    parentItem.InnerItems.Add(item);
                    parentItem = item;
                    continue;
                }
                else if (cur == ')' || cur == ']' || cur == '}')
                {
                    parentItem = parentItem.Parent;
                    if (parentItem == null)
                    {
                        if(this.SurpressError)
                        {
                            parentItem = this.Items;
                            continue;
                        }
                        throw new Exception("Syntax Error");
                    }
                    continue;
                }
                var result = this.DecodeText(i);
                parentItem.InnerItems.AddRange(result);
                i = this.pos;
            }
        }
        private InnerItemsList DecodeText(int start, bool autopar = false)
        {
            var inspec = false;
            var inquot = false;
            char qutochar = '\0';
            var innerItems = new InnerItemsList();
            StringBuilder value = new StringBuilder();
            for (int i = start; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                char next = '\0';
                if (i + 1 < this.TextLength)
                {
                    next = this.Text[i + 1];
                }
                if (inspec)
                {
                    value.Append(cur);
                    inspec = false;
                    continue;
                }
                if (cur == '\\')
                {
                    inspec = true;
                    continue;
                }
                if (!inquot)
                {
                    if (cur == ' ' || cur == '\t')
                    {
                        continue;
                    }
                    if (cur == '\'' || cur == '\"')
                    {
                        inquot = true;
                        qutochar = cur;
                        continue;
                    }
                    if (cur == '+' || cur == '-' || cur == '*' ||
                    cur == '/' || cur == '%' || cur == '!' ||
                    cur == '=' || cur == '&' || cur == '|' ||
                    cur == ')' || cur == '(' || cur == ',' ||
                    cur == '[' || cur == ']' || cur == '^' ||
                    cur == '<' || cur == '>' || cur == '{' ||
                    cur == '}' || (cur == ':' && next != ':') || cur == '?' || cur == '.')
                    {
                        if (value.Length > 0)
                        {
                            innerItems.Add(this.Inner(value.ToString(), qutochar));
                            value.Clear();
                        }
                        if (cur == '[' || cur == '(' || cur == '{')
                        {
                            this.pos = i - 1;
                            return innerItems;
                        }
                        if(autopar && (cur == '?' || cur == ':' || cur == '=' || cur == '<' || cur == '>' || (cur == '!' && next == '=')))
                        {
                           
                            if ((cur == '=' && next == '>') || (cur == '!' && next == '=') || (cur == '>' && next == '=') || (cur == '<' && next == '='))
                            {
                                this.pos = i;
                            }
                            else
                            {
                                //this.pos = i - 1;
                                this.pos = i;
                            }
                            return innerItems;
                        }

                        if (cur != '(' && cur != ')' && cur != '[' && cur != ']' && cur != '{' && cur != '}')
                        {
                            var inner2 = new InnerItem
                            {
                                IsOperator = true
                            };
                            if ((cur == '=' && next == '>') || (cur == '!' && next == '=') || (cur == '>' && next == '=') || (cur == '<' && next == '=')
                                 || (cur == '+' && next == '=') || (cur == '-' && next == '=') || (cur == '*' && next == '=') || (cur == '/' && next == '=') || (cur == '^' && next == '='))
                            {
                                inner2.Value = cur.ToString() + next.ToString();
                                i++;
                            }
                            else if ((cur == '=' || cur == '&' || cur == '|') && cur == next)
                            {
                                inner2.Value = cur.ToString() + next.ToString();
                                i++;
                            }
                            else
                            {
                                inner2.Value = cur.ToString();
                            }
                            string valuestr =(string) inner2.Value;
                            innerItems.Add(inner2);
                            qutochar = '\0';
                            if (valuestr == "=" ||valuestr == "<=" || valuestr == ">=" || valuestr == "<" || valuestr == ">" || valuestr == "!=" || valuestr == "==")
                            {
                                //this.pos = i - 1;
                                this.pos = i;
                                return innerItems;
                            }

                        }
                        else
                        {
                            this.pos = i - 1;
                            return innerItems;
                        }
                        continue;
                    }
                }
                else
                {
                    if (cur == qutochar)
                    {
                        inquot = false;
                        continue;
                    }
                }

                if (cur == ':' && next == ':')
                {
                    value.Append(':');
                    i++;
                }
                value.Append(cur);

            }
            if (value.Length > 0)
            {
                innerItems.Add(this.Inner(value.ToString(), qutochar));
            }
            this.pos = this.TextLength;

            return innerItems;
        }
        private InnerItem Inner(string current, char quotchar)
        {
            var inner = new InnerItem
            {
                Value = current,
                Quote = quotchar,
                InnerType = InnerType.TYPE_STRING
            };

            if (inner.Quote != '\'' && inner.Quote != '"')
            {
                if (current == "true" || current == "false")
                {
                    inner.InnerType = InnerType.TYPE_BOOLEAN;
                    if(current == "true")
                    {
                        inner.Value = true;
                    }
                    else
                    {
                        inner.Value = false;
                    }
                }
                else if (inner.Quote == '\0' && current.IsNumeric())
                {
                    inner.InnerType = InnerType.TYPE_NUMERIC;
                    inner.Value = double.Parse(current.ToString());
                }
                else
                {
                    inner.InnerType = InnerType.TYPE_VARIABLE;
                }
            }
            return inner;
        }

        public ComputeResult Compute(object vars = null, object localvars = null, bool autodecode = true)
        {
            if (autodecode && !string.IsNullOrEmpty(this.Text) && this.Items.InnerItems.Count == 0) this.Decode();
            return this.Items.Compute(vars, null, localvars);
        }
    }
}
