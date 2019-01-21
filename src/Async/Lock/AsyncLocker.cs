using System;
using System.Threading;
using System.Threading.Tasks;

namespace CS.Utils.Async.Lock
{
	public class AsyncLocker : IDisposable
	{
		private readonly SemaphoreSlim semaphore;

		public AsyncLocker(int concurrentTasks = 1)
		{
			semaphore = new SemaphoreSlim(concurrentTasks, concurrentTasks);
		}

		public LockReleaser Lock()
		{
			semaphore.Wait();
			return new LockReleaser(this);
		}
		public async Task<LockReleaser> LockAsync()
		{
			await semaphore.WaitAsync();
			return new LockReleaser(this);
		}

		public bool Release()
		{
			try
			{
				semaphore.Release();
				return true;
			}
			catch (SemaphoreFullException)
			{
				return false;
			}
		}

		public void Dispose()
		{
			semaphore.Dispose();
		}
	}

	public class LockReleaser : IDisposable
	{
		private bool isDisposed = false;
		private readonly AsyncLocker locker;

		public LockReleaser(AsyncLocker locker)
		{
			this.locker = locker;
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				locker.Release();
			}
		}
	}
}
