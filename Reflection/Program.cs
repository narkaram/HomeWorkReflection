using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Reflection
{
    class Sample
    {
        private int field = 42;

        public int Field { get => field; }
    }
    [Serializable]
    class F { 
        int i1, i2, i3, i4, i5;

        public F()
        {
            i1 = 1;
            i2 = 2;
            i3 = 3;
            i4 = 4;
            i5 = 5;
        }
        public int I1 { get => i1; }
        public int I2 { get => i2; }
        public int I3 { get => i3; }
        public int I4 { get => i4; }
        public int I5 { get => i5; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //FieldMagic();
            //PropertiesMagic();
            //DynamicAssembly();
            //DynamicSample();
            F fOriginal = new F();

            string res1 = "";
            F f1;
            string res2 = "";
            F f2;


            var date1 = DateTime.Now;
            var date1OnlySer = DateTime.Now;

            for (int i = 0; i < 1000; i++)
                res1 = Serializer.SerializeFromObjectToCSV(fOriginal);

            var date1OnlySerResult = DateTime.Now - date1OnlySer;
            var date1OnlyDeSer = DateTime.Now;

            for (int i = 0; i < 1000; i++)
                f1 = (F)Serializer.DeserializeFromCSVToObject(res1, typeof(F));

            var date1OnlyDeSerResult = DateTime.Now - date1OnlyDeSer;
            var date1Result = DateTime.Now - date1;


            JsonSerializer serializer = new JsonSerializer();

            var date2 = DateTime.Now;
            var date2OnlySer = DateTime.Now;
            for (int i = 0; i < 1000; i++)
                 res2 = JsonConvert.SerializeObject(fOriginal);

            var date2OnlySerResult = DateTime.Now - date2OnlySer;
            var date2OnlyDeSer = DateTime.Now;

            for (int i = 0; i < 1000; i++)
                f2 = JsonConvert.DeserializeObject<F>(res2);

            var date2OnlyDeSerResult = DateTime.Now - date2OnlyDeSer;
            var date1Result2 = DateTime.Now - date2;

            var str = "Test my Serializator All - " + date1Result.ToString() + "\nTest Newtonsoft Serializator All- " + date1Result2.ToString();
            str += "\n\nTest my Only Serialization - " + date1OnlySerResult.ToString() + "\nTest Newtonsoft Only Serialization - " + date2OnlySerResult.ToString();
            str += "\n\nTest my Only DeSerialization - " + date1OnlyDeSerResult.ToString() + "\nTest Newtonsoft Only DeSerialization - " + date2OnlyDeSerResult.ToString();
            Console.WriteLine(str);

        }

        static void FieldMagic()
        {
            Sample sample = new Sample();

            Type sampleType = sample.GetType();
            FieldInfo fieldInfo = sampleType
                .GetField(name: "field", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

            //get value
            object fieldValue = fieldInfo.GetValue(obj: sample);
            Console.WriteLine(value: fieldValue);

            //set value
            fieldInfo.SetValue(obj: sample, value: 1);
            int sampleField = sample.Field;
            Console.WriteLine(value: sampleField);
        }

        static void PropertiesMagic()
        {
            Type type = typeof(string);
            PropertyInfo[] props = type.GetProperties();
            Console.WriteLine(value: "Properties:");
            foreach (PropertyInfo prop in props)
            {
                Console.WriteLine(value: prop.Name);
            }
            Console.WriteLine();

            FieldInfo[] fieldInfos = type.GetFields();
            Console.WriteLine(value: "Fields:");
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Console.WriteLine(value: fieldInfo.Name);
            }
            Console.WriteLine();

            MethodInfo[] methodInfos = type.GetMethods();
            Console.WriteLine(value: "Methods:");
            foreach (MethodInfo methodInfo in methodInfos)
            {
                Console.WriteLine(value: methodInfo.Name);
            }

            //https://stackoverflow.com/questions/41468722/loop-reflect-through-all-properties-in-all-ef-models-to-set-column-type

            //https://stackoverflow.com/questions/19792295/mapping-composite-keys-using-ef-code-first
        }

        static void DynamicAssembly()
        {
            string solutionRoot = Directory.GetParent(path: Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string assemblyFile = Path.Combine(solutionRoot, @"MyLibrary\bin\Debug\netcoreapp3.1\MyLibrary.dll");
            if (!File.Exists(assemblyFile))
            {
                throw new FileNotFoundException("please build MyLibrary project");
            }

            Assembly assembly = Assembly.LoadFrom(assemblyFile: assemblyFile);
            Console.WriteLine(value: assembly.FullName);
            // получаем все типы из сборки MyLibrary.dll
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                Console.WriteLine(value: type.Name);
            }

            // Позднее связывание
            Type myLibraryClassType = assembly.GetType(name: "MyLibrary.MyLibraryClass", throwOnError: true, ignoreCase: true);
            
            // создаем экземпляр класса
            object myLibraryClass = Activator.CreateInstance(type: myLibraryClassType);

            MethodInfo sumMethodInfo = myLibraryClassType.GetMethod(name: "Sum");

            object result = sumMethodInfo.Invoke(obj: myLibraryClass, parameters: new object[] { 2, 6 });

            Console.WriteLine(value: result);
        }

        static void DynamicSample()
        {
            dynamic sample1 = new Sample();
            var sample2 = new Sample();
            object sample3 = new Sample();
            Sample sample4 = new Sample();

            Console.WriteLine(sample1.Field);
            //Console.WriteLine(sample1.Method());
            Console.WriteLine();

            Console.WriteLine(value: "dynamic: " + sample1.GetType().Name); 
            Console.WriteLine(value: "var: " + sample2.GetType().Name);
            Console.WriteLine(value: "object: " + sample3.GetType().Name); 
            Console.WriteLine(value: "Sample: " + sample4.GetType().Name);
            Console.WriteLine();

            dynamic expando = new ExpandoObject();
            Console.WriteLine(value: "expando: " + expando.GetType().Name);

            expando.Name = "Brian";
            expando.Country = "USA";
            expando.City = new object();

            expando.IsValid = (Func<int, bool>)((number) =>
            {
                // Check that they supplied a name
                return !string.IsNullOrWhiteSpace(value: expando.Name);
            });

            expando.Print = (Action)(() =>
            {
                Console.WriteLine(value: $"{expando.Name} {expando.Country} {expando.IsValid(456456)}");
            });

            expando.Print();
            expando.Name = "Jack";
            expando.Print();
        }
    }
}