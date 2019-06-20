using CS.Utils.Async.Lock;
using CS.Utils.AsyncTests.TestUtils;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CS.Utils.AsyncTests.Lock
{
	[TestFixture]
	internal class AsyncLockerTests
	{
		[Test]
		public async Task TestThreadSafeAsync()
		{
			SyncChecker syncChecker = new SyncChecker();

			using (AsyncLocker locker = new AsyncLocker())
			{
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(async () =>
				{
					for (int i = 0; i < 100; i++)
					{
						using (await locker.LockAsync())
						{
							syncChecker.Enter();
							await Task.Delay(TimeSpan.FromSeconds(0.001));
							syncChecker.Leave();
						}
					}
				})).ToArray();

				await Task.WhenAll(tasks);
			}
		}

		[Test]
		public async Task TestThreadSafe()
		{
			ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
			ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);
			Console.WriteLine($"Max\tworker threads: {maxWorkerThreads} io threads: {maxIoThreads}");
			Console.WriteLine($"Min\tworker threads: {minWorkerThreads} io threads: {minIoThreads}");
			ThreadPool.SetMinThreads(minWorkerThreads * 2, minIoThreads);

			SyncChecker syncChecker = new SyncChecker();

			using (AsyncLocker locker = new AsyncLocker())
			{
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(async () =>
				{
					for (int i = 0; i < 100; i++)
					{
						using (locker.Lock())
						{
							syncChecker.Enter();
							await Task.Delay(TimeSpan.FromSeconds(0.001));
							syncChecker.Leave();
						}
					}
				})).ToArray();

				await Task.WhenAll(tasks);
			}
		}

		[Test]
		public void TestThreadUnsafe()
		{
			SyncChecker syncChecker = new SyncChecker();

			Assert.ThrowsAsync<AssertionException>(async () =>
			{
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(async () =>
				{
					for (int i = 0; i < 100; i++)
					{
						syncChecker.Enter();
						await Task.Delay(TimeSpan.FromSeconds(0.001));
						syncChecker.Leave();
					}
				})).ToArray();

				await Task.WhenAll(tasks);
			});
		}

		[Test]
		public void TestTimeoutAsync()
		{
			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						using (await locker.LockAsync(TimeSpan.FromSeconds(0.1)))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestCancellationTokenAsync()
		{
			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (await locker.LockAsync(cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestTimeoutAndCancellationTokenAsync()
		{
			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.2));
						using (await locker.LockAsync(TimeSpan.FromSeconds(0.1), cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (await locker.LockAsync(TimeSpan.FromSeconds(0.2), cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestTimeout()
		{
			ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
			ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);
			Console.WriteLine($"Max\tworker threads: {maxWorkerThreads} io threads: {maxIoThreads}");
			Console.WriteLine($"Min\tworker threads: {minWorkerThreads} io threads: {minIoThreads}");
			ThreadPool.SetMinThreads(minWorkerThreads * 2, minIoThreads);

			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						using (locker.Lock(TimeSpan.FromSeconds(0.1)))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestCancellationToken()
		{
			ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
			ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);
			Console.WriteLine($"Max\tworker threads: {maxWorkerThreads} io threads: {maxIoThreads}");
			Console.WriteLine($"Min\tworker threads: {minWorkerThreads} io threads: {minIoThreads}");
			ThreadPool.SetMinThreads(minWorkerThreads * 2, minIoThreads);

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (locker.Lock(cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestTimeoutAndCancellationToken()
		{
			ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
			ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);
			Console.WriteLine($"Max\tworker threads: {maxWorkerThreads} io threads: {maxIoThreads}");
			Console.WriteLine($"Min\tworker threads: {minWorkerThreads} io threads: {minIoThreads}");
			ThreadPool.SetMinThreads(minWorkerThreads * 2, minIoThreads);

			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
						using (locker.Lock(TimeSpan.FromSeconds(0.1), cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (locker.Lock(TimeSpan.FromSeconds(1), cancellationTokenSource.Token))
						{
							await Task.Delay(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestDoubleRelease()
		{
			using (AsyncLocker locker = new AsyncLocker())
			{
				using (LockReleaser l = locker.Lock())
				{
					l.Dispose();
				}
			}
		}
	}
}
