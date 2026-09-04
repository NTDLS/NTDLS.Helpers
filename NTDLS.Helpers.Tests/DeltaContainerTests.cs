namespace NTDLS.Helpers.Tests
{
    public class DeltaContainerTests
    {
        [Fact]
        public void CalculateDelta_ReturnsNull_ForFirstObservation()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var result = container.CalculateDelta("key", DateTime.UtcNow, 10);
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateDelta_ReturnsDifference_ForSecondObservation()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculateDelta("key", now, 10);
            var result = container.CalculateDelta("key", now.AddSeconds(1), 25);

            Assert.Equal(15, result);
        }

        [Fact]
        public void CalculateDelta_ReturnsNull_WhenValueDecreases()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculateDelta("key", now, 10);
            var result = container.CalculateDelta("key", now.AddSeconds(1), 5);

            Assert.Equal(0, result);
        }

        [Fact]
        public void TryCalculateDelta_ReturnsFalse_ForFirstObservation()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var success = container.TryCalculateDelta("key", DateTime.UtcNow, 10, out var delta);

            Assert.False(success);
            Assert.Equal(0, delta);
        }

        [Fact]
        public void CalculatePerSecond_ReturnsRate_ForSecondObservation()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculatePerSecond("key", now, 0);
            var result = container.CalculatePerSecond("key", now.AddSeconds(2), 10);

            Assert.Equal(5.0, result);
        }

        [Fact]
        public void CalculatePerSecond_ReturnsNull_WhenTimestampDoesNotAdvance()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculatePerSecond("key", now, 0);
            var result = container.CalculatePerSecond("key", now, 10);

            Assert.Null(result);
        }

        [Fact]
        public void CalculatePerSecond_ReturnsNull_WhenValueDecreases()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculatePerSecond("key", now, 10);
            var result = container.CalculatePerSecond("key", now.AddSeconds(1), 5);

            Assert.Null(result);
        }

        [Fact]
        public void DifferentKeys_TrackIndependently()
        {
            using var container = new DeltaContainer<int>(TimeSpan.FromMinutes(1));
            var now = DateTime.UtcNow;

            container.CalculateDelta("a", now, 100);
            container.CalculateDelta("b", now, 5);

            var deltaA = container.CalculateDelta("a", now.AddSeconds(1), 110);
            var deltaB = container.CalculateDelta("b", now.AddSeconds(1), 8);

            Assert.Equal(10, deltaA);
            Assert.Equal(3, deltaB);
        }
    }
}
