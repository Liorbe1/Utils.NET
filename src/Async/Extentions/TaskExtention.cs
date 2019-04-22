using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS.Utils.Async.Extentions
{
	public static class TaskExtention
	{
		public static async Task SetTimeout(this Task task, TimeSpan timeout)
		{
			using (CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				Task delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
				await Task.WhenAny(task, delayTask);

				if (!task.IsCompleted)
				{
					throw new TimeoutException("Operation timed out");
				}
				else
				{
					timeoutCancellationTokenSource.Cancel();
					await task;
				}
			}
		}
		public static async Task<TResult> SetTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
		{
			await (task as Task).SetTimeout(timeout);
			return await task;
		}
	}
}
