﻿using System;
using System.Linq;
using CS.Utils.IO.Extentions;
using NUnit.Framework;

namespace CS.Utils.IOTests.Extentions
{
	[TestFixture]
	internal class LinqExtentionTests
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(100)]
		public void TestRepeat(int times)
		{
			Assert.AreEqual(times, (-1).Repeat(times).Count());
			Assert.AreEqual(times, (0).Repeat(times).Count());
			Assert.AreEqual(times, (1).Repeat(times).Count());
			Assert.AreEqual(times, (new object()).Repeat(times).Count());
			Assert.AreEqual(times, (string.Empty).Repeat(times).Count());
			Assert.AreEqual(times, ("test").Repeat(times).Count());
			Assert.AreEqual(times, (Guid.Empty).Repeat(times).Count());
			Assert.AreEqual(times, (Guid.NewGuid()).Repeat(times).Count());
		}
	}
}
