using System.IO;
using System.Security.Cryptography;

namespace CS.Utils.Crypto.Streams
{
	public class HashStream : CryptoStream
	{
		public byte[] HashResult => hashCalculator.Hash;

		private readonly HashAlgorithm hashCalculator;

		public HashStream(Stream stream, HashAlgorithm hashCalculator, CryptoStreamMode mode) : base(stream, hashCalculator, mode)
		{
			this.hashCalculator = hashCalculator;
		}
		public HashStream(Stream stream, HashAlgorithmName hashAlgorithmName, CryptoStreamMode mode) : this(stream, HashAlgorithm.Create(hashAlgorithmName.Name), mode) { }
		public HashStream(HashAlgorithm hashCalculator) : this(Stream.Null, hashCalculator, CryptoStreamMode.Write) { }
		public HashStream(HashAlgorithmName hashAlgorithmName) : this(Stream.Null, hashAlgorithmName, CryptoStreamMode.Write) { }
	}
}
