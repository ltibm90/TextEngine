﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public enum SpecialCharType
    {
        /// <summary>
        /// \ character disabled
        /// </summary>
        SCT_NotAllowed = 1,
        /// <summary>
        /// e.g(\test, result: test)
        /// </summary>
        SCT_AllowedAll = 2,
        /// <summary>
        /// e.g(\test\{} result: \test{ 
        /// </summary>
        SCT_AllowedClosedTagOnly = 4,
        /// <summary>
        /// e.g{text}\{}{&text} result: {}
        /// </summary>
        SCT_AllowedNoParseWithParamTagOnly = 8,
    }
}
