using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace CS.Utils.Crypto.Streams
{
	public class MultipleHashStream : Stream
	{
		public override bool CanRead => baseStream.CanRead;
		public override bool CanSeek => false;
		public override bool CanWrite => baseStream.CanWrite;
		public override long Length => baseStream.Length;
		public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
		public bool HasFlushedFinalBlock
		{
			get
			{
				return readHashCalculators.All(hashCalculator => hashCalculator.HasFlushedFinalBlock) &&
					writeHashCalculators.All(hashCalculator => hashCalculator.HasFlushedFinalBlock);
			}
		}

		private readonly bool leaveOpen;
		private readonly Stream baseStream;
		private readonly List<HashStream> readHashCalculators = new List<HashStream>();
		private readonly List<HashStream> writeHashCalculators = new List<HashStream>();

		public MultipleHashStream(Stream baseStream, IEnumerable<HashAlgorithmName> hashAlgorithms, bool leaveOpen = false)
		{
			this.baseStream = baseStream;
			this.leaveOpen = leaveOpen;

			foreach (HashAlgorithmName hashAlgorithmName in hashAlgorithms)
			{
				readHashCalculators.Add(new HashStream(hashAlgorithmName));
				writeHashCalculators.Add(new HashStream(hashAlgorithmName));
			}
		}

		public override void Flush() => baseStream.Flush();
		public override void SetLength(long value) => baseStream.SetLength(value);
		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

		public void FlushFinalBlock()
		{
			if (readHashCalculators != null)
			{
				foreach (HashStream hashCalculator in readHashCalculators)
				{
					hashCalculator.FlushFinalBlock();
				}
			}
			if (writeHashCalculators != null)
			{
				foreach (HashStream hashCalculator in writeHashCalculators)
				{
					hashCalculator.FlushFinalBlock();
				}
			}
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return baseStream.FlushAsync(cancellationToken);
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadAsync(buffer, offset, count).Result;
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			WriteAsync(buffer, offset, count).Wait();
		}
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			int readedBytes = await baseStream.ReadAsync(buffer, offset, count, cancellationToken);

			if (readHashCalculators != null && readedBytes > 0)
			{
				await Task.WhenAll(readHashCalculators.Select(hashCalculator => hashCalculator.WriteAsync(buffer, offset, readedBytes)).ToList());
			}

			return readedBytes;
		}
		public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Task baseStreamWriteTask = baseStream.WriteAsync(buffer, offset, count, cancellationToken);
			await baseStreamWriteTask;

			if (writeHashCalculators != null && !baseStreamWriteTask.IsCanceled)
			{
				await Task.WhenAll(writeHashCalculators.Select(hashCalculator => hashCalculator.WriteAsync(buffer, offset, count)).ToList());
			}
		}
	}
}
