namespace NTDLS.Helpers.Tests
{
    public class ThreadingTests
    {
        [Fact]
        public void StartThread_NoParameter_ExecutesDelegate()
        {
            var signal = new ManualResetEventSlim(false);

            var thread = Threading.StartThread(() => signal.Set());

            Assert.True(signal.Wait(TimeSpan.FromSeconds(5)));
            thread.Join(TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void StartThread_WithParameter_PassesParameterThrough()
        {
            var signal = new ManualResetEventSlim(false);
            int received = -1;

            var thread = Threading.StartThread(42, (int value) =>
            {
                received = value;
                signal.Set();
            });

            Assert.True(signal.Wait(TimeSpan.FromSeconds(5)));
            thread.Join(TimeSpan.FromSeconds(5));
            Assert.Equal(42, received);
        }
    }
}
