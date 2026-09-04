namespace NTDLS.Helpers.Tests
{
    public class TextTests
    {
        [Fact]
        public void TruncateAtWord_ReturnsOriginal_WhenShorterThanDesiredLength()
        {
            Assert.Equal("short", Text.TruncateAtWord("short", 10));
        }

        [Fact]
        public void TruncateAtWord_TruncatesAtNextSpace_WithEllipsis()
        {
            Assert.Equal("The quick...", Text.TruncateAtWord("The quick brown fox", 5));
        }

        [Fact]
        public void TruncateAtWord_TruncatesAtNextSpace_WithoutEllipsis()
        {
            Assert.Equal("The quick", Text.TruncateAtWord("The quick brown fox", 5, addEllipsis: false));
        }

        [Fact]
        public void TruncateAtWord_ReturnsFullText_WhenNoTrailingWhitespace()
        {
            Assert.Equal("Thequickbrownfox", Text.TruncateAtWord("Thequickbrownfox", 5));
        }

        [Fact]
        public void TruncateAtWord_DoesNotThrow_ForNegativeDesiredLength()
        {
            var result = Text.TruncateAtWord("The quick brown fox", -5);
            Assert.NotNull(result);
        }

        [Fact]
        public void ReplaceFirstOccurrence_ReplacesOnlyFirstOccurrence()
        {
            Assert.Equal("x bar foo baz foo", Text.ReplaceFirstOccurrence("foo bar foo baz foo", "foo", "x"));
        }

        [Fact]
        public void ReplaceFirstOccurrence_ReturnsOriginal_WhenNotFound()
        {
            Assert.Equal("foo bar", Text.ReplaceFirstOccurrence("foo bar", "baz", "x"));
        }

        [Fact]
        public void ReplaceRange_ReplacesSpecifiedRange()
        {
            Assert.Equal("Hello Universe!", Text.ReplaceRange("Hello World!", 6, 5, "Universe"));
        }

        [Fact]
        public void SoftWrap_DoesNotLeaveTrailingSpaceBeforeLineBreak()
        {
            var result = Text.SoftWrap("aaaa bbbb", 4);
            Assert.Equal("aaaa\r\nbbbb", result);
        }

        [Fact]
        public void SoftWrap_KeepsPunctuationAtLineEnd()
        {
            var result = Text.SoftWrap("aaaa,bbbb", 4);
            Assert.Equal("aaaa,\r\nbbbb", result);
        }

        [Fact]
        public void SoftWrap_ReturnsOriginal_WhenNullOrWhitespace()
        {
            Assert.Equal("   ", Text.SoftWrap("   ", 10));
        }

        [Fact]
        public void SoftWrap_WithCustomBreakCharacters()
        {
            var result = Text.SoftWrap("aaaa|bbbb", 4, new[] { '|' });
            Assert.Equal("aaaa|\r\nbbbb", result);
        }

        [Fact]
        public void SeparateCamelCase_InsertsSpaces()
        {
            Assert.Equal("The Quick Brown Fox", Text.SeparateCamelCase("TheQuickBrownFox"));
        }

        [Fact]
        public void SeparateCamelCase_HandlesAcronyms()
        {
            Assert.Equal("XML Http Request", Text.SeparateCamelCase("XMLHttpRequest"));
        }

        [Fact]
        public void SplitCamelCase_SplitsIntoTokens()
        {
            Assert.Equal(new[] { "The", "Quick", "Brown", "Fox" }, Text.SplitCamelCase("TheQuickBrownFox"));
        }

        [Fact]
        public void SplitCamelCase_ReturnsEmptyList_ForNullOrWhitespace()
        {
            Assert.Empty(Text.SplitCamelCase("   "));
        }

        [Fact]
        public void RemoveWhitespace_RemovesAllWhitespaceCharacters()
        {
            Assert.Equal("Hello,World!", Text.RemoveWhitespace(" Hello,  World! \t\r\n"));
        }
    }
}
