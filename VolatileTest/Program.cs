// Compile as Release

using System;
using System.Threading.Tasks;

namespace VolatileTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var test = new Test();
      test.Test1();
      test.Test2();
      Console.WriteLine();
      Console.WriteLine("Press Enter To Exit");
      Console.ReadLine();
    }
  }

  class Test
  {
    int[] X = new[] { 0 };
    int[] Y = new[] { 1 };

    volatile int[] local1;
    int[] local2;

    public Test()
    {
      local1 = X;
      local2 = X;
    }

    public void Test1()
    {
      Task.Run(() => local1 = Y);
      while (local1[0] == 0) ;
      Console.WriteLine("Finished Test 1");
    }

    public void Test2()
    {
      Task.Run(() => local2 = Y);
      while (local2[0] == 0) ;
      Console.WriteLine("Finished Test 2");
    }
  }
}