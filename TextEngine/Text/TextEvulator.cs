using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;
using TextEngine.Macros;
using TextEngine.Misc;
using TextEngine.ParDecoder;

namespace TextEngine.Text
{
    public class TextEvulator
    {
        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.NeedParse = true;
            }
        }
        private TextElement elements;

        public TextElement Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        private bool NeedParse { get; set; }
        private bool m_surpressError;
        public bool SurpressError
        {
            get
            {
                return this.m_surpressError;
            }
            set
            {
                this.m_surpressError = value;
                this.ParAttributes.SurpressError = value;
            }
        }
        public bool ThrowExceptionIFPrevIsNull { get; set; }
        private int Depth { get; set; } = 0;
        public char LeftTag { get; set; } = '{';
        public char RightTag { get; set; } = '}';

        //public string NoParseTag { get; set; } = "noparse";
        public bool NoParseEnabled { get; set; } = true;
        public char ParamChar { get; set; } = '%';
        public Dictionary<string, object> Aliasses { get; private set; }
        public object GlobalParameters { get; set; }
        public KeyValues<object> DefineParameters { get; set; }

        public KeyValueGroup LocalVariables { get; private set; }
        public bool ParamNoAttrib { get; set; }
        public bool DecodeAmpCode { get; set; }
        public bool AllowParseCondition { get; set; }
        public EvulatorTypes EvulatorTypes { get; private set; }
        public bool SupportExclamationTag { get; set; }
        public bool SupportCDATA { get; set; }
        public bool AllowXMLTag { get; set; }
        public bool TrimMultipleSpaces { get; set; }
        public bool TrimStartEnd { get; set; }
        public Dictionary<string, string> AmpMaps { get; private set; }
        public SavedMacros SavedMacrosList { get; private set; }
        public TextElementInfos TagInfos { get; private set; }
        public Dictionary<string, object> CustomDataDictionary { get; private set; }
        public Dictionary<char, string> CharMap { get; set; }
        public object CustomDataSingle { get; set; }
        public bool AllowCharMap { get; set; }
        public Func<EvulatorHandler> EvulatorHandler { get; set; }
        public SpecialCharType SpecialCharOption { get; set; }
        public IntertwinedBracketsStateType IntertwinedBracketsState { get; set; }

        public ParDecodeAttributes ParAttributes { get; private set; }
        public bool ReturnEmptyIfTextEvulatorIsNull { get; set; }
        public EvulatorHandler GetHandler()
        {
            return this.EvulatorHandler?.Invoke();
        }
        private bool isParseMode;
        public bool IsParseMode
        {
            get
            {
                return isParseMode;
            }
            set
            {
                isParseMode = value;
            }
        }
        public void ApplyXMLSettings()
        {
            this.SupportCDATA = true;
            this.SupportExclamationTag = true;
            this.LeftTag = '<';
            this.RightTag = '>';
            this.AllowXMLTag = true;
            this.TrimStartEnd = true;
            this.NoParseEnabled = false;
            this.DecodeAmpCode = true;
            this.TrimMultipleSpaces = true;

        }
        public void ApplyCommandLineByLine()
        {
            this.EvulatorTypes.Text = typeof(TextTagCommandEvulator);
            this.EvulatorTypes.Param = null;
            this.ParAttributes.Flags |= PardecodeFlags.PDF_AllowAssigment;
        }
        public TextEvulator(string text = null, bool isfile = false)
        {
            this.ParAttributes = new ParDecodeAttributes();
            this.IntertwinedBracketsState = IntertwinedBracketsStateType.IBST_ALLOW_NOATTRIBUTED_AND_PARAM;
            this.CharMap = new Dictionary<char, string>();
            this.CustomDataDictionary = new Dictionary<string, object>();
            this.DefineParameters = new KeyValues<object>();
            this.LocalVariables = new KeyValueGroup();
            this.LocalVariables.Add(this.DefineParameters);
            this.ThrowExceptionIFPrevIsNull = true;
            var comparer = StringComparer.OrdinalIgnoreCase;
            this.SavedMacrosList = new SavedMacros();

            this.EvulatorTypes = new EvulatorTypes();
            this.AmpMaps = new Dictionary<string, string>();
            this.Aliasses = new Dictionary<string, object>(comparer);
            this.Elements = new TextElement()
            {
                ElemName = "#document",
                ElementType = TextElementType.Document
            };
            if (isfile)
            {
                this.Text = System.IO.File.ReadAllText(text);
            }
            else
            {
                this.Text = text;
            }
            this.TagInfos = new TextElementInfos();

            this.InitAll();
            if (isfile)
            {
                this.SetDir(System.IO.Path.GetDirectoryName(text));
            }
            this.NeedParse = true;
            this.SpecialCharOption = SpecialCharType.SCT_AllowedAll;
        }
        public void OnTagClosed(TextElement element)
        {
            if (!this.AllowParseCondition || !this.IsParseMode || ((element.GetTagFlags() & TextElementFlags.TEF_ConditionalTag) != 0)) return;
            element.Parent.EvulateValue(element.Index, element.Index + 1);
        }
        public void InitAll()
        {
            this.ClearAllInfos();
            this.InitEvulator();
            this.InitAmpMaps();
            this.InitStockTagOptions();
        }

        public void InitStockTagOptions()
        {
            //* default flags;
            this.TagInfos.Default.Flags = TextElementFlags.TEF_NONE;
            this.TagInfos["elif"].Flags = TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_NoAttributedTag;
            this.TagInfos["else"].Flags = TextElementFlags.TEF_AutoClosedTag;
            this.TagInfos["return"].Flags = TextElementFlags.TEF_AutoClosedTag;
            this.TagInfos["break"].Flags = TextElementFlags.TEF_AutoClosedTag;
            this.TagInfos["continue"].Flags = TextElementFlags.TEF_AutoClosedTag;
            this.TagInfos["include"].Flags = TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_ConditionalTag;
            this.TagInfos["cm"].Flags = TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_AllowQuoteOnAttributeName;
            this.TagInfos["set"].Flags = TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_ConditionalTag;
            this.TagInfos["unset"].Flags = TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_ConditionalTag;
            this.TagInfos["if"].Flags = TextElementFlags.TEF_NoAttributedTag | TextElementFlags.TEF_ConditionalTag;
            this.TagInfos["noparse"].Flags = TextElementFlags.TEF_NoParse;
            this.TagInfos["while"].Flags = TextElementFlags.TEF_NoAttributedTag;
            this.TagInfos["do"].Flags = TextElementFlags.TEF_NoAttributedTag;
            this.TagInfos["text"].Flags = TextElementFlags.TEF_NoParse_AllowParam;
            this.TagInfos["rendersection"].SingleData = this.TagInfos["section"].SingleData = new TextElements();
            this.TagInfos["rendersection"].Flags |= TextElementFlags.TEF_AutoClosedTag | TextElementFlags.TEF_AllowQuoteOnAttributeName;
            this.TagInfos["section"].OnTagClosed = m =>
            {
                var ev = new SectionEvulator();
                ev.SetEvulator(m.BaseEvulator);
                if (!ev.ConditionSuccess(m, "if")) return;
                if (m.Parent != null && m.Parent.ElemName != "#document")
                    return;

                if (m.ElemAttr.FirstAttribute != null)
                {
                    TextElements sections = m.TagInfo.SingleData as TextElements;
                    sections.Add(m);
                }
                //tag.Parent.SubElements.Remove(tag);
            };
            this.TagInfos["macro"].OnTagClosed = m =>
            {
                if (!m.ElemAttr.HasAttribute("preload")) 
                    return;
                var ev = new MacroEvulator();
                ev.SetEvulator(m.BaseEvulator);
                if (!ev.ConditionSuccess(m, "if")) return;
                var name = m.GetAttribute("name");
                if (!string.IsNullOrEmpty(name))
                    m.BaseEvulator.SavedMacrosList.SetMacro(name, m);
                //tag.Parent.SubElements.Remove(tag);
            };
        }
        public void InitEvulator()
        {
            this.EvulatorTypes.Param = typeof(ParamEvulator);
            this.EvulatorTypes.GeneralType = typeof(GeneralEvulator);
            this.EvulatorTypes.Text = typeof(TexttagEvulator);
            this.EvulatorTypes["if"] = typeof(IfEvulator);
            this.EvulatorTypes["for"] = typeof(ForEvulator);
            this.EvulatorTypes["foreach"] = typeof(ForeachEvulator);
            this.EvulatorTypes["switch"] = typeof(SwitchEvulator);
            this.EvulatorTypes["return"] = typeof(ReturnEvulator);
            this.EvulatorTypes["break"] = typeof(BreakEvulator);
            this.EvulatorTypes["continue"] = typeof(ContinueEvulator);
            this.EvulatorTypes["cm"] = typeof(CallMacroEvulator);
            this.EvulatorTypes["macro"] = typeof(MacroEvulator);
            this.EvulatorTypes["noprint"] = typeof(NoPrintEvulator);
            this.EvulatorTypes["repeat"] = typeof(RepeatEvulator);
            this.EvulatorTypes["set"] = typeof(SetEvulator);
            this.EvulatorTypes["unset"] = typeof(UnsetEvulator);
            this.EvulatorTypes["include"] = typeof(IncludeEvulator);
            this.EvulatorTypes["while"] = typeof(WhileEvulator);
            this.EvulatorTypes["do"] = typeof(DoEvulator);
            this.EvulatorTypes["text"] = typeof(TextParamEvulator);
            this.EvulatorTypes["section"] = typeof(SectionEvulator);
            this.EvulatorTypes["rendersection"] = typeof(RenderSectionEvulator);
        }

        public void InitAmpMaps()
        {
            this.AmpMaps["nbsp"] = " ";
            this.AmpMaps["amp"] = "&";
            this.AmpMaps["quot"] = "\"";
            this.AmpMaps["lt"] = "<";
            this.AmpMaps["gt"] = ">";
        }
        public void Parse()
        {
            var parser = new TextEvulatorParser(this);
            parser.Parse(this.Elements, this.Text);
            this.NeedParse = false;
        }
        public void Parse(TextElement baselement, string text)
        {
            var parser = new TextEvulatorParser(this);
            parser.Parse(baselement, text);
        }
        public void SetDir(string dir)
        {
            this.LocalVariables.SetValue("_DIR_", dir);
        }
        public void ClearAllInfos()
        {
            this.TagInfos.Clear();
            this.EvulatorTypes.Clear();
            this.AmpMaps.Clear();
            this.EvulatorTypes.Param = null;
            this.EvulatorTypes.Text = null;
            this.EvulatorTypes.GeneralType = null;
        }
        public void ClearElements()
        {
            this.Elements.SubElements.Clear();
            this.Elements.ElemName = "#document";
            this.Elements.ElementType = TextElementType.Document;
        }
        public TextEvulateResult EvulateValue(object vars = null, bool autoparse = true)
        {
            if (autoparse && this.NeedParse) this.Parse();
            return this.Elements.EvulateValue(0, 0, vars);
        }
    }
}
