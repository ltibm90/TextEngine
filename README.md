## Nuget 

**Installation**

Install-Package TextEngine

Install-Package TextEngine.x86

[Text Engine](https://www.nuget.org/packages/TextEngine)

[Text Engine x86](https://www.nuget.org/packages/TextEngine.x86)


## Template Engine Usage

```csharp
            TextEvulator evulator = new TextEvulator();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["is_loaded"] = true;
            data["str_data"] = "string data";
            data["int_data"] = 12345;
            //User can change Left and Right tag.
            //evulator.LeftTag = '[';
            //evulator.RightTag = ']';
            evulator.GloblaParameters = data;
            evulator.Text = "{if is_loaded}Loaded{/if} string data: {%str_data}, int data: {%int_data}";
            //Parse content.
            evulator.Parse();
            //Evulate all.
            var result = evulator.Elements.EvulateValue();
            //Output: Loaded string data: string data, int data: 12345
            string resultStr = result?.TextContent;
```


## ParFormat Usage
```csharp
            //Long usage
            ParFormat pf = new ParFormat();
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["name"] = "MacMillan";
            kv["grup"] = "AR-GE";
            kv["random"] = (Func<int>)delegate () {
                return new Random().Next(1, 100);
            };
            pf.SurpressError = true;
            pf.Text = "User: {%name}, Group: {%grup}, Random Number: {%random()}";

            //Short usage
            ParFormat.Format("User: {%name}, Group: {%grup}, Number Sayı: {%random()}", kv);


            //Output  User: MacMillan, Group: AR-GE, Random Number: 61
            string res = pf.Apply(kv);
```

# Evulators

## NoPrintEvulator
```csharp
            TextEvulator evulator = new TextEvulator();
            //NoPrint inner is evulated bu not print to result.
			evulator.Text = "{NOPRINT}.............{/NOPRINT}";
```

## CM(Call Macro)Evulator and MacroEvulator
```csharp
            TextEvulator evulator = new TextEvulator();
            //evulator.Text = "{noprint}{macro name=macroname}{/macro}{noprint}{cm macroname}";
            evulator.ParamNoAttrib = true;
            evulator.Text = "{noprint}{macro name=macro1}This is macro line, param1: {%param1}, param2: {%param2}\r\n{/macro}{/noprint} {cm macro1}{cm macro1 param1=\"'test'\" param2=123456}";
            evulator.Parse();
            //Output: This is macro line, param1: , param2: \r\nThis is macro line, param1: test, param2: 123456\r\n
            string result = evulator.Elements.EvulateValue()?.TextContent;
```


## ForEvulator, ContinueEvulator And BreakEvulator Usage
```csharp
            TextEvulator evulator = new TextEvulator();
            //evulator.Text = "{FOR var=i start=0 step=1 to=5}Current Step: {%i}{/FOR}";
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["name"] = "TextEngine";
            evulator.GloblaParameters = kv;
            evulator.Text = "{FOR var=i start=0 step=1 to='name.Length'}{%name[i]}{if i == 4}{continue}{/if}{if i==7}{break}{/if}-{/FOR}";
            evulator.ParamNoAttrib = true;
            evulator.Parse();
            //Output: "T-e-x-t-En-g-i"
            string result = evulator.Elements.EvulateValue()?.TextContent;
```

## ForeachEvulator Usage
```csharp
            TextEvulator evulator = new TextEvulator();
            //evulator.Text = "{FOREACH var=item in=list}{/FOR}";
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["list"] = new List<string> { "item1", "item2", "item3" };
            evulator.GloblaParameters = kv;
            evulator.Text = "{FOREACH var=current in=list}{%current}\r\n{/FOREACH}";
            evulator.ParamNoAttrib = true;
            evulator.Parse();
            //Output: item1\r\nitem2\r\nitem3\r\n
            string result = evulator.Elements.EvulateValue()?.TextContent;
```
## IfEvulator Usage
```csharp
			TextEvulator evulator = new TextEvulator();
            //evulator.Text = "{IF statement}true{elif statement}elif true{/elif}{else}false{/IF}";
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["status"] = 3;
            evulator.GloblaParameters = kv;
            evulator.ParamNoAttrib = true;
            evulator.Text = "{IF status==1}status = 1{ELIF status == 2}status = 2 {ELSE}status on else, value: {%status}{/IF}";
            evulator.Parse();
            //Output: status on else, value: 3
            string result = evulator.Elements.EvulateValue()?.TextContent;
```

## IncludeEvulator Usage
```csharp
			TextEvulator evulator = new TextEvulator();
            evulator.Text = "{include name=\"'path'\" xpath='optional' parse='true or false(as text)'";
			evulator.Parse();
            string result = evulator.Elements.EvulateValue()?.TextContent;
```

## RepeatEvulator Usage
```csharp
            TextEvulator evulator = new TextEvulator();
            evulator.Text = "{repeat current_repeat='cur' count=2}Current Repat: {%cur}{if cur == 0}-{/if}{/repeat}";
            evulator.Parse();
            //Output: "Current Repat: 0-Current Repat: 1"
            string result = evulator.Elements.EvulateValue()?.TextContent;
```

## ReturnEvulator Usage
```csharp
            TextEvulator evulator = new TextEvulator();
            evulator.Text = "Test variable, test variable 2 {if !test}{return}{/if} test variable 3";
            evulator.Parse();
            //Output: Test variable, test variable 2 
            string result = evulator.Elements.EvulateValue()?.TextContent;
```


## SetEvulator And UnsetEvulator
```csharp
            TextEvulator evulator = new TextEvulator();
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["status"] = true;
            kv["total"] = 5;
            evulator.GloblaParameters = kv;
            evulator.Text = "{set if=status name=variable value='total * 2'} variable is: {%variable}, {unset name=variable}\r\nvariable is: {%variable}";
            evulator.ParamNoAttrib = true;
            evulator.Parse();
            //Output: " variable is: 10, \r\nvariable is: "
            string result = evulator.Elements.EvulateValue()?.TextContent;
```


## SwitchEvulator
```csharp
            TextEvulator evulator = new TextEvulator();
            Dictionary<string, object> kv = new Dictionary<string, object>();
            kv["total"] = 2;
            evulator.GloblaParameters = kv;
            evulator.Text = @"{switch c=total}
                                {case v=1}Value 1{/case}
                                {case v=2}Value 2{/case}
                                {default}Default Value{/default}
                                {/switch}";
            evulator.ParamNoAttrib = true;
            evulator.Parse();
            //Output: Value 2
            string result = evulator.Elements.EvulateValue()?.TextContent;
```
