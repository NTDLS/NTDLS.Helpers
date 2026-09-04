namespace NTDLS.Helpers.Tests
{
    public class ExceptionsTests
    {
        [Fact]
        public void GetRoot_ReturnsInnermostException()
        {
            var root = new InvalidOperationException("root");
            var middle = new Exception("middle", root);
            var outer = new Exception("outer", middle);

            Assert.Same(root, outer.GetRoot());
        }

        [Fact]
        public void GetRoot_ReturnsSameException_WhenNoInnerException()
        {
            var ex = new Exception("solo");
            Assert.Same(ex, ex.GetRoot());
        }

        [Fact]
        public void GetRoot_ReturnsNull_WhenExceptionIsNull()
        {
            Exception? ex = null;
            Assert.Null(ex.GetRoot());
        }

        [Fact]
        public void Ignore_SwallowsException()
        {
            Exceptions.Ignore(() => throw new Exception("boom"));
        }

        [Fact]
        public void Ignore_WithResult_ReturnsDefaultOnException()
        {
            var result = Exceptions.Ignore<int>(() => throw new Exception("boom"));
            Assert.Equal(0, result);
        }

        [Fact]
        public void Ignore_WithResult_ReturnsValueOnSuccess()
        {
            var result = Exceptions.Ignore(() => 42);
            Assert.Equal(42, result);
        }

        [Fact]
        public void OnError_InvokesHandlerOnException()
        {
            Exception? caught = null;
            Exceptions.OnError(() => throw new InvalidOperationException("boom"), ex => caught = ex);
            Assert.IsType<InvalidOperationException>(caught);
        }

        [Fact]
        public void OnError_DoesNotInvokeHandlerOnSuccess()
        {
            var handlerCalled = false;
            Exceptions.OnError(() => { }, ex => handlerCalled = true);
            Assert.False(handlerCalled);
        }

        [Fact]
        public void OnError_WithResult_ReturnsHandlerValueOnException()
        {
            var result = Exceptions.OnError<int>(() => throw new Exception("boom"), ex => -1);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void OnError_WithResult_ReturnsFunctionValueOnSuccess()
        {
            var result = Exceptions.OnError<int>(() => 42, ex => -1);
            Assert.Equal(42, result);
        }
    }
}
