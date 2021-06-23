using DllLoadFailTestDll;
using System;

namespace DllLoadFailTestExe
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var test = new Class1();
                Console.WriteLine("Works!");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
