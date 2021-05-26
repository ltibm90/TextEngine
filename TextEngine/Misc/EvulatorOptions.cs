using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class EvulatorOptions
    {
        public EvulatorOptions()
        {
            this.OtherOptions = new Dictionary<string, object>();
        }
        public Dictionary<string, object> OtherOptions {get; private set;}
        public int Max_For_Loop { get; set; }
        public int Max_DoWhile_Loop { get; set; }
        public int Max_Repeat_Loop { get; set; }
        public int Max_ForEach_Loop { get; set; }
        public object GetOption(string name, object defaultV = null)
        {
            object val = null;
            if (OtherOptions.TryGetValue(name, out val)) return val;
            return defaultV;
        }
        public void SetOptions(string name, object value)
        {
            this.OtherOptions[name] = value;
        }

    }
}