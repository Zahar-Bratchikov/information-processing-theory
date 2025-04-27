using System;
using System.Text;
using System.Collections.Generic;

namespace ContinuedFractions
{
    /// <summary>
    /// Вспомогательный класс для визуализации цепных дробей в консоли
    /// </summary>
    public static class ContinuedFractionVisualizer
    {
        /// <summary>
        /// Визуализирует конечную цепную дробь в виде многострочного выражения
        /// </summary>
        /// <param name="fraction">Цепная дробь</param>
        /// <returns>Строка с визуализацией цепной дроби</returns>
        public static string Visualize(ContinuedFraction fraction)
        {
            if (fraction == null)
                throw new ArgumentNullException(nameof(fraction));

            var elements = fraction.Elements;
            if (elements.Count == 1)
                return elements[0].ToString();

            StringBuilder sb = new StringBuilder();
            int height = 3 + (elements.Count - 2) * 2;
            char[,] visualization = new char[height, 80];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < 80; j++)
                    visualization[i, j] = ' ';

            int currentLine = 0;
            int offset = 0;

            // Первый элемент
            string a0 = elements[0].ToString();
            for (int i = 0; i < a0.Length; i++)
                visualization[currentLine, offset + i] = a0[i];
            offset += a0.Length;

            if (elements.Count > 1)
            {
                visualization[currentLine, offset] = '+';
                offset += 2;
                DrawFraction(visualization, elements, 1, currentLine, offset);
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < 80; j++)
                    sb.Append(visualization[i, j]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void DrawFraction(char[,] visualization, IReadOnlyList<int> elements, int index, int line, int offset)
        {
            visualization[line, offset] = '1';
            string denominator = elements[index].ToString();
            int denomWidth = denominator.Length + (index < elements.Count - 1 ? 2 : 0);

            for (int i = 0; i < denomWidth; i++)
                visualization[line + 1, offset + i] = '-';

            for (int i = 0; i < denominator.Length; i++)
                visualization[line + 2, offset + i] = denominator[i];

            if (index < elements.Count - 1)
            {
                visualization[line + 2, offset + denominator.Length] = '+';
                visualization[line + 2, offset + denominator.Length + 1] = ' ';
                DrawFraction(visualization, elements, index + 1, line + 2, offset + denominator.Length + 2);
            }
        }
    }
} 