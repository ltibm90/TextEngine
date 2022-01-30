using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementInfo
    {
        public string ElementName { get; set; }
        public TextElementFlags Flags { get; set; }
        public Dictionary<string, object> CustomData { get; set; }
        public Action<TextElement> OnTagOpened { get; set; }
        public Action<TextElement> OnTagClosed { get; set; }
        public Predicate<TextElement> OnAutoCreating { get; set; }
        public object SingleData { get; set; }
    }
}
