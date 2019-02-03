using CS.Utils.Crypto.Streams;
using NUnit.Framework;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CryptoTests.Streams
{
	[TestFixture("MD5")]
	[TestFixture("SHA1")]
	[TestFixture("SHA256")]
	[TestFixture("SHA384")]
	[TestFixture("SHA512")]
	public class HashStreamTests
	{
		public HashAlgorithmName HashAlgorithmName { get; }

		public HashStreamTests(string hashAlgorithmName)
		{
			HashAlgorithmName = new HashAlgorithmName(hashAlgorithmName);
		}

		[Test]
		public void TestHashCalculation()
		{
			FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
			byte[] hash;
			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashAlgorithm hashCalculator = HashAlgorithm.Create(HashAlgorithmName.Name))
			{
				hash = hashCalculator.ComputeHash(fileStream);
			}

			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashStream hashStream = new HashStream(fileStream, HashAlgorithmName, CryptoStreamMode.Read))
			{
				hashStream.CopyTo(Stream.Null);
				Assert.AreEqual(hash, hashStream.HashResult);
			}

			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashStream hashStream = new HashStream(Stream.Null, HashAlgorithmName, CryptoStreamMode.Write))
			{
				fileStream.CopyTo(hashStream);
				hashStream.FlushFinalBlock();
				Assert.AreEqual(hash, hashStream.HashResult);
			}

			return;
		}
		[Test]
		public async Task TestAsyncHashCalculation()
		{
			FileInfo fileInfo = new FileInfo(GetType().Assembly.Location);
			byte[] hash;
			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashAlgorithm hashCalculator = HashAlgorithm.Create(HashAlgorithmName.Name))
			{
				hash = hashCalculator.ComputeHash(fileStream);
			}

			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashStream hashStream = new HashStream(fileStream, HashAlgorithmName, CryptoStreamMode.Read))
			{
				await hashStream.CopyToAsync(Stream.Null);
				Assert.AreEqual(hash, hashStream.HashResult);
			}

			using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using (HashStream hashStream = new HashStream(Stream.Null, HashAlgorithmName, CryptoStreamMode.Write))
			{
				await fileStream.CopyToAsync(hashStream);
				hashStream.FlushFinalBlock();
				Assert.AreEqual(hash, hashStream.HashResult);
			}

			return;
		}
	}
}
