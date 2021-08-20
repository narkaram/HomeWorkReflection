using System;
using System.Reflection;

namespace Reflection
{
    public class MyType
    {
        public MyType()
        {
            Console.WriteLine();
            Console.WriteLine(value: "MyType instantiated!");
        }
    }

    class Test
    {
        public static void Call()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // This call will fail to create an instance of MyType since the
            // assembly resolver is not set
            InstantiateMyTypeFail(domain: currentDomain);

            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

            // This call will succeed in creating an instance of MyType since the
            // assembly resolver is now set.
            InstantiateMyTypeFail(domain: currentDomain);

            // This call will succeed in creating an instance of MyType since the
            // assembly name is valid.
            InstantiateMyTypeSucceed(domain: currentDomain);
        }

        private static void InstantiateMyTypeFail(AppDomain domain)
        {
            // Calling InstantiateMyType will always fail since the assembly info
            // given to CreateInstance is invalid.
            try
            {
                // You must supply a valid fully qualified assembly name here.
                domain.CreateInstance(assemblyName: "Assembly text name, Version, Culture, PublicKeyToken", typeName: "MyType");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(value: e.Message);
            }
        }

        private static void InstantiateMyTypeSucceed(AppDomain domain)
        {
            try
            {
                string asmname = Assembly.GetCallingAssembly().FullName;
                domain.CreateInstance(assemblyName: asmname, typeName: "MyType");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(value: e.Message);
            }
        }

        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Console.WriteLine(value: "Resolving...");
            return typeof(MyType).Assembly;
        }
    }
}
