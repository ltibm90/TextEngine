using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementAttribute
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        private string value;
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.ParData = null;
            }
        }
        public ParDecoder.ParDecode ParData { get; set; }
        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }
    }
}
