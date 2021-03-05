using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ParFormatItem
    {
        public ParFormatType ItemType { get; set; }
        private string itemText;
        public string ItemText
        {
            get
            {
                return this.itemText;
            }
            set
            {
                this.itemText = value;
                this.ParData = null;
            }
        }
        public ParDecode ParData { get; set; }
    }
}
