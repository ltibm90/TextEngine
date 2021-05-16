using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    [Flags]
    public enum PardecodeFlags
    {
        PDF_Default = 0,
        PDF_AllowMethodCall = 1,
        PDF_AllowSubMemberAccess = 2,
        PDF_AllowArrayAccess = 4,
        PDF_AllowAssigment = 8
        
    }
}
