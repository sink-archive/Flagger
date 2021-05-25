using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

// haha making all internal vars available for testing is a GREAT idea
[assembly: InternalsVisibleTo("Flagger.Tests")]

namespace Flagger
{
	// ReSharper disable once UnusedType.Global
	public class FlagCollection
	{
		// internal not private so that, at the expense of being available when used inside of the Flagger namespace,
		// these values are accessible to any unit tests in the Flagger.Tests namespace.
		internal readonly Dictionary<char, int> Flags     = new();
		internal readonly List<byte>            FlagValues = new();

		internal int NextFreePosition;

		/// <summary>
		/// Creates an empty flag collection
		/// </summary>
		public FlagCollection(){}
		
		/// <summary>
		/// Creates a flag collection from a given array of chars and bools
		/// </summary>
		/// <param name="flags">An array of flags</param>
		public FlagCollection((char, bool)[] flags)
		{
			foreach (var (flagChar, value) in flags) SetOrAddFlag(flagChar, value);
		}

		/// <summary>
		/// Creates a flag collection from a string containing every possible flag and a string of true flags - intended for command line parsers
		/// </summary>
		/// <param name="possibleFlags">Every possible flag</param>
		/// <param name="givenFlags">Flags that are set true - any not in possibleFlags are ignored</param>
		public FlagCollection(string possibleFlags, string givenFlags)
		{
			// Use a hash set for better performance with lots of flags!
			var givenSet = givenFlags.ToImmutableHashSet();
			foreach (var flag in possibleFlags) SetOrAddFlag(flag, givenSet.Contains(flag));
		}

		public bool Contains(char flag) => Flags.ContainsKey(flag);
		public bool GetFlag(char  flag) => GetFlag(Flags[flag]);

		private bool GetFlag(int index)
		{
			var byteIndex = index / 8;
			var bitIndex  = index % 8;
			var b         = FlagValues[byteIndex];
			var bit       = 1 << (7 -bitIndex);
			var masked    = b & (byte) bit;
			return masked != 0;
		}

		private void SetFlag(int index, bool value)
		{
			var byteIndex = index / 8;
			var bitIndex  = index % 8;
			var bit       = 1 << (7 - bitIndex);
			if (value)
				FlagValues[byteIndex] |= (byte) bit;
			else
				FlagValues[byteIndex] &= (byte) ~bit;
		}

		public void SetOrAddFlag(char flag, bool value)
		{
			if (Contains(flag))
			{
				SetFlag(Flags[flag], value);
				return;
			}

			var newI = NextFreePosition;
			Flags[flag] = newI;

			while (newI / 8 >= FlagValues.Count)
				FlagValues.Add(0);

			SetFlag(newI, value);
			
			NextFreePosition++;
		}

		public Dictionary<char, int> GetFlagIndexTable() => Flags;
		public byte[]                GetFlagBytes()      => FlagValues.ToArray();
	}
}