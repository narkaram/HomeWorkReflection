using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Reflection
{
    /// <summary> Serializer </summary>
    public static class Serializer
    {
        /// <summary> Serialize from object to CSV </summary>
        /// <param name="obj">any object</param>
        /// <returns>CSV</returns>
        private static char CsvSeparator = ';';

        public static string SerializeFromObjectToCSV(object obj)
        {
            Type type = obj.GetType();
            var fields = type.GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            //сбор названий
            var res = getNames(fields);

            //сбор значений
            var res2 = getValues(fields, obj);

            return res + '\n' + res2;
        }

        private static string getNames(FieldInfo[] fields)
        {
            string result = "";
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Namespace != "System")
                    return result += getNames(field.FieldType.GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
                else {
                    result += field.Name + CsvSeparator;
                }
            }
            return result.TrimEnd(CsvSeparator);
        }

        private static string getValues(FieldInfo[] fields, object obj)
        {
            string result = "";
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Namespace != "System")
                    return result += getValues(field.FieldType.GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), field.GetValue(obj));
                else
                {
                    if (field.FieldType.Name == "Object")
                        result += field.GetValue(obj).GetType() + ":";
                    result += field.GetValue(obj).ToString() + CsvSeparator;
                }
            }
            return result.TrimEnd(CsvSeparator);
        }

        /// <summary> Deserialize from CSV to object</summary>
        /// <param name="csv">string in CSV format</param>
        /// <returns>object</returns>
        public static object DeserializeFromCSVToObject(string csv, Type typeObj)
        {

            var obj = Activator.CreateInstance(type: typeObj);

            List<string> names = new List<string>(csv.Split('\n')[0].Split(CsvSeparator));
            List<string> values = new List<string>(csv.Split('\n')[1].Split(CsvSeparator));
            var fields = typeObj.GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            setValues(names, values, fields, obj);

            return obj;
        }

        private static void setValues(List<string> names, List<string> values, FieldInfo[] fields, object obj)
        {
            for (int i = 0; i < names.Count; i++)
            {
                var field = fields[i];
                if (field.FieldType.Namespace != "System")
                {
                    var objNest = Activator.CreateInstance(type: field.FieldType);
                    field.SetValue(obj, objNest);
                    setValues(names, values, field.FieldType.GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), objNest);
                }
                else
                {
                    var nameInd = names.FindIndex(item => item == field.Name);
                    if (nameInd != -1)
                    {
                        string value = values[nameInd];
                        var fieldName = obj.GetType().GetField(field.Name, bindingAttr: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        if (field.FieldType.Name == "Object")
                        {
                            var val_type = value.Split(':');
                            SetValue(field, val_type[0], obj, val_type[1]);

                        }
                        else SetValue(field, field.FieldType.Name, obj, value);
                        names.RemoveAt(nameInd);
                        values.RemoveAt(nameInd);
                    }
                }
            }
        }

        private static void SetValue(FieldInfo field, string type, object obj, string value)
        {
            switch (type.ToLower())
            {
                case "int32":
                case "int64":
                    field.SetValue(obj, int.Parse(value));
                    break;
                case "single":
                case "float":
                    field.SetValue(obj, float.Parse(value));
                    break;
                case "double":
                    field.SetValue(obj, double.Parse(value));
                    break;
                case "decimal":
                    field.SetValue(obj, decimal.Parse(value));
                    break;
                case "datetime":
                    field.SetValue(obj, DateTime.Parse(value));
                    break;
                case "byte":
                    field.SetValue(obj, byte.Parse(value));
                    break; 
                case "sbyte":
                    field.SetValue(obj, sbyte.Parse(value));
                    break; 
                case "short":
                    field.SetValue(obj, short.Parse(value));
                    break; 
                case "ushort":
                    field.SetValue(obj, ushort.Parse(value));
                    break; 
                case "uint":
                    field.SetValue(obj, uint.Parse(value));
                    break; 
                case "long":
                    field.SetValue(obj, long.Parse(value));
                    break; 
                case "ulong":
                    field.SetValue(obj, ulong.Parse(value));
                    break;
                case "char":
                    field.SetValue(obj, char.Parse(value));
                    break;
                case "bool":
                    field.SetValue(obj, bool.Parse(value));
                    break;
                case "string":
                case "default":
                    field.SetValue(obj, value);
                    break;
            }
        }
    }
    public class TestClass
    {
        private int a;
        private object obj;

        public int A { get => a; }
        public object Obj { get => obj; }

        public TestClass()
        {
            a = 30;
            obj = (float)12.1;
        }
    }
    public class TestClass2
    {
        private int a;
        private TestClass tc;
        public int A { get => a; }
        public TestClass TC { get => tc; }
        public TestClass2()
        {
            a = 25;
            tc = new TestClass();
        }
    }
}
