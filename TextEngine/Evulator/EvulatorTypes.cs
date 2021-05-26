using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;

namespace TextEngine.Evulator
{
    public class EvulatorTypes
    {
        private Dictionary<string, Type> types;
        public Type this[string name]
        {
            get
            {
                if (name == null) return null;
                return this.GetType(name);
            }
            set
            {
                if (value == null) return;
                types[name] = value;
            }
        }
        public EvulatorTypes()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            types = new Dictionary<string, Type>(comparer);
            this.Options = new EvulatorOptions();
        }
        
        public Type Param { get; set; }
        public Type GeneralType { get; set; }
        public Type Text { get; set; }
        public EvulatorOptions Options { get; private set; }
        public void SetType(string name, Type type)
        {
            types[name] = type;
        }
        public Type GetType(string name)
        {
            if(types.TryGetValue(name, out Type type))
            {
                return type;
            }
            return null;
        }
        public void Clear()
        {
            types.Clear();
        }
    }
}
