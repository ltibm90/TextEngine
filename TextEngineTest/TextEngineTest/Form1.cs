﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            //Uzun kullanımı
            ParFormat pf = new ParFormat();
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["name"] = "MacMillan";
            kv["grup"] = "AR-GE";
            kv["random"] = (Func<int>)delegate () {
                return new Random().Next(1, 100);
            };
            pf.SurpressError = true;
            pf.Text = "Kullanıcı: {%name}, Grup: {%grup}, Random Sayı: {%random()}";

            //Direkt kullanımı
            ParFormat.Format("Kullanıcı: {%name}, Grup: {%grup}, Random Sayı: {%random()}", kv);


            //Sonuç  Kullanıcı: MacMillan, Grup: AR-GE, Random Sayı: 61
            string res = pf.Apply(kv);
        }
        private void GeneralTest()
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
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
            evulator.GloblaParameters = new CustomClass();
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
            evulator.GloblaParameters = new object(); //Evulate işlemi sırasındaki değişkenler bu değişken aracılığı il
            //Aşağıdaki şekillerdek ullanabilirsiniz
            var test = new
            {
                name = "Deneme",
                value = 1000
            };
            evulator.GloblaParameters = test;
            IDictionary<string, object> dict = new Dictionary<string, object>();
            evulator.GloblaParameters = dict;
            KeyValues<object> kv = new KeyValues<object>();
            evulator.GloblaParameters = kv;
            evulator.GloblaParameters = new CustomClass();
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
            evulator.GloblaParameters = dict;
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

            evulator.GloblaParameters = dict;
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
    }
}
