﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class KeyValueGroup : ICollection<KeyValues<object>>
    {
        private List<KeyValues<object>> inner = new List<KeyValues<object>>();
        public KeyValues<object> this[int index]
        {
            get
            {
                return this.inner[index];
            }
            set
            {
                this.inner[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(KeyValues<object> item)
        {
            this.inner.Add(item);
        }

        public void Clear()
        {
            this.inner.Clear();
        }

        public bool Contains(KeyValues<object> item)
        {
            return this.inner.Contains(item);
        }

        public void CopyTo(KeyValues<object>[] array, int arrayIndex)
        {
            this.inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValues<object> item)
        {
           return this.inner.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        IEnumerator<KeyValues<object>> IEnumerable<KeyValues<object>>.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }
        public override string ToString()
        {
            return "Count: " + this.Count.ToString();
        }
        public object GetValue(string name)
        {
            for (int i = this.Count - 1; i >=  0; i--)
            {
                var current = this.inner[i];
                int id = current.GetIdByName(name);
                if (id == -1) continue;
                return current[i];
            }
            return null;
        }
       public void SetValue(string name, object value)
        {
            if(this.Count == 0)
            {
                this.Add(new KeyValues<object>());
            }
            this.inner[this.Count - 1].Set(name, value);
        }
    }
}
