using NUnit.Framework;
using System.Threading;

namespace CS.Utils.AsyncTests.TestUtils
{
    internal class SyncChecker
    {
        private int insideCounter = 0;

        public void Enter()
        {
            Assert.AreEqual(1, Interlocked.Increment(ref insideCounter));
        }

        public void Leave()
        {
            Assert.AreEqual(0, Interlocked.Decrement(ref insideCounter));
        }
    }
}
