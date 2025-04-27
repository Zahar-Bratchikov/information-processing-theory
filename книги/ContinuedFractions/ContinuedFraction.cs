using System;
using System.Collections.Generic;
using System.Numerics;

namespace ContinuedFractions
{
    /// <summary>
    /// Класс для работы с конечными цепными дробями вида [a0; a1, a2, ..., an]
    /// Цепная дробь представляет собой выражение вида a0 + 1/(a1 + 1/(a2 + 1/(...+ 1/an)...))
    /// </summary>
    public class ContinuedFraction
    {
        // Элементы цепной дроби a0, a1, ..., an
        // a0 - целая часть, a1...an - частные неполные частные
        private readonly int[] _elements;

        /// <summary>
        /// Создает цепную дробь [a0; a1, ..., an]
        /// </summary>
        /// <param name="elements">Элементы цепной дроби: a0 - целая часть, a1...an - неполные частные</param>
        /// <exception cref="ArgumentException">Выбрасывается, если не указан хотя бы один элемент</exception>
        public ContinuedFraction(params int[] elements)
        {
            if (elements == null || elements.Length == 0)
                throw new ArgumentException("Должен быть указан хотя бы один элемент");

            _elements = elements;
        }

        /// <summary>
        /// Получает элементы цепной дроби в виде коллекции только для чтения
        /// </summary>
        public IReadOnlyList<int> Elements => _elements;

        /// <summary>
        /// Преобразует цепную дробь в рациональное число (обыкновенную дробь)
        /// Использует алгоритм "сворачивания" цепной дроби в обыкновенную
        /// </summary>
        /// <returns>Рациональное число в виде пары (числитель, знаменатель)</returns>
        public (BigInteger Numerator, BigInteger Denominator) ToRational()
        {
            // Инициализируем начальные значения для вычисления
            // На последнем шаге инициализируем дробь как an/1
            BigInteger numerator = _elements[_elements.Length - 1];
            BigInteger denominator = 1;

            // Вычисляем дробь, начиная с последнего элемента и двигаясь к первому
            // На каждом шаге выполняем операцию перевернутой дроби и добавления ai
            // Формула: ai + 1/предыдущее_значение
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
        /// <returns>Приближенное десятичное значение цепной дроби</returns>
        public double ToDouble()
        {
            var rational = ToRational();
            return (double)rational.Numerator / (double)rational.Denominator;
        }

        /// <summary>
        /// Создает цепную дробь из вещественного числа с указанной точностью
        /// Преобразует вещественное число в последовательность целых чисел
        /// </summary>
        /// <param name="value">Вещественное число</param>
        /// <param name="maxElements">Максимальное количество элементов (точность представления)</param>
        /// <returns>Конечная цепная дробь, аппроксимирующая вещественное число</returns>
        public static ContinuedFraction FromDouble(double value, int maxElements = 10)
        {
            List<int> elements = new List<int>();
            
            // Извлекаем целую часть числа (первый элемент цепной дроби a0)
            int intPart = (int)Math.Floor(value);
            elements.Add(intPart);

            // Вычисляем дробную часть числа
            double fractionalPart = value - intPart;
            
            // Последовательно извлекаем элементы цепной дроби
            for (int i = 1; i < maxElements && Math.Abs(fractionalPart) > 1e-10; i++)
            {
                // Переворачиваем дробную часть (берем обратное число)
                double reciprocal = 1 / fractionalPart;
                
                // Извлекаем целую часть (следующий элемент цепной дроби ai)
                intPart = (int)Math.Floor(reciprocal);
                elements.Add(intPart);
                
                // Обновляем дробную часть для следующей итерации
                fractionalPart = reciprocal - intPart;
            }

            return new ContinuedFraction(elements.ToArray());
        }

        /// <summary>
        /// Возвращает строковое представление цепной дроби в формате [a0; a1, a2, ..., an]
        /// </summary>
        /// <returns>Строковое представление цепной дроби</returns>
        public override string ToString()
        {
            return $"[{string.Join("; ", _elements)}]";
        }
    }
} 