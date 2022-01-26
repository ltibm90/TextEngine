using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextEngine.Evulator;
using TextEngine.Misc;
using TextEngine.ParDecoder;
using TextEngine.Text;

namespace TextEngineTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        private void NoParseTest()
        {
            TextEvulator evulator = new TextEvulator();
            evulator.ParamNoAttrib = true;
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.NoParseEnabled = true;
            evulator.Text = "[NOPARSE]Deneme [B]Test[/B][/NOPARSE][B]Deneme[/B]";
            evulator.Parse();
        }
        private void ParFormatTest()
        {
            //Long usage
            ParFormat pf = new ParFormat();
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["name"] = "MacMillan";
            kv["grup"] = "AR-GE";
            kv["random"] = (Func<int>)delegate () {
                return new Random().Next(1, 100);
            };
            pf.ParAttributes.SurpressError = true;
            pf.Text = "User: {%name}, Group: {%grup}, Random Number: {%random()}";

            //Short usage
            ParFormat.Format("User: {%name}, Group: {%grup}, Number Sayı: {%random()}", kv);


            //Output  User: MacMillan, Group: AR-GE, Random Number: 61
            string res = pf.Apply(kv);
        }
        private void GeneralTest()
        {
            /* dynamic obj = new ExpandoObject();
             obj.Func = (Func<string>)delegate () {
                 string sf = "mac";
                 return sf;
             };
             Dictionary<string, object> objd = new Dictionary<string, object>();
             objd["Func"] = obj.Func;
             var s =ParFormat.Format("mac {%Func()}", obj);*/
        }
        class WhileTestClass
        {
            public WhileTestClass()
            {
                this.Items = new List<string>();
                this.Position = -1;
            }
            public int Position { get; set; }
            public List<string> Items { get; private set; }
            public bool Next()
            {
                return ++this.Position < this.Items.Count;
            }
            public string Get()
            {
                return this.Items[this.Position];
            }
        }
        private void DoTest()
        {
            var evulator = new TextEvulator();
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.ParamNoAttrib = true;
            evulator.Text = "[do loop_count == 0 || loop_count < 5]Do: [%loop_count][/do]";
            var result = evulator.EvulateValue();
        }
        private void WhileTest()
        {
            var wtc = new WhileTestClass();
            wtc.Items.Add("Item1");
            wtc.Items.Add("Item2");
            wtc.Items.Add("Item3");
            wtc.Items.Add("Item4");
            wtc.Items.Add("Item5");
            wtc.Items.Add("Item6");
            var evulator = new TextEvulator();
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.ParamNoAttrib = true;
            evulator.Text = "[while Next()][%loop_count + 1]: -[%Get()][/while]";
            evulator.GlobalParameters = wtc;
            var result = evulator.EvulateValue(new object());
        }
        public class TestAssignClass
        {
            public int IntProp { get; set; }
            public int IntProp2 { get; set; }
            public int IntProp3 { get; set; }
            public int IntProp4 { get; set; }
            public string StringProp { get; set; }
            public List<string> Items { get; set; }
            public int[] ItemArrays { get; set; }
            public Dictionary<string ,object> DictItems { get; set; }
            public TestAssignClass()
            {
                this.DictItems = new Dictionary<string, object>();
                this.DictItems["str1"] = "string var";
               
            }
        }
        private void AssignTest()
        {
            var p = new TestAssignClass();
            p.Items = new List<string>();
            p.Items.Add("item1");
            p.Items.Add("item2");
            var pf = new ParFormat();
            pf.Text = "{%Items[0] = 'item1 changed'} {%ItemArrays = [1, 2, 3]} {%DictItems = {'a': 1, 'b': 2, 'c': 3}} {%DictItems.a += 5} {%StringProp = 'string'} {%IntProp = 1903}";
            pf.ParAttributes.Flags |= PardecodeFlags.PDF_AllowAssigment;
            var res = pf.Apply(p);
        }
        private void CommandLineByLineText()
        {
            dynamic obj = new ExpandoObject();
            obj.Item1 = 1;
            obj.Item2 = 2;
            obj.Item3 = 3;
            TextEvulator te = new TextEvulator("Item1 = Item2 + 1\r\nItem2 = Item3 + 1\r\nItem3 = Item1 + Item2");
            te.GlobalParameters = obj;
            te.ApplyCommandLineByLine();
            te.Parse();
            var res = te.EvulateValue();
        }
        public class DenemeClass
        {
            public DenemeClass(bool isalt = false)
            {
                if (isalt) return;
                this.AltClass = new DenemeClass(true);
            }
            public int Test()
            {
                return 1;
            }
            public DenemeClass AltClass { get; set; }
        }
        public class MyGlobalFunctions
        {
            public static int Clamp(int value, int min, int max)
            {
                if (value > max) value = max;
                else if (value < min) value = min;
                return value;
            }
        }
        public void GlobalFunctionsTest()
        {
           var r = ParFormat.FormatEx("Value: {%Clamp(IntVal, 5, 15)}, IsNullOrEmpty test prop: {%String::IsNullOrEmpty(Test)}", new { Test= "a", IntVal = 25}, (m) => {
                m.StaticTypes.GeneralType = typeof(MyGlobalFunctions);
                m.StaticTypes["String"] = typeof(String);
                m.GlobalFunctions.Add("String::");
                m.GlobalFunctions.Add("::");
               m.OnPropertyAccess = (property) =>
               {
                   if (property.Name == "Clamp") return false;
                   return true;
               };
           });
            //Overloaded functions currently not supported.
        }
        private void OperatorsTest()
        {
            TestAssignClass test = new TestAssignClass();
            test.IntProp = 3; //Assigmenting... first: 7, last: 4
            test.IntProp2 = 0; //Assigmenting... first: 16, last: 0
            test.IntProp3 = 1; //After assigment 16
            test.IntProp4 = 4; //After assigment 2
            //Returned 71640162
            var r = ParFormat.FormatEx("{%IntProp |= 4}{%IntProp2 = 1 << 4}{%IntProp &= 4}{%IntProp2 = 1 >> 4}{%IntProp3 <<= 4}{%IntProp4 >>= 1}", test, (m) => {
                m.Flags |= PardecodeFlags.PDF_AllowAssigment;
            });
           
        }
        public class Class3
        {
            public string Test { get; set; }
        }
        public class Class2
        {
            public string StartupPosition { get; set; }
            public bool UseFenFormat { get; set; }
        }
        private void TextTest()
        {
            TextEvulator te = new TextEvulator("{TEXT}This result is print {%3*4+5}{/TEXT}This is value is not print {TEXT}Result is print{/TEXT}");
            //For disable parameter tag
            te.TagInfos["text"].Flags = TextElementFlags.TEF_NoParse;

            //For enable parameter tag
            te.TagInfos["text"].Flags = TextElementFlags.TEF_NoParse_AllowParam; //Default

            te.ReturnEmptyIfTextEvulatorIsNull = true;
            te.EvulatorTypes.Text = null;
            te.ParamNoAttrib = true;
            te.Parse();
            var r = te.EvulateValue();
        }
        public class SubClass
        {
            public string SubMember { get; set; }
            public int SubMethod()
            {
                return 10;
            }
        }
        public class TraceClass
        {
            public TraceClass()
            {
                this.SubItem = new SubClass();
            }
            public int IntProp { get; set; }
            public bool TestMethod(int num)
            {
                return num > 0;
            }
            public SubClass SubItem { get; set; }
        }
        private void TracenRestrictionAndHandleTest()
        {
            TextEvulator te = new TextEvulator("IntProp = 5\r\nTestMethod(5)\r\nSubItem.SubMember = 'testString'\r\nSubItem.SubMethod()");
            te.ApplyCommandLineByLine();
            te.ParAttributes.Tracing.Enabled = true;
            //Initialize first
            te.ParAttributes.RestrictedProperties = new Dictionary<string, ParPropRestrictedType>();
            //Restricted IntProp Get and Set
            te.ParAttributes.RestrictedProperties["IntProp"] = ParPropRestrictedType.PRT_RESTRICT_GET | ParPropRestrictedType.PRT_RESTRICT_SET;
            //or
            te.ParAttributes.RestrictedProperties["IntProp"] = ParPropRestrictedType.PRT_RESTRICT_ALL;
            te.ParAttributes.OnPropertyAccess = (prop) =>
            {
                //Prevent to access property if returned false
                // return false;
                return true;
            };
            te.EvulateValue(new TraceClass());
            if((bool)te.ParAttributes.Tracing.GetField("TestMethod").Value)
            {
                MessageBox.Show("Test Method returned true");
            }
            

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TextEvulator te = new TextEvulator();
            te.Text = "{set name=array value=[1,2,3,4,5,6]}{for var=i to=array.Length}{%array[i]}{/for}";
           var res = te.EvulateValue();
            TracenRestrictionAndHandleTest();
            TextTest();
            OperatorsTest();
            GlobalFunctionsTest();
            AssignTest();
            CommandLineByLineText();
            DoTest();
            WhileTest();
            GeneralTest();
            NoParseTest();
            ParFormatTest();
            //TextEngineTest6();
            TextEvulator evulator = new TextEvulator();
            evulator.ParamNoAttrib = true;
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.Text = "[";
          //  evulator.Text = "deneme{tag}içerik: <b>{%'Mesaj: ' + mesaj + ', Uzunluk: ' + strlen_cw(mesaj) + ':'}</b>{/tag}";
            evulator.Parse();
            evulator.Elements.EvulateValue();
            evulator.Text = "<cw><uyeler><uye name='macmillan'>Üye</uye><uye name='xuye'>XÜye</uye><uye attr='kestane'>YÜye</uye></uyeler></cw>";
            //Parametrelerin attirbute alamayacağınıbelirttik aksi halde yukarı kod syntax hatası verecektir.
            evulator.GlobalParameters = new CustomClass();
            evulator.ParamNoAttrib = true;
            evulator.LeftTag = '<';
            evulator.RightTag = '>';
            evulator.Parse();
           //var items = evulator.Elements.FindByXPath("(cw/uyeler/uye)[@name]");
            var items = evulator.Elements.FindByXPath("cw/uyeler/uye[@name='xuye' or @name='macmillan'][1]");
          
            var result = evulator.Elements.EvulateValue();
            MessageBox.Show(result.TextContent);




        }
        public class CustomClass
        {
            public int MyProperty { get; set; }
            public string mesaj { get; set; } = "Mesaj";
            public string Deneme()
            {
                return "215";
            }
            public int strlen_cw(string str)
            {
                return str.Length;
            }
        }
        private void AçiklamaSatiri()
        {
            TextEvulator evulator = new TextEvulator();
            evulator.Text = "Ayrıştırılacak Metin";
            evulator.ParamChar = '%'; //Parametre karakteri {%variant} olarak kullanılır.
            evulator.LeftTag = '{'; //{tag} içerisindeki sol kısım
            evulator.RightTag = '}'; //{tag} içersinideki {tag} sağ kısım.
            //evulator.NoParseTag = "NOPARSE"; //{NOPARSE}{/NOPARSE} içerisindeki kodlar ayrıştırılmaz.
            evulator.ApplyXMLSettings(); //XML Ayrıştırıcı olarak kullanacaksınız bu fonksiyonu çağırın öncelikle
            evulator.DecodeAmpCode = true; //&nbsp; gibi komutları çevirir.
            evulator.ParamNoAttrib = true; //Parametlerin attribute kullanıp kullanamayacağını belirler.
            //Örneğin false ayarlanırsa {%i+5} şeklinde kullanamazsınız fakat true olarak ayarlandığında {%i + 5 *2} gibi
            //kullanabilirsiniz.
            evulator.GlobalParameters = new object(); //Evulate işlemi sırasındaki değişkenler bu değişken aracılığı il
            //Aşağıdaki şekillerdek ullanabilirsiniz
            var test = new
            {
                name = "Deneme",
                value = 1000
            };
            evulator.GlobalParameters = test;
            IDictionary<string, object> dict = new Dictionary<string, object>();
            evulator.GlobalParameters = dict;
            KeyValues<object> kv = new KeyValues<object>();
            evulator.GlobalParameters = kv;
            evulator.GlobalParameters = new CustomClass();
            evulator.TagInfos["test"].Flags =  TextElementFlags.TEF_AutoClosedTag; //ismi yazılan taglar otomatik kapatılır
            evulator.Aliasses.Add("bb", "strong"); //bb kodu aynı zamanda strong olarakta kullanılabilir.
            evulator.Parse(); //Ayrıştırma işlemi yapılır
            var elems = evulator.Elements; //Ayrıştırılan elemanlar bu sınıfta tutulur.
            elems.EvulateValue(); //ELeman içeriğini verilen parametrelere göre değerlendirip sonucunu döner.
            
        }

        private void TextEngineTest1()
        {
            TextEvulator evulator = new TextEvulator();
            evulator.Text = "{set name='vars' value='isim + 5'}{if vars==10}Değerler: İsim {%isim}, vars: {%vars}{/if}";
            evulator.Parse();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["isim"] = 5;
            //Desteklenen sınıflar
            //IDictionary<string, object>, KeyValueGrup veye KeyValues<object> veya Doğrudan sınıflar belirtilebilir.
            evulator.GlobalParameters = dict;
            var result =  evulator.Elements.EvulateValue();
        }
        private void TextEngineTest2()
        {
            TextEvulator evulator = new TextEvulator("metin.txt", true);
            evulator.Parse();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["isim"] = 5;
            //Desteklenen sınıflar
            //IDictionary<string, object>, KeyValueGrup veye KeyValues<object> veya Doğrudan sınıflar belirtilebilir.

            evulator.GlobalParameters = dict;
            var result = evulator.Elements.EvulateValue();
        }
        private void TextEngineTest3()
        {
            TextEvulator evulator = new TextEvulator("{cw}" +
                "{Üyeler}{Üye boşiçerik}{Üye name='macmillan' /} {Üye name='ar-ge'/}{TestTag name='test'/}{/Üyeler}" +
                "{/cw}");
            evulator.Parse();
            //Tüm elemanların Üye tagında name özelliği varolan elemanlarını getir.
            var üyeler = evulator.Elements.FindByXPath("//Üye[@name]");
            string üyelerçıktısı = "";
            foreach (var üye in üyeler)
            {
                //true ayarlandığı zaman çıktı biçimine göre ayarlar(tab ve satırları)
                üyelerçıktısı += üye.Outer(true) + "\r\n";
            }
            MessageBox.Show(üyelerçıktısı);
        }
        class IBEvulator : BaseEvulator
        {
            public override TextEvulateResult Render(TextElement tag, object vars)
            {
                TextEvulateResult result = new TextEvulateResult();
                result.TextContent = $"<{tag.ElemName.ToLowerInvariant()}>" + tag.Inner() + $"</{tag.ElemName.ToLowerInvariant()}>";
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                return result;
            }
        }
        class UrlEvulator : BaseEvulator
        {
            public override TextEvulateResult Render(TextElement tag, object vars)
            {
                TextEvulateResult result = new TextEvulateResult();
                result.TextContent = $"<a href=\"{tag.TagAttrib}\">" + tag.InnerText() + "</a>";
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                return result;
            }
        }
        private void TextEngineTest4()
        {
            TextEvulator evulator = new TextEvulator("[B]Bold[/B][I]Italic[/I][URL=www.cyber-warrior.org]CW[/URL]");
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.Parse();
            evulator.EvulatorTypes["b"] = typeof(IBEvulator);
            evulator.EvulatorTypes["i"] = typeof(IBEvulator);
            evulator.EvulatorTypes["url"] = typeof(UrlEvulator);
            //Kapatılan tag açılmamışsa hata vermeyecek.
            evulator.ThrowExceptionIFPrevIsNull = false;
            string result = evulator.Elements.EvulateValue().TextContent;
            MessageBox.Show(result);
           
        }
        private void TextEngineTest5()
        {
            TextEvulator evulator = new TextEvulator("[FOR var='i' start='1' to='5' step='1']mevcut döngü [%i]\r\n[/for]");
            evulator.LeftTag = '[';
            evulator.RightTag = ']';
            evulator.Parse();
            //Kapatılan tag açılmamışsa hata vermeyecek.
            evulator.ThrowExceptionIFPrevIsNull = false;
            string result = evulator.Elements.EvulateValue().TextContent;
            MessageBox.Show(result);

        }
        private void TextEngineTest6()
        {
            var te = new TextEvulator("[SET name='numeric' value='5'][B]Deneme: [%numeric][/B][SET name='numeric' value='55']DenemeTest: [%numeric][if numeric==55]Değer 55[/if]");
            te = new TextEvulator("[if numeric==55]Değer 55[/if]");
            te.LeftTag = '[';
            te.RightTag = ']';
            te.Parse();
            var r = te.Elements.EvulateValue();
        }
        public class CustomEvulator : BaseEvulator
        {
            public override TextEvulateResult Render(TextElement tag, object vars)
            {
                TextEvulateResult result = new TextEvulateResult();
                result.TextContent = "Custom*: " + tag.Inner();
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                return result;
            }
        }
        private void CustomEvulator1()
        {
            TextEvulator evulator = new TextEvulator("[custom]Custom Content[/custom]");
            evulator.EvulatorTypes["custom"] = typeof(CustomEvulator);
            //.....
        }
        private void ParTest1()
        {
            ParDecode pd = new ParDecode("5 + ((3 * 6 + 2 + 2 * 6) * 2 + 5");
            //Öncelikle ayrıştırma işlemi yapılır.
            pd.Decode();
            //Ayrıştırlan itemler hesaplanır.
            var result = pd.Items.Compute();
            
        }
        public class ParDeneme
        {
            public int Random(int min, int max)
            {
                return new Random().Next(min, max);
            }
            public int Random()
            {
                return new Random().Next();
            }
        }
        private void ParTest2()
        {
            ParDecode pd = new ParDecode("Random(5, 10) + Random()");
            //Öncelikle ayrıştırma işlemi yapılır.
            pd.Decode();
            //Ayrıştırlan itemler hesaplanır.
            var result = pd.Items.Compute(new ParDeneme());
           


        }
        private void TextEngineTest7()
        {
            TextEvulator evulator = new TextEvulator();
            evulator.Text = "{set name='boolvalue' value='true'}{%(boolvalue) ? 'Değer True' : 'Değer False'}\r\n" +
                "{unset name='boolvalue'}{%(boolvalue) ? 'Değer True' : 'Değer False'}";
            //Parametrelerin attirbute alamayacağınıbelirttik aksi halde yukarı kod syntax hatası verecektir.
            evulator.ParamNoAttrib = true;
            evulator.Parse();
            var result = evulator.Elements.EvulateValue();
            MessageBox.Show(result.TextContent);
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
