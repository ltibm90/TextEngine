using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ParTracerItem
    {
        public ParTracerItem()
        {

        }
        public ParTracerItem(string name, PropType type, object value = null)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public PropType Type { get; set; }
        public bool Accessed { get; set; }
        public bool IsAssign { get; set; }
    }
}
