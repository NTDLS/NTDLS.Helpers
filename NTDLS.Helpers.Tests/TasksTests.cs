namespace NTDLS.Helpers.Tests
{
    public class TasksTests
    {
        [Fact]
        public void ThrowTaskException_DoesNotThrow_WhenTaskSucceeded()
        {
            var task = Task.FromResult(1);
            Tasks.ThrowTaskException(task);
        }

        [Fact]
        public void ThrowTaskException_Throws_WhenTaskFaulted()
        {
            var task = Task.FromException<int>(new InvalidOperationException("boom"));
            Assert.Throws<Exception>(() => Tasks.ThrowTaskException(task));
        }

        [Fact]
        public void ThrowTaskException_Throws_WhenTaskCanceled()
        {
            var task = Task.FromCanceled<int>(new CancellationToken(true));
            Assert.Throws<Exception>(() => Tasks.ThrowTaskException(task));
        }

        [Fact]
        public void ThrowTaskException_WithCustomText_UsesGivenMessage()
        {
            var task = Task.FromException<int>(new InvalidOperationException("boom"));
            var ex = Assert.Throws<Exception>(() => Tasks.ThrowTaskException(task, "custom failure"));
            Assert.Equal("custom failure", ex.Message);
        }
    }
}
