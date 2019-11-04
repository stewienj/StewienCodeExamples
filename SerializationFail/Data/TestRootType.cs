using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationFail.Data
{
	public class TestRootType
	{
		private List<TestDataType> _testData = new List<TestDataType>();

		public TestRootType()
		{

		}

		public void PopulateTestData()
		{
			var names = new[]
			{
		  "Lisa",
		  "Bart",
		  "Marge",
		  "Homer",
		  "Abe"
		};

			_testData.AddRange(names.Select(n => new TestDataType(n)));
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
					&& testRoot.TestData.Count == _testData.Count
					&& testRoot
						.TestData
						.Zip(_testData, (a, b) => a.Name == b.Name && a.SomeColor == b.SomeColor)
						.Aggregate((a, b) => a && b);
			}
			return false;
		}

		public List<TestDataType> TestData
		{
			get => _testData;
			set => _testData = value;
		}
	}
}
