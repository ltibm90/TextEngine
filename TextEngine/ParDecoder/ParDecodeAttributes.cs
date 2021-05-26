using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;

namespace TextEngine.ParDecoder
{
    public enum ParPropRestrictedType
    {
        PRT_RESTRICT_GET = 1 << 0,
        PRT_RESTRICT_SET = 1 << 1,
        PRT_RESTRICT_ALL = PRT_RESTRICT_GET | PRT_RESTRICT_SET
    }
    public class ParDecodeAttributes
    {
        public Dictionary<string, ParPropRestrictedType> RestrictedProperties { get; set; }
        public virtual EvulatorTypes StaticTypes { get; set; }
        public virtual List<string> GlobalFunctions { get; set; }
        public virtual PardecodeFlags Flags { get; set; }
        public virtual ParItemAssignReturnType AssignReturnType { get; set; }
        public virtual bool SurpressError { get; set; }
        //PropertyName, which, ismethod => return false to
        public Func<ParProperty, bool> OnPropertyAccess { get; set; }
        public ParTracer Tracing { get; private set; }
        
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
            this.Tracing = new ParTracer();
        }

    }
}
