using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;

namespace TextEngine.ParDecoder
{
    public class ParDecodeAttributes
    {
        public virtual EvulatorTypes StaticTypes { get; set; }
        public virtual List<string> GlobalFunctions { get; set; }
        public virtual PardecodeFlags Flags { get; set; }
        public virtual ParItemAssignReturnType AssignReturnType { get; set; }
        public virtual bool SurpressError { get; set; }
        public ParDecodeAttributes()
        {
            this.Initialise();
        }
        protected virtual void Initialise()
        {
            this.GlobalFunctions = new List<string>();
            this.StaticTypes = new EvulatorTypes();
            this.AssignReturnType = ParItemAssignReturnType.PIART_RETRUN_BOOL;
            this.Flags = PardecodeFlags.PDF_AllowMethodCall | PardecodeFlags.PDF_AllowSubMemberAccess | PardecodeFlags.PDF_AllowArrayAccess;
        }

    }
}
