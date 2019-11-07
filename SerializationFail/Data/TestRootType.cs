using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationFail.Data
{
	public class TestRootType
	{
		private TestDataType[] _testData = null;

		public TestRootType()
		{
			PopulateTestData();
		}

		public void PopulateTestData()
		{
			_testData = new[]
			{
			  "Lisa",
			  "Bart",
			  "Marge",
			  "Homer",
			  "Abe"
			}
			.Select(n => new TestDataType(n))
			.ToArray();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is TestRootType testRoot)
			{
				return
				  testRoot?.TestData != null
					&& testRoot.TestData.Length == _testData.Length
					&& testRoot
						.TestData
						.Zip(_testData, (a, b) => a.Name.Equals(b.Name) && a.SomeColor.Equals(b.SomeColor))
						.Aggregate((a, b) => a && b);

			}
			return false;
		}

		public TestDataType[] TestData
		{
			get => _testData;
			set => _testData = value;
		}
	}
}
