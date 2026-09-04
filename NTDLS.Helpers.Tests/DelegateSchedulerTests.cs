namespace NTDLS.Helpers.Tests
{
    public class DelegateSchedulerTests
    {
        [Fact]
        public void TryRegister_ReturnsTrue_ForNewName()
        {
            using var scheduler = new DelegateScheduler();
            Assert.True(scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => { }));
        }

        [Fact]
        public void TryRegister_ReturnsFalse_ForDuplicateName()
        {
            using var scheduler = new DelegateScheduler();
            scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => { });

            Assert.False(scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => { }));
        }

        [Fact]
        public void Register_Throws_ForDuplicateName()
        {
            using var scheduler = new DelegateScheduler();
            scheduler.Register("job", TimeSpan.FromMinutes(5), () => { });

            Assert.Throws<InvalidOperationException>(() =>
                scheduler.Register("job", TimeSpan.FromMinutes(5), () => { }));
        }

        [Fact]
        public void IsRegistered_ReflectsRegistrationState()
        {
            using var scheduler = new DelegateScheduler();
            Assert.False(scheduler.IsRegistered("job"));

            scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => { });
            Assert.True(scheduler.IsRegistered("job"));
        }

        [Fact]
        public void TryUnregister_RemovesTaskAndStopsFutureRuns()
        {
            using var scheduler = new DelegateScheduler();
            scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => { });

            Assert.True(scheduler.TryUnregister("job"));
            Assert.False(scheduler.IsRegistered("job"));
            Assert.False(scheduler.TryUnregister("job"));
        }

        [Fact]
        public void TryRegister_StartImmediately_RunsRightAway()
        {
            using var scheduler = new DelegateScheduler();
            var signal = new ManualResetEventSlim(false);

            scheduler.TryRegister("job", TimeSpan.FromMinutes(5), () => signal.Set(), startImmediately: true);

            Assert.True(signal.Wait(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void Register_RecurringInterval_FiresRepeatedly()
        {
            using var scheduler = new DelegateScheduler();
            var runCount = 0;
            var signal = new ManualResetEventSlim(false);

            scheduler.Register(TimeSpan.FromMilliseconds(50), () =>
            {
                if (Interlocked.Increment(ref runCount) >= 3)
                {
                    signal.Set();
                }
            });

            Assert.True(signal.Wait(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void Register_AtSpecificTime_FiresOnceAndUnregisters()
        {
            using var scheduler = new DelegateScheduler();
            var signal = new ManualResetEventSlim(false);
            var runCount = 0;

            var name = Guid.NewGuid().ToString();
            scheduler.Register(name, DateTime.UtcNow.AddMilliseconds(50), () =>
            {
                Interlocked.Increment(ref runCount);
                signal.Set();
            });

            Assert.True(signal.Wait(TimeSpan.FromSeconds(5)));

            // Give the scheduler a moment to unregister itself after firing.
            Thread.Sleep(200);
            Assert.False(scheduler.IsRegistered(name));
            Assert.Equal(1, runCount);
        }
    }
}
