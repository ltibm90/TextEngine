using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.Misc;

namespace TextEngine.Text
{
    public class TextEvulatorParser
    {
        public string Text { get; set; }
        private int pos = 0;
        private bool directclose = false;

        public int TextLength
        {
            get
            {
                return this.Text == null ? 0 : this.Text.Length;
            }
        }
        private TextEvulator evulator;
        public TextEvulator Evulator
        {
            get
            {
                return evulator;
            }
            private set
            {
                evulator = value;
            }
        }
        public TextEvulatorParser(TextEvulator baseevulator)
        {
            this.evulator = baseevulator;
        }
        private void OnTagOpened(TextElement element)
        {
            element.TagInfo?.OnTagOpened?.Invoke(element);
        }

        public void Parse(TextElement baseitem, string text)
        {
            this.Text = text;
            this.Evulator.IsParseMode = true;
            TextElement currenttag = null;
            if (baseitem == null)
            {
                currenttag = this.Evulator.Elements;
            }
            else
            {
                currenttag = baseitem;
            }
            currenttag.BaseEvulator = this.Evulator;
            for (int i = 0; i < TextLength; i++)
            {
                TextElement tag = this.ParseTag(i, currenttag);
                if (tag == null || string.IsNullOrEmpty(tag.ElemName))
                {
                    i = this.pos;
                    continue;
                }

                if (!tag.SlashUsed)
                {
                    OnTagOpened(tag);
                    if (tag.HasFlag(TextElementFlags.TEF_AutoCloseIfSameTagFound))
                    {
                        var prev = this.GetNotClosedPrevTag(tag, tag.ElemName);
                        if (prev != null && !prev.Closed)
                        {
                            prev.CloseState = TextElementClosedType.TECT_AUTOCLOSED;
                            tag.TagInfo?.OnTagClosed?.Invoke(tag);
                            currenttag = this.GetNotClosedPrevTag(prev);
                            tag.Parent = currenttag;
                            if (currenttag == null && this.Evulator.ThrowExceptionIFPrevIsNull && !this.Evulator.SurpressError)
                            {
                                this.Evulator.IsParseMode = false;
                                throw new Exception("Syntax Error");
                            }
                            else if (currenttag == null)
                            {
                                continue;
                            }
                        }
                    }
                    currenttag.AddElement(tag);
                    if (tag.DirectClosed)
                    {
                        tag.TagInfo?.OnTagClosed?.Invoke(tag);
                        this.Evulator.OnTagClosed(tag);
                    }
                }
                if (tag.SlashUsed)
                {
                    TextElement prevtag = this.GetNotClosedPrevTag(tag);
                    //$alltags = $this->GetNotClosedPrevTagUntil($tag, $tag->elemName);
                    // int total = 0;
                    /** @var TextElement $baseitem */
                    TextElement previtem = null;
                    while (prevtag != null)
                    {

                        if (!prevtag.NameEquals(tag.ElemName, true))
                        {
                            var elem = new TextElement
                            {
                                ElemName = prevtag.ElemName,
                                ElemAttr = prevtag.ElemAttr.CloneWCS(),
                                AutoAdded = true,
                                BaseEvulator = this.Evulator
                            };
                            prevtag.CloseState = TextElementClosedType.TECT_CLOSED;
                            prevtag.TagInfo?.OnTagClosed?.Invoke(prevtag);
                            bool allowautocreation = !elem.HasFlag(TextElementFlags.TEF_PreventAutoCreation) && (elem.TagInfo.OnAutoCreating == null || elem.TagInfo.OnAutoCreating(elem));
                            if (allowautocreation)
                            {
                                this.OnTagOpened(elem);
                                if (previtem != null)
                                {
                                    previtem.Parent = elem;
                                    elem.AddElement(previtem);

                                }
                                else
                                {
                                    currenttag = elem;
                                }
                                previtem = elem;
                            }

                        }
                        else
                        {
                            if (prevtag.ElemName != tag.ElemName)
                            {
                                prevtag.AliasName = tag.ElemName;
                                //Alias
                            }
                            if (previtem != null)
                            {
                                previtem.Parent = prevtag.Parent;
                                previtem.Parent.AddElement(previtem);
                            }
                            else
                            {
                                currenttag = prevtag.Parent;
                            }
                            prevtag.CloseState = TextElementClosedType.TECT_CLOSED;
                            prevtag.TagInfo?.OnTagClosed?.Invoke(prevtag);
                            break;
                        }
                        prevtag = this.GetNotClosedPrevTag(prevtag);


                    }
                    if (prevtag == null && this.Evulator.ThrowExceptionIFPrevIsNull && !this.Evulator.SurpressError)
                    {
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                }
                else if (!tag.Closed)
                {
                    currenttag = tag;
                }


                i = this.pos;
            }
            this.pos = 0;
            this.Evulator.IsParseMode = false;
        }
        private TextElement GetNotClosedPrevTag(TextElement tag, string name)
        {
            var stag = this.GetNotClosedPrevTag(tag);
            while (stag != null)
            {
                if (stag.ElemName == name) return stag;
                stag = this.GetNotClosedPrevTag(stag);
            }
            return null;
        }
        private TextElements GetNotClosedPrevTagsUntil(TextElement tag, string name)
        {
            var array = new TextElements();
            var stag = this.GetNotClosedPrevTag(tag);
            while (stag != null)
            {

                if (stag.ElemName == name)
                {
                    array.Add(stag);
                    break;
                }
                array.Add(stag);
                stag = this.GetNotClosedPrevTag(stag);
            }
            return array;
        }

        private TextElement GetNotClosedPrevTag(TextElement tag)
        {
            var parent = tag.Parent;
            while (parent != null)
            {
                if (parent.Closed || parent.ElemName == "#document")
                {
                    return null;
                }
                return parent;
            }
            return null;
        }

        private TextElement GetNotClosedTag(TextElement tag, string name)
        {
            var parent = tag.Parent;
            while (parent != null)
            {
                if (parent.Closed) return null;
                if (parent.NameEquals(name))
                {
                    return parent;
                }
                parent = parent.Parent;
            }
            return null;
        }
        private string DecodeAmp(int start, bool decodedirect = true)
        {
            StringBuilder current = new StringBuilder();

            for (int i = start; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                if (cur == ';')
                {
                    this.pos = i;
                    if (decodedirect)
                    {
                        if (this.Evulator.AmpMaps.TryGetValue(current.ToString(), out string key))
                        {
                            return key;
                        }
                    }
                    else
                    {
                        return current.ToString();
                    }

                    return null;
                }
                if (!char.IsLetterOrDigit(cur))
                {
                    break;
                }
                current.Append(cur);
            }
            this.pos = this.TextLength;
            return '&' + current.ToString();
        }
        private TextElement ParseTag(int start, TextElement parent = null)
        {
            bool inspec = false;
            TextElement tagElement = new TextElement
            {
                Parent = parent,
                BaseEvulator = this.Evulator
            };
            bool istextnode = false;
            bool intag = false;
            bool in_noparse = parent != null && (parent.HasFlag(TextElementFlags.TEF_NoParse) || parent.HasFlag(TextElementFlags.TEF_NoParse_AllowParam));
            for (int i = start; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                var next = '\0';
                if (i + 1 < this.TextLength)
                {
                    next = this.Text[i + 1];
                }
                if (in_noparse && cur == this.Evulator.LeftTag && (next != this.Evulator.ParamChar || !parent.HasFlag(TextElementFlags.TEF_NoParse_AllowParam)))
                {
                    istextnode = true;
                    tagElement.SetTextTag(true);
                }
                else
                {
                    if (!inspec)
                    {
                        if (cur == this.Evulator.LeftTag)
                        {
                            if (intag)
                            {
                                if (this.Evulator.SurpressError)
                                {
                                    tagElement.SetTextTag(true);
                                    tagElement.Value = this.Text.Substring(start, i - start);
                                    this.pos = i - 1;
                                    return tagElement;
                                }
                                else
                                {
                                    this.Evulator.IsParseMode = false;
                                    throw new Exception("Syntax Error");
                                }
                            }
                            intag = true;
                            continue;
                        }
                        else if (!in_noparse && this.Evulator.DecodeAmpCode && cur == '&')
                        {
                            string ampcode = this.DecodeAmp(i + 1, false);
                            i = this.pos;
                            tagElement.SetTextTag(true);
                            tagElement.ElementType = TextElementType.EntityReferenceNode;
                            if (ampcode.StartsWith("&") && this.Evulator.SurpressError)
                            {
                                if (this.Evulator.SurpressError)
                                {
                                    tagElement.ElementType = TextElementType.TextNode;
                                }
                                else
                                {
                                    this.Evulator.IsParseMode = false;
                                    throw new Exception("Syntax Error");
                                }
                            }
                            tagElement.CloseState = TextElementClosedType.TECT_AUTOCLOSED;
                            tagElement.Value = ampcode;
                            return tagElement;
                        }
                        else
                        {
                            if (!intag)
                            {
                                istextnode = true;
                                tagElement.SetTextTag(true);
                            }
                        }
                    }
                    if (!inspec && cur == this.Evulator.RightTag)
                    {
                        if (!intag)
                        {
                            if (this.Evulator.SurpressError)
                            {
                                tagElement.SetTextTag(true);
                                tagElement.Value = this.Text.Substring(start, i - start);
                                this.pos = i - 1;
                                return tagElement;

                            }
                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }
                        intag = false;
                    }
                }
                this.pos = i;
                if (!intag || istextnode)
                {
                    tagElement.Value = this.ParseInner(parent);
                    if (!in_noparse && tagElement.ElementType == TextElementType.TextNode && string.IsNullOrEmpty(tagElement.Value))
                    {
                        return null;
                    }
                    intag = false;
                    if (this.directclose && in_noparse)
                    {
                        parent.AddElement(tagElement);
                        var elem = new TextElement
                        {
                            Parent = parent,
                            ElemName = parent.ElemName,
                            SlashUsed = true
                        };
                        return elem;
                    }
                    return tagElement;
                }
                else
                {
                    this.ParseTagHeader(ref tagElement);
                    intag = false;
                    if (string.IsNullOrEmpty(tagElement.ElemName)) return null;

                    return tagElement;

                }
            }
            return tagElement;
        }
        private void ParseTagHeader(ref TextElement tagElement)
        {
            bool inquot = false;
            bool inspec = false;
            StringBuilder current = new StringBuilder();
            bool namefound = false;
            //bool inattrib = false;
            bool firstslashused = false;
            bool lastslashused = false;
            StringBuilder currentName = new StringBuilder();
#pragma warning disable CS0219 // Değişken atandı ancak değeri hiç kullanılmadı
            bool quoted = false;
#pragma warning restore CS0219 // Değişken atandı ancak değeri hiç kullanılmadı
            char quotchar = '\0';
            bool initial = false;
            bool istagattrib = false;
            int totalPar = 0;
            for (int i = this.pos; i < this.TextLength; i++)
            {
                var cur = this.Text[i];


                if (inspec)
                {
                    inspec = false;
                    current.Append(cur);
                    continue;
                }
                var next = '\0';
                var next2 = '\0';
                if (i + 1 < this.TextLength)
                {
                    next = this.Text[i + 1];
                }
                if (i + 2 < this.TextLength)
                {
                    next2 = this.Text[i + 2];
                }
                if (tagElement.ElementType == TextElementType.CDATASection)
                {
                    if (cur == ']' && next == ']' && next2 == this.Evulator.RightTag)
                    {
                        tagElement.Value = current.ToString();
                        this.pos = i += 2;
                        return;
                    }
                    current.Append(cur);
                    continue;
                }
                if (this.Evulator.AllowXMLTag && cur == '?' && !namefound && current.Length == 0)
                {
                    tagElement.CloseState = TextElementClosedType.TECT_AUTOCLOSED;
                    tagElement.ElementType = TextElementType.XMLTag;
                    continue;

                }
                if (this.Evulator.SupportExclamationTag && cur == '!' && !namefound && current.Length == 0)
                {
                    tagElement.CloseState = TextElementClosedType.TECT_AUTOCLOSED;
                    if (i + 8 < this.TextLength)
                    {
                        var mtn = this.Text.Substring(i, 8);
                        if (this.Evulator.SupportCDATA && mtn == "![CDATA[")
                        {
                            tagElement.ElementType = TextElementType.CDATASection;
                            tagElement.ElemName = "#cdata";
                            namefound = true;
                            i += 7;
                            continue;
                        }
                    }
                }
                if (cur == '\\' && tagElement.ElementType != TextElementType.CommentNode)
                {
                    if (!namefound && tagElement.ElementType != TextElementType.Parameter)
                    {
                        if (this.Evulator.SurpressError) continue;
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                    inspec = true;
                    continue;
                }

                if (!initial && cur == '!' && next == '-' && next2 == '-')
                {
                    tagElement.ElementType = TextElementType.CommentNode;
                    tagElement.ElemName = "#summary";
                    tagElement.CloseState = TextElementClosedType.TECT_CLOSED;
                    i += 2;
                    continue;
                }
                if (tagElement.ElementType == TextElementType.CommentNode)
                {
                    if (cur == '-' && next == '-' && next2 == this.Evulator.RightTag)
                    {
                        tagElement.Value = current.ToString();
                        this.pos = i + 2;
                        return;
                    }
                    else
                    {
                        current.Append(cur);
                    }
                    continue;
                }
                initial = true;
                if (this.Evulator.DecodeAmpCode && tagElement.ElementType != TextElementType.CommentNode && cur == '&')
                {
                    current.Append(this.DecodeAmp(i + 1));
                    i = this.pos;
                    continue;
                }

                if ((tagElement.ElementType == TextElementType.Parameter && this.Evulator.ParamNoAttrib)
                     || (namefound && tagElement.NoAttrib) || (istagattrib && tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly)))
                {
                    if (inquot && quotchar == cur)
                    {
                        inquot = false;
                    }
                    else if (!inquot && (cur == '\'' || cur == '"'))
                    {
                        inquot = true;
                        quotchar = cur;
                    }
                    if (!inquot && cur == this.Evulator.LeftTag && tagElement.AllowIntertwinedPar)
                    {
                        totalPar++;
                    }
                    if (inquot || totalPar > 0 || (cur != this.Evulator.RightTag && tagElement.ElementType == TextElementType.Parameter) ||
                        cur != this.Evulator.RightTag && (cur != '/' && next != this.Evulator.RightTag ||
                        (tagElement.TagFlags & TextElementFlags.TEF_DisableLastSlash) != 0))
                    {
                        if (!inquot && cur == this.Evulator.RightTag && totalPar > 0) totalPar--;
                        current.Append(cur);
                        continue;
                    }
                }
                if (firstslashused && namefound)
                {
                    if (cur != this.Evulator.RightTag)
                    {
                        if (cur == ' ' && next != '\t' && next != ' ')
                        {
                            if (this.Evulator.SurpressError) continue;
                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }
                    }
                }
                if (cur == '"' || cur == '\'')
                {
                    if(!namefound)
                    {
                        if (this.Evulator.SurpressError) continue;
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                    if (currentName.Length == 0)
                    {
                        if(!tagElement.TagFlags.HasFlag(TextElementFlags.TEF_AllowQuoteOnAttributeName))
                        {
                            if (this.Evulator.SurpressError) continue;
                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }

                    }
                    if (inquot && cur == quotchar)
                    {
                        bool clearcurrentname = true;
                        if (istagattrib)// if (currentName.ToString() == "##set_TAG_ATTR##")
                        {
                            tagElement.TagAttrib = current.ToString();
                            istagattrib = false;

                        }
                        else if (currentName.Length > 0 && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                        {

                            tagElement.ElemAttr.SetAttribute(currentName.ToString(), current.ToString());
                        }
                        else if(currentName.Length == 0 && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                        {
                            clearcurrentname = false;
                            currentName.Append(current.ToString());

                        }
                        if(clearcurrentname) currentName.Clear();
                        current.Clear();
                        inquot = false;
                        quoted = true;
                        continue;
                    }
                    else if (!inquot)
                    {
                        quotchar = cur;
                        inquot = true;
                        continue;
                    }


                }
                if (!inquot)
                {
                    if (cur == this.Evulator.ParamChar && !namefound && !firstslashused)
                    {
                        tagElement.ElementType = TextElementType.Parameter;
                        tagElement.CloseState = TextElementClosedType.TECT_CLOSED;
                        continue;
                    }
                    if (cur == '/')
                    {
                        if (!namefound && current.Length > 0)
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                        }
                        if (namefound)
                        {
                            if (next == this.Evulator.RightTag && (tagElement.TagFlags & TextElementFlags.TEF_DisableLastSlash) == 0)
                            {
                                lastslashused = true;
                            }

                        }
                        else
                        {
                            firstslashused = true;
                        }
                        if ((tagElement.TagFlags & TextElementFlags.TEF_DisableLastSlash) != 0)
                        {
                            current.Append(cur);
                        }
                        continue;
                    }
                    if (cur == '=')
                    {
                        if (namefound)
                        {
                            if (istagattrib)
                            {
                                current.Append(cur);
                            }
                            else
                            {

                                if (current.Length == 0)
                                {
                                    if (this.Evulator.SurpressError) continue;
                                    this.Evulator.IsParseMode = false;
                                    throw new Exception("Syntax Error");
                                }
                                currentName.Clear();
                                currentName.Append(current.ToString());
                                current.Clear();
                            }

                        }
                        else
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                            currentName.Clear();
                            //currentName.Append("##set_TAG_ATTR##");
                            istagattrib = true;
                        }
                        continue;
                    }
                    if (tagElement.ElementType == TextElementType.XMLTag)
                    {
                        if (cur == '?' && next == this.Evulator.RightTag)
                        {
                            cur = next;
                            i++;
                        }
                    }



                    if (cur == this.Evulator.RightTag)
                    {
                        if (totalPar > 0)
                        {
                            totalPar--;
                            current.Append(cur);
                            continue;
                        }
                        if (!namefound)
                        {
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                        }
                        if (tagElement.NoAttrib)
                        {
                            tagElement.Value = current.ToString();
                        }
                        else if (istagattrib) //(currentName.ToString() == "##set_TAG_ATTR##")
                        {
                            tagElement.TagAttrib = current.ToString();
                            istagattrib = false;
                        }
                        else if (currentName.Length > 0 && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                        {
                            tagElement.SetAttribute(currentName.ToString(), current.ToString());
                        }
                        else if (current.Length > 0 && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                        {
                            tagElement.SetAttribute(current.ToString(), null);
                        }
                        tagElement.SlashUsed = firstslashused;
                        if (lastslashused)
                        {
                            tagElement.CloseState = TextElementClosedType.TECT_DIRECTCLOSED;
                        }
                        string elname = tagElement.ElemName.ToLowerInvariant();
                        if ((this.Evulator.TagInfos.GetElementFlags(elname) & TextElementFlags.TEF_AutoClosedTag) != 0)
                        {

                            tagElement.CloseState = TextElementClosedType.TECT_AUTOCLOSED;
                            tagElement.TagInfo?.OnTagClosed?.Invoke(tagElement);
                        }
                        this.pos = i;
                        return;
                    }
                    if (cur == ' ')
                    {
                        if (next == ' ' || next == '\t' || next == this.Evulator.RightTag) continue;
                        if (!namefound && !PhpFuctions.empty(current))
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();


                        }
                        else if (namefound)
                        {
                            if (istagattrib) //(currentName.ToString() == "##set_TAG_ATTR##")
                            {
                                tagElement.TagAttrib = current.ToString();
                                quoted = false;
                                currentName.Clear();
                                current.Clear();
                                istagattrib = false;
                            }
                            else if (!PhpFuctions.empty(currentName) && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                            {
                                tagElement.SetAttribute(currentName.ToString(), current.ToString());
                                currentName.Clear();
                                current.Clear();
                                quoted = false;
                            }
                            else if (!PhpFuctions.empty(current) && !tagElement.HasFlag(TextElementFlags.TEF_TagAttribonly))
                            {
                                tagElement.SetAttribute(current.ToString(), null);
                                current.Clear();
                                quoted = false;
                            }
                        }
                        continue;
                    }
                    if (cur == this.Evulator.LeftTag)
                    {
                        if (!tagElement.AllowIntertwinedPar)
                        {
                            if (this.Evulator.SurpressError) continue;

                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }
                        totalPar++;

                    }
                }
                current.Append(cur);
            }
            this.pos = this.TextLength;
        }
        private string ParseInner(TextElement parent, bool isat = false)
        {
            StringBuilder text = new StringBuilder();
            bool inspec = false;
            StringBuilder nparsetext = new StringBuilder();
            bool parfound = false;
            StringBuilder waitspces = new StringBuilder();
            bool in_noparse = parent != null && (parent.HasFlag(TextElementFlags.TEF_NoParse) || parent.HasFlag(TextElementFlags.TEF_NoParse_AllowParam));
            this.directclose = false;
            for (int i = this.pos; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                var next = (i + 1 < this.TextLength) ? this.Text[i + 1] : '\0';
                if (inspec)
                {
                    inspec = false;
                    text.Append(cur);
                    continue;
                }
                if (cur == '\\')
                {
                    if (this.Evulator.SpecialCharOption == SpecialCharType.SCT_AllowedAll || ((this.Evulator.SpecialCharOption & SpecialCharType.SCT_AllowedClosedTagOnly) != 0 && next == this.Evulator.RightTag) 
                        || ((this.Evulator.SpecialCharOption & SpecialCharType.SCT_AllowedNoParseWithParamTagOnly) != 0 && in_noparse && parent.HasFlag(TextElementFlags.TEF_NoParse_AllowParam)))
                    {
                        inspec = true;
                        continue;
                    }
                }
                if (this.Evulator.AllowCharMap && cur != this.Evulator.LeftTag && cur != this.Evulator.RightTag && this.Evulator.CharMap.Keys.Count > 0)
                {
                    if (this.Evulator.CharMap.TryGetValue(cur, out string str))
                    {
                        if (parfound)
                        {
                            nparsetext.Append(str);
                        }
                        else
                        {
                            text.Append(str);
                        }
                        continue;
                    }
                }

                //if (this.DecodeAmpCode && cur == '&')
                //{
                //    text.Append(this.DecodeAmp(i + 1));
                //    i = this.pos;
                //    continue;
                //}
                if (this.Evulator.NoParseEnabled && in_noparse)
                {
                    if (parfound)
                    {

                        if (cur == this.Evulator.LeftTag || cur == '\r' || cur == '\n' || cur == '\t' || cur == ' ')
                        {
                            text.Append(this.Evulator.LeftTag + nparsetext.ToString());
                            parfound = (cur == this.Evulator.LeftTag);
                            nparsetext.Clear();
                        }
                        else if (cur == this.Evulator.RightTag)
                        {
                            if (nparsetext.ToString().ToLowerInvariant() == '/' + parent.ElemName.ToLowerInvariant())
                            {
                                parfound = false;
                                this.pos = i;
                                this.directclose = true;
                                if (this.Evulator.TrimStartEnd)
                                {
                                    return text.ToString().Trim();
                                }
                                return text.ToString();
                            }
                            else
                            {
                                text.Append(this.Evulator.LeftTag + nparsetext.ToString() + cur);
                                parfound = false;
                                nparsetext.Clear();
                            }
                            continue;
                        }

                    }
                    else
                    {
                        if (cur == this.Evulator.LeftTag)
                        {
                            if (next == this.Evulator.ParamChar && parent.HasFlag(TextElementFlags.TEF_NoParse_AllowParam))
                            {
                                this.pos = i - 1;
                                this.directclose = false;
                                if (this.Evulator.TrimStartEnd)
                                {
                                    return text.ToString().Trim();
                                }
                                return text.ToString();
                            }
                            parfound = true;
                            continue;
                        }
                    }
                }
                else
                {
                    if (!inspec && cur == this.Evulator.LeftTag || this.Evulator.DecodeAmpCode && cur == '&')
                    {
                        this.pos = i - 1;
                        if (this.Evulator.TrimStartEnd)
                        {
                            return text.ToString().Trim();
                        }
                        return text.ToString();
                    }
                }
                if (parfound)
                {
                    nparsetext.Append(cur);
                }
                else
                {
                    if (this.Evulator.TrimMultipleSpaces)
                    {
                        if (cur == ' ' && next == ' ') continue;
                    }
                    text.Append(cur);
                }
            }
            this.pos = this.TextLength;
            if (this.Evulator.TrimStartEnd)
            {
                return text.ToString().Trim();
            }
            return text.ToString();
        }
    }
}
