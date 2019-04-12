using System;
using System.Linq;

namespace CS.Utils.Crypto
{
	public class HashResult : IEquatable<HashResult>, IComparable<HashResult>, IComparable
	{
		#region Operators
		public static bool operator ==(HashResult a, HashResult b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(HashResult a, HashResult b)
		{
			return !a.Equals(b);
		}
		public static explicit operator byte[] (HashResult hashResult)
		{
			return hashResult.HashBytes;
		}
		public static explicit operator string(HashResult hashResult)
		{
			return hashResult.HashString;
		}
		public static implicit operator HashResult(byte[] hashBytes)
		{
			return new HashResult(hashBytes);
		}
		#endregion


		private string _hashString;

		public byte[] HashBytes { get; }
		public string HashString
		{
			get
			{
				if (_hashString != null)
				{
					return _hashString;
				}

				return _hashString = string.Concat(HashBytes.Select(b => b.ToString("X2")));
			}
		}

		public HashResult(byte[] hashBytes)
		{
			HashBytes = hashBytes;
		}

		public override string ToString()
		{
			return HashString;
		}

		#region Operators Methods
		public bool Equals(HashResult other)
		{
			return HashBytes.SequenceEqual(other.HashBytes);
		}
		public override bool Equals(object obj)
		{
			return obj is HashResult ? Equals((HashResult)obj) : false;
		}
		public int CompareTo(HashResult other)
		{
			if (HashBytes.Length != other.HashBytes.Length)
			{
				return HashBytes.Length.CompareTo(other.HashBytes.Length);
			}

			for (int i = 0; i < HashBytes.Length; i++)
			{
				if (HashBytes[i] != other.HashBytes[i])
				{
					return HashBytes[i].CompareTo(other.HashBytes[i]);
				}
			}

			return 0;
		}
		public int CompareTo(object obj)
		{
			return obj is HashResult ? CompareTo((HashResult)obj) : 1;
		}

		public override int GetHashCode()
		{
			return 175587528 + HashBytes.GetHashCode();
		}
		#endregion
	}
}
