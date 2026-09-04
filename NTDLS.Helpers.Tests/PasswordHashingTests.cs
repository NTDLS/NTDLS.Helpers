namespace NTDLS.Helpers.Tests
{
    public class PasswordHashingTests
    {
        [Fact]
        public void Verify_ReturnsTrue_ForCorrectPassword()
        {
            var hash = PasswordHashing.Hash("correct horse battery staple");
            Assert.True(PasswordHashing.Verify("correct horse battery staple", hash));
        }

        [Fact]
        public void Verify_ReturnsFalse_ForIncorrectPassword()
        {
            var hash = PasswordHashing.Hash("correct horse battery staple");
            Assert.False(PasswordHashing.Verify("wrong password", hash));
        }

        [Fact]
        public void Hash_ProducesDifferentOutput_ForSamePassword()
        {
            // Salted, so two hashes of the same password should differ.
            var hash1 = PasswordHashing.Hash("password");
            var hash2 = PasswordHashing.Hash("password");
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void Verify_ReturnsFalse_ForMalformedHash()
        {
            Assert.False(PasswordHashing.Verify("password", "not-a-valid-hash"));
        }

        [Fact]
        public void Hash_ContainsIterationCountSaltAndHashSeparatedByDots()
        {
            var hash = PasswordHashing.Hash("password");
            var parts = hash.Split('.');
            Assert.Equal(3, parts.Length);
            Assert.True(int.TryParse(parts[0], out _));
        }
    }
}
