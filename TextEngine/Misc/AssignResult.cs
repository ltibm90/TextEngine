using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class AssignResult
    {
        public bool Success { get; set; }
        public object AssignedValue { get; set; }

        public static implicit operator bool(AssignResult a)
        {
            return a != null && a.Success;
        }


    }
}
