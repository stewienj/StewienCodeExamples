using System;
using System.Linq;
using System.Diagnostics;

namespace MinMaxSpeedTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();

			Console.WriteLine("Calculating Array");
			sw.Start();
			var array = CalculateArray();
			sw.Stop();
			Console.WriteLine(sw.Elapsed);

			Console.WriteLine("Testing Without Branching");
			sw.Reset();
			sw.Start();
			var minMaxWB = MinMaxWithoutBranching(array);
			sw.Stop();
			Console.WriteLine($"Min: {minMaxWB.Item1} Max: {minMaxWB.Item2} Time: {sw.Elapsed}");

			Console.WriteLine("Testing Ternary");
			sw.Reset();
			sw.Start();
			var minMaxT = MinMaxTernary(array);
			sw.Stop();
			Console.WriteLine($"Min: {minMaxT.Item1} Max: {minMaxT.Item2} Time: {sw.Elapsed}");

			Console.WriteLine("Testing Linq");
			sw.Reset();
			sw.Start();
			var minMaxL = (array.Min(), array.Max());
			sw.Stop();
			Console.WriteLine($"Min: {minMaxL.Item1} Max: {minMaxL.Item2} Time: {sw.Elapsed}");

		}

		private static int[] CalculateArray()
		{
			Random rand = new Random(1000);
			int[] array = new int[2_000_000];
			for (int i = 0; i < array.Length; ++i)
			{
				array[i] = rand.Next(-100_000_000, 100_000_000);
			}
			return array;
		}

		/// <summary>
		/// This uses a non branching algorithm for calculating min, max from:
		/// https://graphics.stanford.edu/~seander/bithacks.html#IntegerMinOrMax
		/// 
		/// Compute the minimum (min) or maximum (max) of two integers without branching
		/// int x;  // we want to find the minimum of x and y
		/// int y;
		/// int r;  // the result goes here 
		///
		/// r = y ^ ((x ^ y) & -(x<y)); // min(x, y)
		/// On some rare machines where branching is very expensive and no condition move
		/// instructions exist, the above expression might be faster than the obvious approach, 
		/// r = (x<y) ? x : y, even though it involves two more instructions. (Typically, the 
		/// obvious approach is best, though.) It works because if x<y, then -(x<y) will be
		/// all ones, so r = y ^ (x ^ y) & ~0 = y ^ x ^ y = x.Otherwise, if x >= y, then -(x<y)
		/// will be all zeros, so r = y ^ ((x ^ y) & 0) = y.On some machines, evaluating(x<y)
		/// as 0 or 1 requires a branch instruction, so there may be no advantage.
		/// 
		/// To find the maximum, use:
		/// 
		/// r = x ^ ((x ^ y) & -(x<y)); // max(x, y)
		/// 
		/// Quick and dirty versions:
		/// 
		/// If you know that INT_MIN <= x - y <= INT_MAX, then you can use the following,
		/// which are faster because(x - y) only needs to be evaluated once.
		/// 
		/// r = y + ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // min(x, y)
		/// r = x - ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // max(x, y)
		/// 
		/// Note that the 1989 ANSI C specification doesn't specify the result of signed
		/// right-shift, so these aren't portable.If exceptions are thrown on overflows,
		/// then the values of x and y should be unsigned or cast to unsigned for the
		/// subtractions to avoid unnecessarily throwing an exception, however the right-shift
		/// needs a signed operand to produce all one bits when negative, so cast to signed there.
		/// On March 7, 2003, Angus Duggan pointed out the right-shift portability issue.
		/// On May 3, 2005, Randal E. Bryant alerted me to the need for the precondition,
		/// INT_MIN <= x - y <= INT_MAX, and suggested the non-quick and dirty version as
		/// a fix. Both of these issues concern only the quick and dirty version. Nigel 
		/// Horspoon observed on July 6, 2005 that gcc produced the same code on a Pentium
		/// as the obvious solution because of how it evaluates (x<y). On July 9, 2008 Vincent
		/// Lefèvre pointed out the potential for overflow exceptions with subtractions in
		/// r = y + ((x - y) & -(x < y)), which was the previous version.Timothy B. Terriberry
		/// suggested using xor rather than add and subract to avoid casting and the risk
		/// of overflows on June 2, 2009.
		/// </summary>
		/// <returns></returns>
		public static (int, int) MinMaxWithoutBranching(int[] values)
		{
			int min = values[0];
			int max = values[1];

			for (int i = 1; i < values.Length; ++i)
			{
				// Have to use quick and dirty because in C# a bool can't be converted to an int
				int value = values[i];
				int diff = min - value;
				diff = diff & (diff >> 31);
				min = value + diff;
				max = min - diff;
			}
			return (min, max);
		}

		public static (int, int) MinMaxTernary(int[] values)
		{
			int min = values[0];
			int max = values[1];

			for (int i = 1; i < values.Length; ++i)
			{
				int value = values[i];
				min = min <= value ? min : value;
				max = max >= value ? max : value;
			}
			return (min, max);
		}
	}
}
