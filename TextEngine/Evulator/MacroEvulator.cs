using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Macros;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class MacroEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if (tag.ElemAttr.HasAttribute("preload")) return null;
            var name = tag.GetAttribute("name");

            if (!string.IsNullOrEmpty(name))
            {
                this.Evulator.SavedMacrosList.SetMacro(name, tag);
            }
            return null;
        }
    }
}
