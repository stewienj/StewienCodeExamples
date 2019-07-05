// Compile as Release

using System;
using System.Threading.Tasks;

namespace VolatileTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Starting...");
            var taskArray = Task.Run(() =>
            {
                var test = new TestArray();
                test.Test1();
                test.Test2();
            });
            var taskBool = Task.Run(() =>
            {
                var test = new TestBool();
                test.Test1();
                test.Test2();
            });
            Task.WaitAll(taskArray, taskBool);
            Console.WriteLine();
            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }
    }

    class TestArray
    {
        int[] X = new[] { 0 };
        int[] Y = new[] { 1 };

        volatile int[] local1;
        int[] local2;

        public TestArray()
        {
            local1 = X;
            local2 = X;
        }

        public void Test1()
        {
            Task.Run(() => local1 = Y);
            while (local1[0] == 0) ;
            Console.WriteLine("Finished Test Array 1");
        }

        public void Test2()
        {
            Task.Run(() => local2 = Y);
            while (local2[0] == 0) ;
            Console.WriteLine("Finished Test Array 2");
        }
    }

    class TestBool
    {
        volatile bool local1 = false;
        bool local2 = false;

        public void Test1()
        {
            Task.Run(() => local1 = true);
            while (!local1) ;
            Console.WriteLine("Finished Test Bool 1");
        }

        public void Test2()
        {
            Task.Run(() => local2 = true);
            while (!local2) ;
            Console.WriteLine("Finished Test Bool 2");
        }
    }

}