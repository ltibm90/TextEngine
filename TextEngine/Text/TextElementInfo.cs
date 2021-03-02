using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementInfo
    {
        public string ElementName { get; set; }
        public TextElementFlags Flags { get; set; }
        Dictionary<string, object> CustomData { get; set; }
    }
}
