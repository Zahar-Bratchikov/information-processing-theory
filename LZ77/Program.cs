using System;
using System.Collections.Generic;
using System.Text;

class LZ77
{
    private const int WindowSize = 20; // Размер окна поиска
    private const int LookaheadBufferSize = 10; // Размер буфера предпросмотра

    public static List<(int, int, char)> Compress(string input)
    {
        List<(int, int, char)> compressed = new List<(int, int, char)>();
        int index = 0;

        while (index < input.Length)
        {
            int bestMatchDistance = 0, bestMatchLength = 0;
            string searchBuffer = input.Substring(Math.Max(0, index - WindowSize), Math.Min(index, WindowSize));

            for (int matchLength = 1; matchLength <= LookaheadBufferSize && index + matchLength <= input.Length; matchLength++)
            {
                string match = input.Substring(index, matchLength);
                int foundIndex = searchBuffer.LastIndexOf(match);
                if (foundIndex >= 0)
                {
                    bestMatchDistance = searchBuffer.Length - foundIndex;
                    bestMatchLength = matchLength;
                }
                else
                {
                    break;
                }
            }

            char nextChar = index + bestMatchLength < input.Length ? input[index + bestMatchLength] : '\0';
            compressed.Add((bestMatchDistance, bestMatchLength, nextChar));
            index += bestMatchLength + 1;
        }

        return compressed;
    }

    public static string Decompress(List<(int, int, char)> compressed)
    {
        StringBuilder decompressed = new StringBuilder();

        foreach (var (distance, length, nextChar) in compressed)
        {
            int start = decompressed.Length - distance;
            for (int i = 0; i < length; i++)
            {
                decompressed.Append(decompressed[start + i]);
            }
            if (nextChar != '\0')
                decompressed.Append(nextChar);
        }

        return decompressed.ToString();
    }

    static void Main()
    {
        string input = "early_to_bed_and_early_to_rise_makes_a_man_wise";
        var compressedData = Compress(input);
        Console.WriteLine("Compressed Data:");
        foreach (var entry in compressedData)
        {
            Console.WriteLine(entry);
        }

        string decompressed = Decompress(compressedData);
        Console.WriteLine("Decompressed String: " + decompressed);
    }
}
