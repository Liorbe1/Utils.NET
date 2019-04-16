using System.IO;
using System.Threading.Tasks;
using CS.Utils.IO.Extentions;
using CS.Utils.IO.Streams;
using NUnit.Framework;
using Types.BytesSize;

namespace CS.Utils.IOTests.Streams
{
	[TestFixture]
	internal class LimitedStreamTests
	{
		private readonly FileInfo readFileInfo = new FileInfo(typeof(LimitedStreamTests).Assembly.Location);

		[Test]
		public void TestRead()
		{
			Size limitedSize = new Size(2);
			using (FileStream fileStream = readFileInfo.OpenRead())
			using (LimitedStream limitedStream = new LimitedStream(fileStream, limitedSize))
			{
				Assert.AreNotEqual(limitedSize.Bytes, fileStream.Length);
				Assert.AreEqual(limitedSize.Bytes, limitedStream.ReadToEnd().LongLength);
				Assert.AreEqual(0, limitedStream.ReadToEnd().LongLength);
			}
		}
		[Test]
		public void TestReadByte()
		{
			Size limitedSize = new Size(2);
			using (FileStream fileStream = readFileInfo.OpenRead())
			using (LimitedStream limitedStream = new LimitedStream(fileStream, limitedSize))
			{
				for (long i = 0; i < limitedSize.Bytes; i++)
				{
					Assert.AreNotEqual(-1, limitedStream.ReadByte());
				}

				Assert.AreEqual(-1, limitedStream.ReadByte());
				Assert.AreEqual(-1, limitedStream.ReadByte());
			}
		}
		[Test]
		public async Task TestReadAsync()
		{
			Size limitedSize = new Size(2);
			using (FileStream fileStream = readFileInfo.OpenRead())
			using (LimitedStream limitedStream = new LimitedStream(fileStream, limitedSize))
			{
				Assert.AreNotEqual(limitedSize.Bytes, fileStream.Length);
				Assert.AreEqual(limitedSize.Bytes, (await limitedStream.ReadToEndAsync()).LongLength);
				Assert.AreEqual(0, (await limitedStream.ReadToEndAsync()).LongLength);
			}
		}
	}
}
