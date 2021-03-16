using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public abstract class EvulatorHandler
    {
        public virtual bool OnRenderPre(TextElement tag, object vars) { return true; }
        public virtual void OnRenderPost(TextElement tag, object vars, TextEvulateResult result) { }
        public virtual bool OnRenderFinishPre(TextElement tag, object vars, TextEvulateResult result) { return true; }
        public virtual void OnRenderFinishPost(TextElement tag, object vars, TextEvulateResult result) { }
    }
}
