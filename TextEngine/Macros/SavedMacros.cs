using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Macros
{
    public class SavedMacros : TextElements
    {
        public bool AllowMultipleNames { get; set; }
        private int GetMacroIndex(string name)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].GetAttribute("name") == name) return i;
            }
            return -1;
        }
        public TextElement GetMacro(string name)
        {
            var index = GetMacroIndex(name);
            if (index == -1) return null;
            return this[index];
        }
        public List<TextElement> GetMacros(string name)
        {
            return this.GetElements(m => m.GetAttribute("name") == name).ToList();
        }
        public void SetMacro(string name, TextElement tag)
        {
            if (this.Exists(tag))
            {
                return;
            }
            if(!AllowMultipleNames)
            {
                var index = GetMacroIndex(name);
                if (index == -1)
                {
                    this.Add(tag);
                }
                else
                {
                    this[index] = tag;
                }
            }
            else
            {
                this.Add(tag);
            }

        }

    }
}
