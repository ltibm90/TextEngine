using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public enum IntertwinedBracketsStateType
    {
        IBST_NOT_ALLOWED,
        IBST_ALLOW_ALWAYS,
        IBST_ALLOW_NOATTRIBUTED_ONLY,
        IBST_ALLOW_NOATTRIBUTED_AND_PARAM,
        IBST_ALLOW_PARAM_ONLY
    }
}
