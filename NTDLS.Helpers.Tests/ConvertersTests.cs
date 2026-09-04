namespace NTDLS.Helpers.Tests
{
    public class ConvertersTests
    {
        [Fact]
        public void ConvertTo_ParsesInt()
        {
            Assert.Equal(123, Converters.ConvertTo<int>("123"));
        }

        [Fact]
        public void ConvertTo_ParsesDouble()
        {
            Assert.Equal(1.5, Converters.ConvertTo<double>("1.5"));
        }

        [Fact]
        public void ConvertTo_ParsesBoolFromWord()
        {
            Assert.True(Converters.ConvertTo<bool>("true"));
            Assert.False(Converters.ConvertTo<bool>("false"));
        }

        [Fact]
        public void ConvertTo_ParsesBoolFromNumber()
        {
            Assert.True(Converters.ConvertTo<bool>("1"));
            Assert.False(Converters.ConvertTo<bool>("0"));
        }

        [Fact]
        public void ConvertTo_ParsesEnum()
        {
            Assert.Equal(DayOfWeek.Friday, Converters.ConvertTo<DayOfWeek>("Friday"));
        }

        [Fact]
        public void ConvertTo_ParsesNullableTargetType()
        {
            Assert.Equal(42, Converters.ConvertTo<int?>("42", null));
        }

        [Fact]
        public void ConvertTo_WithDefault_ReturnsDefaultOnFailure()
        {
            Assert.Equal(99, Converters.ConvertTo("not-a-number", 99));
        }

        [Fact]
        public void ConvertTo_WithDefault_ReturnsDefaultOnNull()
        {
            Assert.Equal(99, Converters.ConvertTo<int>(null, 99));
        }

        [Fact]
        public void ConvertTo_WithoutDefault_ThrowsOnFailure()
        {
            Assert.Throws<FormatException>(() => Converters.ConvertTo<int>("not-a-number"));
        }

        [Fact]
        public void ConvertTo_WithoutDefault_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => Converters.ConvertTo<int>(null!));
        }

        [Fact]
        public void ConvertToNullable_ReturnsNullForNullInput()
        {
            // Must be instantiated with the nullable type itself (int?, not int) to get real
            // null semantics for a value type — see Converters.cs remarks on generic erasure.
            Assert.Null(Converters.ConvertToNullable<int?>(null));
        }

        [Fact]
        public void ConvertToNullable_NonNullableValueType_ReturnsDefaultRatherThanNull()
        {
            // Documents a sharp edge: for an unconstrained generic T, "T?" erases to plain T
            // when T is a non-nullable value type, so this can never actually be null.
            Assert.Equal(0, Converters.ConvertToNullable<int>(null));
        }

        [Fact]
        public void ConvertTo_SingleCharacter_Succeeds()
        {
            Assert.Equal('x', Converters.ConvertTo<char>("x"));
        }

        [Fact]
        public void ConvertTo_MultiCharacter_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Converters.ConvertTo<char>("xy"));
        }

        [Fact]
        public void ConvertTo_UnsupportedType_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Converters.ConvertTo<object>("abc"));
        }
    }
}
