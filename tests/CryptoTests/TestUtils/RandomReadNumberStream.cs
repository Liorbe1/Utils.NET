using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTests.TestUtils
{
	internal class RandomReadNumberStream : Stream
	{
		public override bool CanRead => baseStream.CanRead;
		public override bool CanWrite => baseStream.CanWrite;
		public override bool CanSeek => baseStream.CanSeek;
		public override bool CanTimeout => baseStream.CanTimeout;
		public override long Length => baseStream.Length;
		public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
		public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }
		public override int WriteTimeout { get => baseStream.WriteTimeout; set => baseStream.WriteTimeout = value; }

		private readonly bool leaveOpen;
		private readonly Stream baseStream;
		private readonly Random random = new Random();

		public RandomReadNumberStream(Stream baseStream, bool leaveOpen = false)
		{
			this.baseStream = baseStream;
			this.leaveOpen = leaveOpen;
		}

		public override void Flush() => baseStream.Flush();
		public override Task FlushAsync(CancellationToken cancellationToken) => baseStream.FlushAsync(cancellationToken);
		public override void SetLength(long value) => baseStream.SetLength(value);
		public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

		public override int Read(byte[] buffer, int offset, int count)
		{
			count = random.Next(1, count);
			return baseStream.Read(buffer, offset, count);
		}
		public override int Read(Span<byte> buffer)
		{
			buffer = buffer.Slice(0, random.Next(1, buffer.Length));
			return baseStream.Read(buffer);
		}
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			count = random.Next(1, count);
			return baseStream.ReadAsync(buffer, offset, count, cancellationToken);
		}
		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
		{
			buffer = buffer.Slice(0, random.Next(1, buffer.Length));
			return base.ReadAsync(buffer, cancellationToken);
		}
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			count = random.Next(1, count);
			return baseStream.BeginRead(buffer, offset, count, callback, state);
		}
		public override int EndRead(IAsyncResult asyncResult)
		{
			return baseStream.EndRead(asyncResult);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			baseStream.Write(buffer, offset, count);
		}
	}
}
