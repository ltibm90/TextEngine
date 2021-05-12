using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ParFormat
    {
        private PardecodeFlags flags;
        public PardecodeFlags Flags
        {
            get { return this.flags; }
            set
            {
                this.flags = value;
            }
        }
        public ParFormat()
        {
            this.Flags = PardecodeFlags.PDF_AllowArrayAccess | PardecodeFlags.PDF_AllowMethodCall | PardecodeFlags.PDF_AllowSubMemberAccess;
        }
        public ParFormat(string text)
        {
            this.Text = text;
            this.Flags = PardecodeFlags.PDF_AllowArrayAccess | PardecodeFlags.PDF_AllowMethodCall | PardecodeFlags.PDF_AllowSubMemberAccess;
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
        public bool SurpressError { get; set; }
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
                        item.ParData.OnGetFlags = () => this.Flags;
                        item.ParData.Decode();
                        item.ParData.SurpressError = this.SurpressError;
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

        private void ParseFromString(string s)
        {
            this.FormatItems = new List<ParFormatItem>();
            StringBuilder text = new StringBuilder();
            bool inpar = false;
    
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
                    if(cur == '{')
                    {
                        if (this.SurpressError)
                        {
                            continue;
                        }
                        throw new Exception("Syntax Error: Unexpected {");
                    }
                    if(cur == '}')
                    {
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
