using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ContinuedFractions
{
    /// <summary>
    /// Класс для работы с конечными цепными дробями
    /// </summary>
    public class ContinuedFraction
    {
        // Элементы цепной дроби a0, a1, ..., an
        private readonly int[] _elements;

        /// <summary>
        /// Создает цепную дробь [a0; a1, ..., an]
        /// </summary>
        /// <param name="elements">Элементы цепной дроби</param>
        public ContinuedFraction(params int[] elements)
        {
            if (elements == null || elements.Length == 0)
                throw new ArgumentException("Должен быть указан хотя бы один элемент");

            _elements = elements;
        }

        /// <summary>
        /// Получает элементы цепной дроби
        /// </summary>
        public IReadOnlyList<int> Elements => _elements;

        /// <summary>
        /// Преобразует цепную дробь в рациональное число (обыкновенную дробь)
        /// </summary>
        /// <returns>Рациональное число в виде пары (числитель, знаменатель)</returns>
        public (BigInteger Numerator, BigInteger Denominator) ToRational()
        {
            // Инициализируем начальные значения для вычисления
            BigInteger numerator = _elements[_elements.Length - 1];
            BigInteger denominator = 1;

            // Вычисляем дробь, начиная с последнего элемента и двигаясь к первому
            for (int i = _elements.Length - 2; i >= 0; i--)
            {
                BigInteger temp = numerator;
                numerator = _elements[i] * numerator + denominator;
                denominator = temp;
            }

            return (numerator, denominator);
        }

        /// <summary>
        /// Возвращает десятичное значение цепной дроби
        /// </summary>
        public double ToDouble()
        {
            var rational = ToRational();
            return (double)rational.Numerator / (double)rational.Denominator;
        }

        /// <summary>
        /// Создает цепную дробь на основе рационального числа
        /// </summary>
        /// <param name="numerator">Числитель</param>
        /// <param name="denominator">Знаменатель</param>
        /// <returns>Цепная дробь</returns>
        public static ContinuedFraction FromRational(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Знаменатель не может быть равен нулю");

            List<int> elements = new List<int>();

            // Алгоритм Евклида для получения цепной дроби
            while (denominator != 0)
            {
                BigInteger quotient = BigInteger.DivRem(numerator, denominator, out BigInteger remainder);
                elements.Add((int)quotient);
                numerator = denominator;
                denominator = remainder;
            }

            return new ContinuedFraction(elements.ToArray());
        }

        /// <summary>
        /// Создает цепную дробь из вещественного числа с указанной точностью
        /// </summary>
        /// <param name="value">Вещественное число</param>
        /// <param name="maxElements">Максимальное количество элементов</param>
        /// <returns>Цепная дробь</returns>
        public static ContinuedFraction FromDouble(double value, int maxElements = 10)
        {
            List<int> elements = new List<int>();
            
            // Извлекаем целую часть
            int intPart = (int)Math.Floor(value);
            elements.Add(intPart);

            // Вычисляем дробную часть
            double fractionalPart = value - intPart;
            
            for (int i = 1; i < maxElements && Math.Abs(fractionalPart) > 1e-10; i++)
            {
                // Переворачиваем дробную часть
                double reciprocal = 1 / fractionalPart;
                
                // Извлекаем целую часть
                intPart = (int)Math.Floor(reciprocal);
                elements.Add(intPart);
                
                // Обновляем дробную часть
                fractionalPart = reciprocal - intPart;
            }

            return new ContinuedFraction(elements.ToArray());
        }

        /// <summary>
        /// Возвращает строковое представление цепной дроби
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(_elements[0]);
            
            for (int i = 1; i < _elements.Length; i++)
            {
                sb.Append("; ");
                sb.Append(_elements[i]);
            }
            
            sb.Append(']');
            return sb.ToString();
        }
    }
} 