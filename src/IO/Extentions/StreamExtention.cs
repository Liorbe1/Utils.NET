﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Types.BytesSize;

namespace CS.Utils.IO.Extentions
{
	public static class StreamExtention
	{
		public static byte[] ReadToEnd(this Stream stream, Size? estimatedSize = null)
		{
			try
			{
				estimatedSize = estimatedSize ?? stream.Length;
			}
			catch { }

			byte[] content;
			using (MemoryStream memoryStream = new MemoryStream((int)(estimatedSize?.Bytes ?? 0)))
			{
				//TODO: limit the max file size
				stream.CopyTo(memoryStream);
				if (memoryStream.Length == memoryStream.Capacity)
				{
					content = memoryStream.GetBuffer();
				}
				else
				{
					content = memoryStream.ToArray();
				}
			}

			return content;
		}
		public static async Task<byte[]> ReadToEndAsync(this Stream stream, Size? estimatedSize = null)
		{
			try
			{
				estimatedSize = estimatedSize ?? stream.Length;
			}
			catch { }

			byte[] content;
			using (MemoryStream memoryStream = new MemoryStream((int)(estimatedSize?.Bytes ?? 0)))
			{
				//TODO: limit the max file size
				await stream.CopyToAsync(memoryStream);
				if (memoryStream.Length == memoryStream.Capacity)
				{
					content = memoryStream.GetBuffer();
				}
				else
				{
					content = memoryStream.ToArray();
				}
			}

			return content;
		}
		public static byte[] ReadMax(this Stream stream, Size limit, Size? estimatedSize = null)
		{
			using (LimitedStream limitedStream = new LimitedStream(stream, limit))
			{
				return limitedStream.ReadToEnd(estimatedSize);
			}
		}
		public static async Task<byte[]> ReadMaxAsync(this Stream stream, Size limit, Size? estimatedSize = null)
		{
			using (LimitedStream limitedStream = new LimitedStream(stream, limit))
			{
				return await limitedStream.ReadToEndAsync(estimatedSize);
			}
		}
	}
}