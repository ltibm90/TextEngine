using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementInfo
    {
        public string ElementName { get; set; }
        public bool IsConditionalTag { get; set; }
        public bool IsNoAttributedTag { get; set; }
        public bool IsAutoClosedTag { get; set; }
        Dictionary<string, object> CustomData { get; set; }
    }
}
