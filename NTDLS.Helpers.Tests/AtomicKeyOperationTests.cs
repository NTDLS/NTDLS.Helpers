namespace NTDLS.Helpers.Tests
{
    public class AtomicKeyOperationTests
    {
        [Fact]
        public void Execute_Action_RunsDelegate()
        {
            var atomic = new AtomicKeyOperation();
            var ran = false;

            atomic.Execute("key", () => ran = true);

            Assert.True(ran);
        }

        [Fact]
        public void Execute_Function_ReturnsValue()
        {
            var atomic = new AtomicKeyOperation();
            var result = atomic.Execute("key", () => 42);
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task Execute_DifferentKeys_CanRunConcurrently()
        {
            var atomic = new AtomicKeyOperation();
            using var barrier = new Barrier(2);

            var task1 = Task.Run(() => atomic.Execute("key1", () =>
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(5));
            }));
            var task2 = Task.Run(() => atomic.Execute("key2", () =>
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(5));
            }));

            var all = Task.WhenAll(task1, task2);
            var winner = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(5)));

            Assert.Same(all, winner);
        }

        [Fact]
        public async Task Execute_SameKey_SerializesConcurrentCalls()
        {
            var atomic = new AtomicKeyOperation();
            var concurrentCount = 0;
            var maxObservedConcurrency = 0;
            var gate = new object();

            void Bump()
            {
                lock (gate)
                {
                    concurrentCount++;
                    maxObservedConcurrency = Math.Max(maxObservedConcurrency, concurrentCount);
                }
                Thread.Sleep(50);
                lock (gate)
                {
                    concurrentCount--;
                }
            }

            var tasks = Enumerable.Range(0, 5)
                .Select(_ => Task.Run(() => atomic.Execute("shared-key", Bump)))
                .ToArray();

            var all = Task.WhenAll(tasks);
            var winner = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(10)));

            Assert.Same(all, winner);
            Assert.Equal(1, maxObservedConcurrency);
        }

        [Fact]
        public void Execute_PropagatesException()
        {
            var atomic = new AtomicKeyOperation();
            Assert.Throws<InvalidOperationException>(() =>
                atomic.Execute("key", () => throw new InvalidOperationException("boom")));
        }

        [Fact]
        public void Execute_ThrowsOnNullKey()
        {
            var atomic = new AtomicKeyOperation();
            Assert.Throws<ArgumentNullException>(() => atomic.Execute(null!, () => { }));
        }

        [Fact]
        public void Execute_ThrowsOnNullAction()
        {
            var atomic = new AtomicKeyOperation();
            Assert.Throws<ArgumentNullException>(() => atomic.Execute("key", (Action)null!));
        }
    }
}
