using Xunit;

namespace Adliance.Buddy.Crypto.Test;

public class CryptoTest
{
    private const string TestPassword1 = @"R£pCn&ATv7-95L36";
    private const string TestPassword2 = @"R£pCn&ATv7-95L37";

    [Fact]
    public void HashWithoutSalt_Is_Equal()
    {
        var hash1 = Crypto.HashWithoutSalt(TestPassword1);
        var hash2 = Crypto.HashWithoutSalt(TestPassword1);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_With_Null_Salt_Is_Equal()
    {
        var hash1 = Crypto.Hash(TestPassword1, null);
        var hash2 = Crypto.Hash(TestPassword1, null);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_With_Equal_Guid_Salt_Is_Equal()
    {
        var guid = Guid.NewGuid();
        var hash1 = Crypto.Hash(TestPassword1, guid);
        var hash2 = Crypto.Hash(TestPassword1, guid);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_With_Unequal_Guid_Salt_Is_Unequal()
    {
        var hash1 = Crypto.Hash(TestPassword1, Guid.NewGuid());
        var hash2 = Crypto.Hash(TestPassword1, Guid.NewGuid());
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Hash_With_Equal_Salt_Is_Equal()
    {
        var hash1 = Crypto.Hash(TestPassword1, out var salt);
        var hash2 = Crypto.Hash(TestPassword1, salt);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_With_Two_Random_Salt_Is_Unequal()
    {
        var hash1 = Crypto.Hash(TestPassword1, out var salt1);
        var hash2 = Crypto.Hash(TestPassword1, out var salt2);
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

    [Theory]
    [InlineData("!")]
    [InlineData("abcde")]
    public void Hash_With_Invalid_Salt_Throws(string salt)
    {
        Assert.Throws<FormatException>(() => Crypto.Hash(TestPassword1, salt));
    }

    [Fact]
    public void HashWithoutSaltV2_Is_Equal()
    {
        var hash1 = Crypto.HashWithoutSaltV2(TestPassword1);
        var hash2 = Crypto.HashWithoutSaltV2(TestPassword1);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashV2_With_Equal_Guid_Salt_Is_Equal()
    {
        var guid = Guid.NewGuid();
        var hash1 = Crypto.HashV2(TestPassword1, guid);
        var hash2 = Crypto.HashV2(TestPassword1, guid);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashV2_With_Unequal_Guid_Salt_Is_Unequal()
    {
        var hash1 = Crypto.HashV2(TestPassword1, Guid.NewGuid());
        var hash2 = Crypto.HashV2(TestPassword1, Guid.NewGuid());
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashV2_With_Equal_Salt_Is_Equal()
    {
        var hash1 = Crypto.HashV2(TestPassword1, out var salt);
        var hash2 = Crypto.HashV2(TestPassword1, salt);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashV2_With_Two_Random_Salt_Is_Unequal()
    {
        var hash1 = Crypto.HashV2(TestPassword1, out var salt1);
        var hash2 = Crypto.HashV2(TestPassword1, out var salt2);
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

    [Theory]
    [InlineData("!")]
    [InlineData("abcde")]
    public void HashV2_With_Invalid_Salt_Throws(string salt)
    {
        Assert.Throws<FormatException>(() => Crypto.HashV2(TestPassword1, salt));
    }

    [Fact]
    public void VerifyHashWithoutSaltV2_Is_True()
    {
        var hash = Crypto.HashWithoutSaltV2(TestPassword1);
        var equal = Crypto.VerifyHashWithoutSaltV2(TestPassword1, hash);
        Assert.True(equal);
    }

    [Fact]
    public void VerifyHashWithoutSaltV2_Is_False()
    {
        var hash = Crypto.HashWithoutSaltV2(TestPassword1);
        var equal = Crypto.VerifyHashWithoutSaltV2(TestPassword2, hash);
        Assert.False(equal);
    }

    [Fact]
    public void VerifyHashV2_With_Salt_Is_True()
    {
        var hash = Crypto.HashV2(TestPassword1, out var salt);
        var equal = Crypto.VerifyHashV2(TestPassword1, hash, salt);
        Assert.True(equal);
    }

    [Fact]
    public void VerifyHashV2_With_Salt_Is_False()
    {
        var hash = Crypto.HashV2(TestPassword1, out var salt);
        var equal = Crypto.VerifyHashV2(TestPassword2, hash, salt);
        Assert.False(equal);
    }

    [Fact]
    public void VerifyHashV2_With_Guid_Salt_Is_True()
    {
        var guidSalt = Guid.NewGuid();
        var hash = Crypto.HashV2(TestPassword1, guidSalt);
        var equal = Crypto.VerifyHashV2(TestPassword1, hash, guidSalt);
        Assert.True(equal);
    }

    [Fact]
    public void VerifyHashV2_With_Guid_Salt_Is_False()
    {
        var guidSalt = Guid.NewGuid();
        var hash = Crypto.HashV2(TestPassword1, guidSalt);
        var equal = Crypto.VerifyHashV2(TestPassword2, hash, guidSalt);
        Assert.False(equal);
    }

    [Fact]
    public void RandomString_Contains_Correct_Characters()
    {
        var random = Crypto.RandomString(10000, "ab");
        var charactersValid = random.All(x => "ab".Contains(x));
        Assert.True(charactersValid);
        Assert.Equal(10000, random.Length);
    }

    [Theory]
    [InlineData(-1, "abc")]
    [InlineData(0, "abc")]
    [InlineData(int.MinValue, "abc")]
    [InlineData(5, "")]
    [InlineData(5, "a")]
    public void RandomString_Throws(int length, string charSet)
    {
        Assert.Throws<ArgumentException>(() => Crypto.RandomString(length, charSet));
    }
}