using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class EvulatorInstanceOptions
    {
        /// <summary>
        /// Must be 2 length
        /// </summary>
        public string LeftRightTag { get; set; } = "[]";
        /// <summary>
        /// set true if content is file
        /// </summary>
        public bool ContentIsFile { get; set; }
        //Any object types is supported(Anonymous object, Class, Dictionary ...)
        public object GlobalParameters { get; set; }
    }
}
