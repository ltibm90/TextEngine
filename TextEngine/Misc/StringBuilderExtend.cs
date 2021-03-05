using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class StringBuilderExtend
    {
        private StringBuilder sb = new StringBuilder();
        public StringBuilderExtend()
        {

        }
        public void Append(string s)
        {
            sb.Append(s);
        }
        public StringBuilderExtend(string v)
        {
            this.Append(v);
        }
        public string Text
        {
            get
            {
                return sb.ToString();
            }
        }
        public override string ToString()
        {
            return this.Text;
        }
        public int Length
        {
            get
            {
                return this.Text.Length;
            }
        }
        public string ToLower()
        {
            return this.Text.ToLower();
        }
        public string ToUpper()
        {
            return this.Text.ToUpper();
        }
        public static implicit operator string(StringBuilderExtend v)
        {
            if (v == null) return null;
            return v.Text;
        }
        public static implicit operator StringBuilderExtend(string v)
        {
            return new StringBuilderExtend(v);
        }
        public static StringBuilderExtend operator +(StringBuilderExtend item1, string item2)
        {
            if(item1 == null)
            {
                item1 = new StringBuilderExtend();
            }
            item1.Append(item2);
            return item1;
        }
        public static StringBuilderExtend operator +(StringBuilderExtend item1, StringBuilderExtend item2)
        {
            if (item1 == null)
            {
                item1 = new StringBuilderExtend();
            }
            if(item2 != null)
            {
                item1.Append(item2.Text);
            }
            return item1;
        }

    }
}
