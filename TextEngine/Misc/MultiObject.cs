using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public sealed class MultiObject : IEnumerable<object>
    {
        private List<object> inner = new List<object>();
        public int Count
        {
            get
            {
                return this.inner.Count;
            }
        }
        public void Add(object obj)
        {
            this.inner.Add(obj);
        }
    
        public IEnumerator<object> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }
        public void Clear()
        {
            this.inner.Clear();
        }
        public object Get(int num)
        {
            if (num < 0 || num >= this.Count) return num;
            return this.inner[num];
        }
    }
}
