using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public enum ParTraceFlags
    {
        PTF_TRACE_PROPERTY = 1 << 0,
        PTF_TRACE_METHOD = 1 << 1,
        PTF_TRACE_INDIS = 1 << 2,
        PTF_TRACE_ASSIGN = 1 << 3,
        PTF_KEEP_VALUE = 1 << 4,
    }
    public class ParTracer
    {
        public List<ParTracerItem> inner;
        public bool Enabled { get; set; }
        public ParTraceFlags Flags { get; set; }
        public int Count
        {
            get
            {
                return this.inner.Count;
            }
        }
        public ParTracer()
        {
            this.inner = new List<ParTracerItem>();
            this.Flags = ParTraceFlags.PTF_KEEP_VALUE | ParTraceFlags.PTF_TRACE_METHOD | ParTraceFlags.PTF_TRACE_PROPERTY | ParTraceFlags.PTF_TRACE_ASSIGN | ParTraceFlags.PTF_TRACE_INDIS;
        }
        public void Clear()
        {
            this.inner.Clear();
        }
        public void Add(ParTracerItem item)
        {
            this.inner.Add(item);  
        }
        public ParTracerItem Get(int id)
        {
            return this.inner[id];
        }
        public ParTracerItem GetField(string name)
        {
            for (int i = 0; i < this.inner.Count; i++)
            {
                if(this.inner[i].Name == name)
                {
                    return this.inner[i];
                }
            }
            return null;
        }
        public List<ParTracerItem> GetFields(string name, int limit = 0)
        {
            var list = new List<ParTracerItem>();
            for (int i = 0; i < this.Count; i++)
            {
                if (this.inner[i].Name == name) list.Add(this.inner[i]);
                if (limit > 0 && list.Count >= limit) break; 
            }
            return list;
        }
        public bool HasFlag(ParTraceFlags flag)
        {
            return (this.Flags & flag) != 0;
        }
        public bool HasTraceThisType(PropType pt)
        {
            switch (pt)
            {
                case PropType.Property:
                    return this.HasFlag(ParTraceFlags.PTF_TRACE_PROPERTY);
                case PropType.Indis:
                    return this.HasFlag(ParTraceFlags.PTF_TRACE_INDIS);
                case PropType.Method:
                    return this.HasFlag(ParTraceFlags.PTF_TRACE_METHOD);
            }
            return false;
        }
    }
}
