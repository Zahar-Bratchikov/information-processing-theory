using System;
using System.Text;

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

            // Дробная часть сначала имеет высоту 3 строки
            int height = 3;
            
            // Каждая следующая вложенная дробь добавляет 2 строки
            for (int i = 2; i < elements.Count; i++)
            {
                height += 2;
            }

            // Создаем двумерный массив символов для представления визуализации
            char[,] visualization = new char[height, 80];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < 80; j++)
                {
                    visualization[i, j] = ' ';
                }
            }

            // Заполняем массив
            int currentLine = 0;
            int offset = 0;

            // Первый элемент
            string a0 = elements[0].ToString();
            for (int i = 0; i < a0.Length; i++)
            {
                visualization[currentLine, offset + i] = a0[i];
            }
            offset += a0.Length;

            if (elements.Count > 1)
            {
                visualization[currentLine, offset] = '+';
                offset += 2;

                // Рисуем дроби
                DrawFraction(visualization, elements, 1, currentLine, offset);
            }

            // Преобразуем массив в строку
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < 80; j++)
                {
                    sb.Append(visualization[i, j]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static int DrawFraction(char[,] visualization, System.Collections.Generic.IReadOnlyList<int> elements, int index, int line, int offset)
        {
            // Рисуем числитель (всегда 1)
            visualization[line, offset] = '1';
            
            // Определяем ширину знаменателя
            string denominator = elements[index].ToString();
            int denomWidth = denominator.Length;
            
            if (index < elements.Count - 1)
            {
                // Если это не последний элемент, нужно добавить "+ ..."
                denomWidth += 2; // Учитываем " +"
            }
            
            // Рисуем линию дроби
            int fractionLineLength = Math.Max(1, denomWidth);
            for (int i = 0; i < fractionLineLength; i++)
            {
                visualization[line + 1, offset + i] = '-';
            }
            
            // Рисуем знаменатель
            for (int i = 0; i < denominator.Length; i++)
            {
                visualization[line + 2, offset + i] = denominator[i];
            }
            
            // Если есть еще элементы, рисуем "+" и вызываем рекурсивно
            if (index < elements.Count - 1)
            {
                visualization[line + 2, offset + denominator.Length] = '+';
                visualization[line + 2, offset + denominator.Length + 1] = ' ';
                
                // Вызываем рекурсивно для следующего элемента, со сдвигом на 2 строки вниз
                DrawFraction(visualization, elements, index + 1, line + 2, offset + denominator.Length + 2);
            }
            
            return fractionLineLength;
        }
    }
} 