namespace NTDLS.Helpers.Tests
{
    public class InsensitiveStringTests
    {
        [Fact]
        public void Is_TrueForCaseInsensitiveMatch()
        {
            Assert.True("Hello".Is("hello"));
        }

        [Fact]
        public void Is_FalseForDifferentValues()
        {
            Assert.False("Hello".Is("world"));
        }

        [Fact]
        public void IsNot_InverseOfIs()
        {
            Assert.True("Hello".IsNot("world"));
            Assert.False("Hello".IsNot("hello"));
        }

        [Fact]
        public void IsOneOf_TrueWhenPresentIgnoringCase()
        {
            Assert.True("Hello".IsOneOf(new[] { "foo", "HELLO", "bar" }));
        }

        [Fact]
        public void IsOneOf_FalseWhenAbsent()
        {
            Assert.False("Hello".IsOneOf(new[] { "foo", "bar" }));
        }

        [Fact]
        public void ContainsInsensitive_Single_TrueIgnoringCase()
        {
            Assert.True("Hello World".ContainsInsensitive("WORLD"));
        }

        [Fact]
        public void ContainsInsensitive_Array_TrueWhenAnyMatch()
        {
            Assert.True("Hello World".ContainsInsensitive(new[] { "xyz", "WORLD" }));
        }

        [Fact]
        public void ContainsInsensitive_Array_FalseWhenNoneMatch()
        {
            Assert.False("Hello World".ContainsInsensitive(new[] { "xyz", "abc" }));
        }

        [Theory]
        [InlineData("hello.txt", "%.txt", true)]
        [InlineData("hello.csv", "%.txt", false)]
        [InlineData("abc", "a_c", true)]
        public void IsLike_SupportsSqlStyleWildcards(string value, string pattern, bool expected)
        {
            Assert.Equal(expected, value.IsLike(pattern));
        }

        [Fact]
        public void IsLike_IsCaseInsensitive()
        {
            Assert.True("HELLO.TXT".IsLike("%.txt"));
        }
    }
}
