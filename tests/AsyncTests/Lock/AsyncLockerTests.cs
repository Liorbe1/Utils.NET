using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using CS.Utils.Async.Lock;
using CS.Utils.AsyncTests.TestUtils;

namespace CS.Utils.AsyncTests.Lock
{
    public class AsyncLockerTests
    {
        [Test]
        public async Task TestThreadSafe()
        {
            SyncChecker syncChecker = new SyncChecker();

            using (AsyncLocker locker = new AsyncLocker())
            {
                Task[] tasks = Enumerable.Range(0, 10).Select(x => Task.Run(async () =>
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        using (await locker.Lock())
                        {
                            syncChecker.Enter();
                            syncChecker.Leave();
                        }
                    }
                })).ToArray();

                await Task.WhenAll(tasks);
            }
        }
    }
}
