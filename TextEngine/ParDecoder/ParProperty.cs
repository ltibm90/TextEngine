using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ParProperty
    {
        public ParProperty()
        {

        }
        public ParProperty(string name, PropType type = PropType.Property, bool isassign = false)
        {
            this.Name = name;
            this.Type = type;
            this.IsAssign = isassign;
        }
        public string Name { get; set; }
        public PropType Type { get; set; }
        public bool IsAssign { get; set; }
    }
}
