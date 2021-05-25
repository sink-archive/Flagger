using System.Collections.Generic;
using NUnit.Framework;

namespace Flagger.Tests
{
	public class FlagTests
	{
		[SetUp]
		public void Setup() { }

		[Test]
		public void ArrayCreateTest()
		{
			var flagCollection = new FlagCollection
				(new []
				{
					('f', false),
					('h', true),
					('g', true),
					('i', false)
				});
			
			Assert.AreEqual(new Dictionary<char, int>
							{
								{'f', 0},
								{'h', 1},
								{'g', 2},
								{'i', 3}
							},
							flagCollection.Flags);
			
			Assert.AreEqual(new List<byte>
							{
								0b01100000
							},
							flagCollection.FlagValues);
			
			Assert.AreEqual(4, flagCollection.NextFreePosition);
		}

		[Test]
		public void EmptyCreateTest()
		{
			var flagCollection = new FlagCollection();
			Assert.AreEqual(new Dictionary<char, int>(), flagCollection.Flags);
			Assert.AreEqual(new List<byte>(), flagCollection.FlagValues);
			Assert.AreEqual(0, flagCollection.NextFreePosition);
		}

		[Test]
		public void StringParseCreateTest()
		{
			// g should be ignored as it is not in possibleValues!
			// ReSharper disable once StringLiteralTypo
			var flagCollection = new FlagCollection("fhgi", "hgv");
			
			Assert.AreEqual(new Dictionary<char, int>
							{
								{'f', 0},
								{'h', 1},
								{'g', 2},
								{'i', 3}
							},
							flagCollection.Flags);
			
			Assert.AreEqual(new List<byte>
							{
								0b01100000
							},
							flagCollection.FlagValues);
			
			Assert.AreEqual(4, flagCollection.NextFreePosition);
		}

		[Test]
		public void AddTest()
		{
			var flagCollection = new FlagCollection()
			{
				Flags            = {{'h', 0}},
				FlagValues       = {0b10000000},
				NextFreePosition = 1
			};
			
			flagCollection.SetOrAddFlag('f', true);
			flagCollection.SetOrAddFlag('h', false);
			
			Assert.AreEqual(new Dictionary<char, int>
							{
								{'h', 0},
								{'f', 1}
							},
							flagCollection.Flags);

			Assert.AreEqual(new List<byte> { 0b01000000 }, flagCollection.FlagValues);
			
			Assert.AreEqual(2, flagCollection.NextFreePosition);
		}

		[Test]
		public void GetTest()
		{
			var flagCollection = new FlagCollection
			{
				Flags            = {{'h', 0}, {'g', 1}, {'v', 2}},
				FlagValues       = {0b10100000},
				NextFreePosition = 3
			};
			
			Assert.IsTrue(flagCollection.GetFlag('h'));
			Assert.IsTrue(flagCollection.GetFlag('v'));
			Assert.IsFalse(flagCollection.GetFlag('g'));
		}
	}
}