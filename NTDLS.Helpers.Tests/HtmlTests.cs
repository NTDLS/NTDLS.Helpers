namespace NTDLS.Helpers.Tests
{
    public class HtmlTests
    {
        [Fact]
        public void StripHtml_RemovesTags()
        {
            var result = Html.StripHtml("<p>Hello World</p>");
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void StripHtml_RemovesEntities()
        {
            var result = Html.StripHtml("Fish &amp; Chips");
            Assert.Equal("Fish Chips", result);
        }

        [Fact]
        public void StripHtml_CollapsesWhitespace()
        {
            var result = Html.StripHtml("<div>Hello</div>   <div>World</div>");
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void StripHtml_WithScopes_RemovesScriptBlocks()
        {
            var scopes = new[] { new Html.HtmlScope("<script>", "</script>") };
            var result = Html.StripHtml("Before <script>alert('x')</script> After", scopes);
            Assert.Equal("Before After", result);
        }

        [Fact]
        public void StripHtml_WithExtraCharacters_RemovesThem()
        {
            var result = Html.StripHtml("Hello-World", null, new[] { '-' });
            Assert.Equal("HelloWorld", result);
        }
    }
}
