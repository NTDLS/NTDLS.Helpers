namespace NTDLS.Helpers.Tests
{
    public class NullExtensionsTests
    {
        [Fact]
        public void IsDefault_TrueForNullReference()
        {
            string? value = null;
            Assert.True(value.IsDefault());
        }

        [Fact]
        public void IsDefault_TrueForDefaultStruct()
        {
            int value = 0;
            Assert.True(value.IsDefault());
        }

        [Fact]
        public void IsDefault_FalseForNonDefaultValue()
        {
            Assert.False(5.IsDefault());
        }

        [Fact]
        public void IsNotDefault_IsInverseOfIsDefault()
        {
            Assert.True(5.IsNotDefault());
            Assert.False(0.IsNotDefault());
        }

        [Fact]
        public void EnsureNotNull_ReferenceType_ReturnsValue()
        {
            string value = "hello";
            Assert.Equal("hello", value.EnsureNotNull());
        }

        [Fact]
        public void EnsureNotNull_ReferenceType_ThrowsOnNull()
        {
            string? value = null;
            Assert.Throws<ArgumentNullException>(() => value.EnsureNotNull());
        }

        [Fact]
        public void EnsureNotNull_ReferenceType_ThrowsArgumentExceptionWithCustomMessage()
        {
            string? value = null;
            var ex = Assert.Throws<ArgumentException>(() => value.EnsureNotNull("custom message"));
            Assert.Equal("custom message", ex.Message.Split(" (")[0]);
        }

        [Fact]
        public void EnsureNotNull_StructType_ReturnsValue()
        {
            int? value = 42;
            Assert.Equal(42, value.EnsureNotNull());
        }

        [Fact]
        public void EnsureNotNull_StructType_ThrowsOnNull()
        {
            int? value = null;
            Assert.Throws<ArgumentNullException>(() => value.EnsureNotNull());
        }

        [Fact]
        public void EnsureNotNullOrEmpty_Struct_ReturnsValue()
        {
            int? value = 7;
            Assert.Equal(7, value.EnsureNotNullOrEmpty());
        }

        [Fact]
        public void EnsureNotNullOrEmpty_Struct_ThrowsOnNullWithCorrectParamName()
        {
            int? value = null;
            var ex = Assert.Throws<ArgumentNullException>(() => value.EnsureNotNullOrEmpty());
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void EnsureNotNullOrEmpty_String_ReturnsValue()
        {
            Assert.Equal("hi", "hi".EnsureNotNullOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnsureNotNullOrEmpty_String_ThrowsOnNullOrEmpty(string? value)
        {
            Assert.Throws<ArgumentNullException>(() => value.EnsureNotNullOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void EnsureNotNullOrWhiteSpace_ThrowsOnNullEmptyOrWhitespace(string? value)
        {
            Assert.Throws<ArgumentNullException>(() => value.EnsureNotNullOrWhiteSpace());
        }

        [Fact]
        public void EnsureNotNullOrWhiteSpace_DoesNotThrowForValidString()
        {
            "hi".EnsureNotNullOrWhiteSpace();
        }

        [Fact]
        public void DefaultWhenNull_ReturnsValueWhenNotNull()
        {
            int? value = 5;
            Assert.Equal(5, value.DefaultWhenNull(10));
        }

        [Fact]
        public void DefaultWhenNull_ReturnsDefaultWhenNull()
        {
            int? value = null;
            Assert.Equal(10, value.DefaultWhenNull(10));
        }

        [Fact]
        public void DefaultWhenNullOrEmpty_ReturnsValueWhenPresent()
        {
            Assert.Equal("hi", "hi".DefaultWhenNullOrEmpty("fallback"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void DefaultWhenNullOrEmpty_ReturnsFallback(string? value)
        {
            Assert.Equal("fallback", value.DefaultWhenNullOrEmpty("fallback"));
        }

        [Fact]
        public void IsNull_TrueForNull()
        {
            string? value = null;
            Assert.True(value.IsNull());
        }

        [Fact]
        public void IsNull_FalseForNonNull()
        {
            Assert.False("hi".IsNull());
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("hi", false)]
        public void IsNullOrEmpty_MatchesStringIsNullOrEmpty(string? value, bool expected)
        {
            Assert.Equal(expected, value!.IsNullOrEmpty());
        }
    }
}
