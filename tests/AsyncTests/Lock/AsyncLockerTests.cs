using CS.Utils.Async.Lock;
using CS.Utils.AsyncTests.TestUtils;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CS.Utils.AsyncTests.Lock
{
	public class AsyncLockerTests
	{
		[Test]
		public async Task TestThreadSafeAsync()
		{
			SyncChecker syncChecker = new SyncChecker();

			using (AsyncLocker locker = new AsyncLocker())
			{
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(async () =>
				{
					for (int i = 0; i < 10000; i++)
					{
						using (await locker.LockAsync())
						{
							syncChecker.Enter();
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
			SyncChecker syncChecker = new SyncChecker();

			using (AsyncLocker locker = new AsyncLocker())
			{
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(() =>
				{
					for (int i = 0; i < 10000; i++)
					{
						using (locker.Lock())
						{
							syncChecker.Enter();
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
				Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(() =>
				{
					for (int i = 0; i < 10000; i++)
					{
						syncChecker.Enter();
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
			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(() =>
					{
						using (locker.Lock(TimeSpan.FromSeconds(0.1)))
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestCancellationToken()
		{
			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(() =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (locker.Lock(cancellationTokenSource.Token))
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}

		[Test]
		public void TestTimeoutAndCancellationToken()
		{
			Assert.ThrowsAsync<TimeoutException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(() =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
						using (locker.Lock(TimeSpan.FromSeconds(0.1), cancellationTokenSource.Token))
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
			{
				using (AsyncLocker locker = new AsyncLocker())
				{
					Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(() =>
					{
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
						using (locker.Lock(TimeSpan.FromSeconds(1), cancellationTokenSource.Token))
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
						}
					})).ToArray();

					await Task.WhenAll(tasks);
				}
			});
		}
	}
}
