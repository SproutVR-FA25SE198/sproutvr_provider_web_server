using System.Globalization;
using System.Text;

namespace Services.Orders.Application.Helpers;

/// <summary>
/// Encodes and decodes byte arrays using Crockford's Base32 encoding.
/// This encoding is user-friendly as it excludes ambiguous characters like 'I', 'L', 'O', and 'U'.
/// It is also case-insensitive.
/// </summary>
public static class Base32EncodingUtils
{
    /// <summary>
    /// The 32-character alphabet used in Crockford's Base32 encoding.
    /// </summary>
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

    /// <summary>
    /// A char array cache of the <see cref="Alphabet"/> for fast lookups during encoding.
    /// </summary>
    private static readonly char[] AlphabetChars = Alphabet.ToCharArray();

    /// <summary>
    /// A reverse lookup table to quickly convert a Base32 character (ASCII value) back to its 5-bit integer value.
    /// Unused ASCII values are initialized to 0xFF.
    /// </summary>
    private static readonly byte[] ReverseAlphabet = new byte[128];

    /// <summary>
    /// Static constructor to initialize the reverse lookup table <see cref="ReverseAlphabet"/>.
    /// This table allows for O(1) lookups when decoding.
    /// </summary>
    static Base32EncodingUtils()
    {
        // Initialize all values to 0xFF (an invalid value)
        Array.Fill(ReverseAlphabet, (byte)0xFF);

        // Populate the lookup table for both uppercase and lowercase versions of the alphabet
        for (int i = 0; i < Alphabet.Length; i++)
        {
            char c = Alphabet[i];
            ReverseAlphabet[c] = (byte)i;
            ReverseAlphabet[char.ToLower(c, CultureInfo.InvariantCulture)] = (byte)i;
        }
    }

    /// <summary>
    /// Encodes a byte array into its Crockford's Base32 string representation.
    /// </summary>
    /// <param name="data">The raw byte array to encode.</param>
    /// <returns>A Base32-encoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    public static string ToBase32String(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Pre-calculate capacity: Each byte is 8 bits, each Base32 char is 5 bits.
        // (Length * 8) / 5. The +4 is to round up to the nearest 5-bit chunk.
        var result = new StringBuilder((data.Length * 8 + 4) / 5);

        int buffer = 0;   // A bit buffer to hold bits read from the input
        int bitsLeft = 0; // The number of valid bits currently in the buffer

        foreach (byte b in data)
        {
            // Shift the existing buffer left by 8 bits and add the new byte
            buffer = (buffer << 8) | b;
            bitsLeft += 8;

            // Process all 5-bit chunks currently available in the buffer
            while (bitsLeft >= 5)
            {
                // Get the top 5 bits:
                // 1. Shift the buffer right so the 5 bits are at the bottom.
                // 2. Mask with 0x1F (binary 0001 1111) to isolate those 5 bits.
                int index = (buffer >> (bitsLeft - 5)) & 0x1F;
                result.Append(AlphabetChars[index]);

                // Remove the 5 bits we just processed
                bitsLeft -= 5;
            }
        }

        // Handle any remaining bits (if bitsLeft is 1, 2, 3, or 4)
        if (bitsLeft > 0)
        {
            // 1. Shift the remaining bits to the top of a 5-bit chunk.
            // 2. Mask with 0x1F to get the index.
            int index = (buffer << (5 - bitsLeft)) & 0x1F;
            result.Append(AlphabetChars[index]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Decodes a Crockford's Base32 string back into its original byte array.
    /// </summary>
    /// <param name="s">The Base32-encoded string.</param>
    /// <returns>The decoded raw byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown if <paramref name="s"/> contains an invalid Base32 character.</exception>
    public static byte[] FromBase32String(string s)
    {
        ArgumentNullException.ThrowIfNull(s);

        // Pre-calculate capacity: Each char is 5 bits, each byte is 8 bits.
        // (Length * 5) / 8. The +7 is to round up.
        var result = new MemoryStream((s.Length * 5 + 7) / 8);

        int buffer = 0;   // A bit buffer to hold bits read from the input
        int bitsLeft = 0; // The number of valid bits currently in the buffer

        foreach (char c in s)
        {
            // Look up the character in the reverse alphabet.
            // Check for invalid characters (out of range or marked as 0xFF).
            if (c >= ReverseAlphabet.Length || ReverseAlphabet[c] == 0xFF)
            {
                throw new FormatException($"Invalid Base32 character: '{c}'");
            }

            // Shift the existing buffer left by 5 bits and add the 5-bit value
            buffer = (buffer << 5) | ReverseAlphabet[c];
            bitsLeft += 5;

            // Process all 8-bit (byte) chunks currently available
            if (bitsLeft >= 8)
            {
                // Get the top 8 bits (a full byte):
                // 1. Shift the buffer right so the 8 bits are at the bottom.
                // 2. No mask is needed as WriteByte() only takes the bottom 8 bits.
                result.WriteByte((byte)(buffer >> (bitsLeft - 8)));

                // 4. Remove the 8 bits we just processed
                bitsLeft -= 8;
            }
        }

        // Any remaining bits in the buffer are padding and are discarded.
        return result.ToArray();
    }
}
