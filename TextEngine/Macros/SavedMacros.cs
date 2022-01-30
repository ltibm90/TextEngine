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
        private List<TextElement> macros = new List<TextElement>();
        private int GetMacroIndex(string name)
        {
            for (int i = 0; i < macros.Count; i++)
            {
                if (macros[i].GetAttribute("name") == name) return i;
            }
            return -1;
        }
        public TextElement GetMacro(string name)
        {
            var index = GetMacroIndex(name);
            if (index == -1) return null;
            return macros[index];
        }
        public List<TextElement> GetMacros(string name)
        {
            return this.GetElements(m => m.GetAttribute("name") == name).ToList();
        }
        public void SetMacro(string name, TextElement tag)
        {
            if(!AllowMultipleNames)
            {
                var index = GetMacroIndex(name);
                if (index == -1)
                {
                    macros.Add(tag);
                }
                else
                {
                    macros[index] = tag;
                }
            }
            else
            {
                macros.Add(tag);
            }

        }

    }
}
