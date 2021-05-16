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
        private KeyValues<object> localVars;
        protected TextEvulator Evulator { get; set; }   
        public BaseEvulator()
        {

        }
        protected ParDecode CreatePardecode(string text, bool decode = true)
        {
            var pd = new ParDecode(text);
            pd.OnGetFlags = () => this.Evulator.ParFlags;
            if (decode) pd.Decode();
            return pd;
        }
        public abstract TextEvulateResult Render(TextElement tag, object vars);
        public virtual void RenderFinish(TextElement tag, object vars, TextEvulateResult latestResult) { }

        protected object EvulatePar(ParDecode pardecoder, object additionalparams = null)
        {
            if(pardecoder.SurpressError != this.Evulator.SurpressError)
            {
                pardecoder.SurpressError = this.Evulator.SurpressError;
            }
            ComputeResult er = null;
            if(additionalparams == null)
            {
                er = pardecoder.Items.Compute(this.Evulator.GlobalParameters, null, this.Evulator.LocalVariables);
            }
            else
            {
                var multi = new MultiObject();
                multi.Add(additionalparams);
                multi.Add(this.Evulator.GlobalParameters);
                er = pardecoder.Items.Compute(multi, null, this.Evulator.LocalVariables);
            }

            return er.Result.First();
        }
        protected object EvulateText(string text, object additionalparams = null)
        {
		    var pardecoder = this.CreatePardecode(text);
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
                attribute.ParData = this.CreatePardecode(attribute.Value);
            }
            return this.EvulatePar(attribute.ParData, additionalparams);
            
        }
        protected bool ConditionSuccess(TextElement tag, string attr = "*", object vars = null)
        {
            ParDecode pardecoder = null;
            if((attr == null || attr == "" || attr == "*") && tag.NoAttrib)
            {
                if (tag.Value == null) return true;
                pardecoder = tag.ParData;
                if(pardecoder == null)
                {

                    pardecoder = this.CreatePardecode(tag.Value);
                    tag.ParData = pardecoder;
                }
            }
            else
            {
                if (attr == "*") attr = "c";
                var cAttr = tag.ElemAttr[attr];
                if (cAttr == null || cAttr.Value ==null) return true;
                pardecoder = cAttr.ParData;
                if(pardecoder == null)
                {
                    pardecoder  = this.CreatePardecode(cAttr.Value);
                    cAttr.ParData = pardecoder;
                }
 
            }
		    var res = this.EvulatePar(pardecoder, vars);
            if(res is bool b)
            {
                return b;
            }
            return false;
        }
        protected void CreateLocals()
        {
            if (this.localVars != null) return;
            this.localVars = new KeyValues<object>();
            this.Evulator.LocalVariables.Add(this.localVars);
        }
        protected void DestroyLocals()
        {
            if (this.localVars == null) return;
            this.Evulator.LocalVariables.Remove(this.localVars);
            this.localVars = null;
        }
        protected void SetLocal(string name, object value)
        {
            this.localVars.Set(name, value);
        }
        protected object GetLocal(string name)
        {
            return this.localVars[name];
        }
    }
}
