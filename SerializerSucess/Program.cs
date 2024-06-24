using System;
using System.IO;
using System.Reflection;

namespace SerializerSucess;

public class Program
{
    static void Main(string[] args)
    {
        // Need this for handing when the serializer doesn't load
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MyClass));
        var myObject = new MyClass { Value1 = 1, Value2 = 2 };

        string serializedOutput;

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(stream, myObject);
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream))
            {
                serializedOutput = reader.ReadToEnd();
            }
        }
        
        Console.WriteLine(serializedOutput);
    }

    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        if (args.Name.StartsWith("SerializerSucess.XmlSerializers,"))
        {
            return Assembly.LoadFrom("SerializerSucess.XmlSerializers.dll");
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}

public class MyClass
{
    public int Value1 { get; set; }
    public int Value2 { get; set; }
}
