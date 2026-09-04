namespace NTDLS.Helpers.Tests
{
    public class FormattersTests
    {
        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(500, "500 B")]
        [InlineData(1536, "1.50 KB")]
        public void FileSize_Long_FormatsExpectedUnit(long size, string expected)
        {
            Assert.Equal(expected, Formatters.FileSize(size));
        }

        [Fact]
        public void FileSize_Long_MB()
        {
            Assert.Equal("1.00 MB", Formatters.FileSize(1024L * 1024));
        }

        [Fact]
        public void FileSize_WithDecimalPlaces()
        {
            Assert.Equal("1.00 KB", Formatters.FileSize(1024L, 2));
        }

        [Fact]
        public void FileSize_Double_DoesNotThrowForHugeValues()
        {
            // Regression test: previously overflowed the format array for values beyond the EB range.
            var result = Formatters.FileSize(1e30);
            Assert.EndsWith("EB", result);
        }

        [Fact]
        public void FileSize_Decimal_DoesNotOverflow()
        {
            // Regression test: previously threw OverflowException when cast straight to long.
            var result = Formatters.FileSize(decimal.MaxValue);
            Assert.EndsWith("EB", result);
        }

        [Fact]
        public void FileSize_Int_MatchesLongOverload()
        {
            Assert.Equal(Formatters.FileSize(2048L), Formatters.FileSize(2048));
        }

        [Fact]
        public void FileSize_Float_MatchesDoubleOverload()
        {
            Assert.Equal(Formatters.FileSize(2048d), Formatters.FileSize(2048f));
        }

        [Fact]
        public void FileSize_Ulong_Works()
        {
            Assert.Equal("1 KB", Formatters.FileSize((ulong)1024));
        }
    }
}
