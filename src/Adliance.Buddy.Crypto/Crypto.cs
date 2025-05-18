using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Adliance.Buddy.Crypto;

public static class Crypto
{
    #region V1

    /// <summary>
    /// Calculate a hash from a string value without a salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The computed hash.</returns>
    public static string HashWithoutSalt(string value)
    {
        return HashInternal(value, null);
    }

    /// <summary>
    /// Calculate a hash from a string value with a random generated salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The generated salt.</param>
    /// <returns>The computed hash.</returns>
    public static string Hash(string value, out string salt)
    {
        salt = GenerateRandomSalt();
        return Hash(value, salt);
    }

    /// <summary>
    /// Calculate a hash from a string value with an optional salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The optional salt as a base64 string.</param>
    /// <returns>The computed hash.</returns>
    /// <exception cref="FormatException">Thrown if the salt is an invalid base64 string.</exception>
    public static string Hash(string value, string? salt)
    {
        var saltBytes = salt != null ? GetSaltBytes(salt) : null;
        return HashInternal(value, saltBytes);
    }

    /// <summary>
    /// Calculate a hash from a string value with a Guid salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>The computed hash.</returns>
    public static string Hash(string value, Guid salt)
    {
        return HashInternal(value, GetSaltBytes(salt));
    }

    private static string HashInternal(string value, byte[]? salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(value, salt ?? Array.Empty<byte>(),
            KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
    }

    #endregion

    #region V2

    /// <summary>
    /// Calculate a V2 hash from a string value without salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The computed hash.</returns>
    public static string HashWithoutSaltV2(string value)
    {
        return HashInternalV2(value, null);
    }

    /// <summary>
    /// Calculate a V2 hash from a string value with a random generated salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The generated salt.</param>
    /// <returns>The computed hash.</returns>
    public static string HashV2(string value, out string salt)
    {
        salt = GenerateRandomSalt();
        return HashV2(value, salt);
    }

    /// <summary>
    /// Calculate a V2 hash from a string value with a salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt as a base64 string.</param>
    /// <returns>The computed hash.</returns>
    /// <exception cref="FormatException">Thrown if the salt is an invalid base64 string.</exception>
    public static string HashV2(string value, string salt)
    {
        return HashInternalV2(value, GetSaltBytes(salt));
    }

    /// <summary>
    /// Calculate a hash from a string value with a Guid salt.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>The computed hash.</returns>
    public static string HashV2(string value, Guid salt)
    {
        return HashInternalV2(value, GetSaltBytes(salt));
    }

    /// <summary>
    /// Verify a V2 hash by calculating a hash from a value without a salt and compare it to an existing hash.
    /// </summary>
    /// <param name="value">The value that should be hashed.</param>
    /// <param name="hash">The hash to compare to.</param>
    /// <returns>True if the hashes match, otherwise false.</returns>
    public static bool VerifyHashWithoutSaltV2(string value, string hash)
    {
        var hashToCompare = HashWithoutSaltV2(value);
        return hashToCompare == hash;
    }

    /// <summary>
    /// Verify a V2 hash by calculating a hash from a value with a salt and compare it to an existing hash.
    /// </summary>
    /// <param name="value">The value that should be hashed.</param>
    /// <param name="hash">The hash to compare to.</param>
    /// <param name="salt">The salt which should be used to calculate the hash.</param>
    /// <returns>True if the hashes match, otherwise false.</returns>
    public static bool VerifyHashV2(string value, string hash, string salt)
    {
        var hashToCompare = HashV2(value, salt);
        return hashToCompare == hash;
    }

    /// <summary>
    /// Verify a V2 hash by calculating a hash from a value with a salt and compare it to an existing hash.
    /// </summary>
    /// <param name="value">The value that should be hashed.</param>
    /// <param name="hash">The hash to compare to.</param>
    /// <param name="salt">The salt which should be used to calculate the hash.</param>
    /// <returns>True if the hashes match, otherwise false.</returns>
    public static bool VerifyHashV2(string value, string hash, Guid salt)
    {
        var hashToCompare = HashV2(value, salt);
        return hashToCompare == hash;
    }

    private static string HashInternalV2(string value, byte[]? salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(value, salt ?? Array.Empty<byte>(),
            KeyDerivationPrf.HMACSHA256, 600000, 256 / 8));
    }

    #endregion

    /// <summary>
    /// Generate a crypto secure random string.
    /// </summary>
    /// <param name="length">The desired string length. Must be greater than zero.</param>
    /// <param name="charSet">The set of characters that should be used for generating the random string.</param>
    /// <returns>A crypto secure random string with the desired length and characters.</returns>
    /// <exception cref="ArgumentException">Thrown if either the <paramref name="length"/> or <paramref name="charSet"/> is invalid.</exception>
    public static string RandomString(int length,
        string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        if (length <= 0)
        {
            throw new ArgumentException("length must be greater than zero", nameof(length));
        }

        if (string.IsNullOrEmpty(charSet))
        {
            throw new ArgumentException("charSet must not be empty", nameof(charSet));
        }

        if (charSet.Length < 2)
        {
            throw new ArgumentException("charSet must contain at least two characters", nameof(charSet));
        }

        var result = new char[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = charSet[RandomNumberGenerator.GetInt32(charSet.Length)];
        }

        return new string(result);
    }

    private static string GenerateRandomSalt()
    {
        var saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    private static byte[] GetSaltBytes(string base64Salt)
    {
        return Convert.FromBase64String(base64Salt);
    }

    private static byte[] GetSaltBytes(Guid salt)
    {
        return Encoding.Unicode.GetBytes(salt.ToString());
    }
}
