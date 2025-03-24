﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

class RSA
{
    static void Main()
    {
        // Генерируем ключи
        var keys = GenerateKeys();
        var publicKey = keys.Item1;
        var privateKey = keys.Item2;

        Console.Write("Введите сообщение для шифровки: ");
        string message = Console.ReadLine();  // Исходное сообщение
        var ciphertext = Encrypt(message, publicKey);  // Шифруем сообщение
        var decryptedMessage = Decrypt(ciphertext, privateKey);  // Расшифровываем сообщение

        // Выводим результаты
        Console.WriteLine("Оригинальное сообщение: " + message);
        Console.WriteLine("Зашифрованное сообщение: " + string.Join(", ", ciphertext));
        Console.WriteLine("Расшифрованное сообщение: " + decryptedMessage);
    }

    // Функция для генерации случайного простого числа в заданном диапазоне
    static BigInteger GeneratePrime(int start = 1000, int end = 5000)
    {
        Random random = new Random();
        while (true)
        {
            int num = random.Next(start, end);
            if (IsPrime(num))
            {
                return num;
            }
        }
    }

    // Проверка на простое число
    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    // Вычисление обратного по модулю (с помощью алгоритма Евклида https://habr.com/ru/articles/745820/)
    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m, t, q;
        BigInteger x0 = 0, x1 = 1;
        if (m == 1) return 0;
        while (a > 1)
        {
            q = a / m;
            t = m;
            m = a % m;
            a = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }
        if (x1 < 0) x1 += m0;
        return x1;
    }

    // Функция для генерации открытого и закрытого ключей RSA
    static Tuple<(BigInteger, BigInteger), (BigInteger, BigInteger)> GenerateKeys()
    {
        BigInteger p = GeneratePrime();
        BigInteger q = GeneratePrime();
        BigInteger n = p * q;
        BigInteger phi_n = (p - 1) * (q - 1);

        BigInteger e = 3;
        while (e < phi_n && BigInteger.GreatestCommonDivisor(e, phi_n) != 1)
        {
            e += 2;
        }

        BigInteger d = ModInverse(e, phi_n);

        return Tuple.Create((e, n), (d, n));
    }

    // Функция для шифрования сообщения
    static List<BigInteger> Encrypt(string message, (BigInteger e, BigInteger n) publicKey)
    {
        var encrypted = new List<BigInteger>();
        foreach (char c in message)
        {
            encrypted.Add(BigInteger.ModPow(c, publicKey.e, publicKey.n));
        }
        return encrypted;
    }

    // Функция для расшифрования сообщения
    static string Decrypt(List<BigInteger> ciphertext, (BigInteger d, BigInteger n) privateKey)
    {
        var decrypted = new List<char>();
        foreach (var c in ciphertext)
        {
            decrypted.Add((char)BigInteger.ModPow(c, privateKey.d, privateKey.n));
        }
        return new string(decrypted.ToArray());
    }
}