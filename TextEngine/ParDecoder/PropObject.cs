using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class PropObject
    {
        public object Value { get; set; }
        public object PropertyInfo { get; set; }
        public object[] IndisParams { get; set; }
        public PropType PropType { get; set; }
        public object Indis { get; set; }
        public object CustomData { get; set; }
    }
    public enum PropType
    {
        Empty = 0,
        Property,
        Dictionary,
        KeyValues,
        Indis
    }

}