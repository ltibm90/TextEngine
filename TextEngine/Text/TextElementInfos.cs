using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementInfos : IEnumerable<TextElementInfo>, ICloneable
    {
        private TextElementInfo lastElement;
        private List<TextElementInfo> inner;
        public bool AutoInitialize { get; set; }
        public TextElementInfos()
        {
            this.AutoInitialize = true;
            inner = new List<TextElementInfo>();
        }
        public TextElementInfo this[string name]
        {
            get
            {
                if (name == "#text") return null;
                if(lastElement != null && name.ToLowerInvariant() == lastElement.ElementName)
                {
                    return lastElement;
                }
                TextElementInfo info = null;
                info =  inner.Where(e => e.ElementName == name.ToLowerInvariant()).FirstOrDefault();
                if(info == null)
                {
                    if(this.AutoInitialize)
                    {
                        info = new TextElementInfo() { ElementName = name.ToLowerInvariant() };
                        this.inner.Add(info);
                    }

                }
                lastElement = info;
                return info;
            }
            set
            {
                if (value == null) return;
                TextElementInfo info = null;
                info = inner.Where(e => e.ElementName == name.ToLowerInvariant()).FirstOrDefault();
                if(info != null)
                {
                    if (info == lastElement) lastElement = null;
                    this.inner.Remove(info);
                }
                value.ElementName = name.ToLowerInvariant();
                this.inner.Add(value);

            }
        }
        public bool HasTagInfo(string tagName)
        {
            return this[tagName] != null;
        }
        public TextElementFlags GetElementFlags(string tagName)
        {
            if (!this.HasTagInfo(tagName)) return TextElementFlags.TEF_NONE;
            return this[tagName].Flags;
        }
        public void Add(TextElementInfo item)
        {
            this.inner.Add(item);
        }

        public void Clear()
        {
            this.inner.Clear();
        }


        public IEnumerator<TextElementInfo> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public TextElementInfos CloneWCS()
        {
            return (TextElementInfos)Clone();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return inner.GetEnumerator();
        }
    }
}
