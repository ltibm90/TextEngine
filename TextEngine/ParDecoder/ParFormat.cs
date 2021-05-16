using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ParFormat
    {
        public ParFormat()
        {
            this.Initialise();
        }
        public ParFormat(string text)
        {
            this.Text = text;
            this.Initialise();
        }
        private void Initialise()
        {

            this.ParAttributes = new ParDecodeAttributes();
            this.ParAttributes.AssignReturnType = ParItemAssignReturnType.PIART_RETURN_ASSIGNVALUE_OR_NULL;
        }
        private string text;
        public string Text {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.FormatItems = null;
            }
        }
        private List<ParFormatItem> FormatItems { get; set; }
        public ParDecodeAttributes ParAttributes { get; private set; }
        public string Apply(object data = null)
        {
            if (string.IsNullOrEmpty(this.Text)) return this.Text;
            if(this.FormatItems == null)
            {
                this.ParseFromString(this.Text);
            }
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < this.FormatItems.Count; i++)
            {
                var item = this.FormatItems[i];
                if(item.ItemType == ParFormatType.TextPar)
                {
                    text.Append(item.ItemText);
                    continue;
                }
                else if(item.ItemType == ParFormatType.FormatPar)
                {
                    if(item.ParData == null)
                    {
                        item.ParData = new ParDecode(item.ItemText);
                        item.ParData.OnGetAttributes = () => this.ParAttributes;
                        item.ParData.Decode();
                    }
                    var cr = item.ParData.Items.Compute(data);
                    text.Append(cr.Result?.First()?.ToString());
                }
            }
            return text.ToString();
        }
        public static string Format(string s, object data = null)
        {
            var pf = new ParFormat(s);
            return pf.Apply(data);
        }
        public static string FormatEx(string s, object data = null, Action<ParDecodeAttributes> onInitialise = null)
        {
            var pf = new ParFormat(s);
            if (onInitialise != null) onInitialise(pf.ParAttributes);
            return pf.Apply(data);
        }
        private void ParseFromString(string s)
        {
            int openedPar = 0;
            this.FormatItems = new List<ParFormatItem>();
            StringBuilder text = new StringBuilder();
            bool inpar = false;
            char quotchar = '0';
            for (int i = 0; i < s.Length; i++)
            {
                char cur = s[i];
                char next = '\0';

                if (i + 1 < s.Length) next = s[i + 1];
                if(!inpar)
                {
                    if(cur == '{' && next == '{')
                    {
                        i++;
                        text.Append(cur);
                        continue;
                    }
                    if (cur == '{' && next == '%')
                    {
                        i += 1;
                        if(text.Length > 0)
                        {
                            this.FormatItems.Add(new ParFormatItem() {
                             ItemText = text.ToString(),
                             ItemType = ParFormatType.TextPar}
                            );
                            text.Clear();
                        }
                        inpar = true;
                        continue;
                    }
                }
                else
                {
                    if(quotchar == '0' && (cur == '\'' || cur == '"'))
                    {
                        quotchar = cur;
                    }
                    else if (quotchar != '0' && cur == quotchar) quotchar = '0';
                    if(cur == '{' && quotchar == '0')
                    {
                        openedPar++;
                        //if (this.SurpressError)
                        //{
                        //    continue;
                        //}
                        //throw new Exception("Syntax Error: Unexpected {");
                    }
                    if(cur == '}')
                    {
                        if(openedPar > 0)
                        {
                            openedPar--;
                            text.Append(cur);
                            continue;
                        }
                        if (text.Length > 0)
                        {
                            this.FormatItems.Add(new ParFormatItem()
                            {
                                ItemText = text.ToString(),
                                ItemType = ParFormatType.FormatPar
                            }
                            );
                            text.Clear();
                        }
                        inpar = false;
                        continue;
                    }
                }
                text.Append(cur);
            }
            if(text.Length > 0)
            {
                this.FormatItems.Add(new ParFormatItem()
                {
                    ItemText = text.ToString(),
                    ItemType = (inpar) ? ParFormatType.FormatPar : ParFormatType.TextPar
                });
            }
            
        }
       
    }
}
