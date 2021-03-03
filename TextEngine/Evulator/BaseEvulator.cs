using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.ParDecoder;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public abstract class BaseEvulator
    {
        protected TextEvulator Evulator { get; set; }
        public BaseEvulator()
        {

        }
        public abstract TextEvulateResult Render(TextElement tag, object vars);
        public virtual void RenderFinish(TextElement tag, object vars, TextEvulateResult latestResult) { }

        protected object EvulatePar(ParDecode pardecoder, object additionalparams = null)
        {
            var addpar = additionalparams as KeyValues<object>;
            if (addpar != null)
            {
                this.Evulator.LocalVariables.Add(addpar);
            }
            var er = pardecoder.Items.Compute(this.Evulator.GloblaParameters, null, this.Evulator.LocalVariables);
            if (addpar != null)
            {
                this.Evulator.LocalVariables.Remove(addpar);
            }
            return er.Result.First();
        }
        protected object EvulateText(string text, object additionalparams = null)
        {
		    var pardecoder = new ParDecode(text);
		    pardecoder.Decode();
            return this.EvulatePar(pardecoder, additionalparams);
        }
        public void SetEvulator(TextEvulator evulator)
        {
            this.Evulator = evulator;
        }
        protected object EvulateAttribute(TextElementAttribute attribute, object additionalparams = null)
        {
            if (attribute == null || string.IsNullOrEmpty(attribute.Value)) return null;
            if(attribute.ParData == null)
            {
                attribute.ParData = new ParDecode(attribute.Value);
                attribute.ParData.Decode();
            }
            return this.EvulatePar(attribute.ParData, additionalparams);
            
        }
        protected bool ConditionSuccess(TextElement tag, string attr = "c")
        {
            ParDecode pardecoder = null;
            if(tag.NoAttrib)
            {
                if (tag.Value == null) return true;
                pardecoder = tag.ParData;
                if(pardecoder == null)
                {
                    pardecoder = new ParDecode(tag.Value);
                    pardecoder.Decode();
                    tag.ParData = pardecoder;
                }
            }
            else
            {
                var cAttr = tag.ElemAttr["c"];
                if (cAttr == null || cAttr.Value ==null) return true;
                pardecoder = cAttr.ParData;
                if(pardecoder == null)
                {
                    pardecoder = new ParDecode(tag.Value);
                    pardecoder.Text = cAttr.Value;
                    pardecoder.Decode();
                    cAttr.ParData = pardecoder;
                }
 
            }
		    var res = this.EvulatePar(pardecoder);
            if(res is bool b)
            {
                return b;
            }
            return false;
        }
    }
}
