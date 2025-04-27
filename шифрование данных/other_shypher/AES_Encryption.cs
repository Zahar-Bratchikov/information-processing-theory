using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionSample
{
    public class AESEncryption
    {
        /// <summary>
        /// Зашифровывает строку с помощью AES
        /// </summary>
        /// <param name="plainText">Исходный текст</param>
        /// <param name="key">Ключ шифрования (должен быть 16, 24 или 32 байта)</param>
        /// <param name="iv">Вектор инициализации (должен быть 16 байт)</param>
        /// <returns>Зашифрованный текст в виде строки Base64</returns>
        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] encrypted;
            
            // Выводим исходный текст в байтах для демонстрации
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Console.WriteLine("\nПРОМЕЖУТОЧНЫЕ ШАГИ ШИФРОВАНИЯ:");
            Console.WriteLine($"1. Исходный текст в байтах: {BitConverter.ToString(plainTextBytes).Replace("-", " ")}");
            Console.WriteLine($"2. Размер исходного текста: {plainTextBytes.Length} байт");
            Console.WriteLine($"3. Ключ шифрования: {BitConverter.ToString(key).Replace("-", " ")}");
            Console.WriteLine($"4. Вектор инициализации: {BitConverter.ToString(iv).Replace("-", " ")}");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                
                // Выводим параметры AES
                Console.WriteLine($"5. Режим шифрования: {aesAlg.Mode}");
                Console.WriteLine($"6. Режим дополнения: {aesAlg.Padding}");
                Console.WriteLine($"7. Размер блока: {aesAlg.BlockSize} бит");
                Console.WriteLine($"8. Размер ключа: {aesAlg.KeySize} бит");

                // Создаем шифратор для выполнения преобразования потока
                Console.WriteLine("9. Создание шифратора...");
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Создаем потоки для шифрования
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Записываем данные в поток
                            Console.WriteLine("10. Запись данных в криптографический поток...");
                            swEncrypt.Write(plainText);
                        }
                        // При закрытии потока выполняется финальное дополнение блоков
                        Console.WriteLine("11. Завершение шифрования и получение зашифрованных данных...");
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            Console.WriteLine($"12. Зашифрованные данные в байтах: {BitConverter.ToString(encrypted).Replace("-", " ")}");
            Console.WriteLine($"13. Размер зашифрованных данных: {encrypted.Length} байт");
            
            // Конвертируем в строку base64 для удобства хранения и передачи
            string base64Result = Convert.ToBase64String(encrypted);
            Console.WriteLine($"14. Представление в Base64: {base64Result}");
            Console.WriteLine("\nРЕЗУЛЬТАТ ШИФРОВАНИЯ:");
            
            return base64Result;
        }

        /// <summary>
        /// Расшифровывает строку, зашифрованную с помощью AES
        /// </summary>
        /// <param name="cipherText">Зашифрованный текст в виде строки Base64</param>
        /// <param name="key">Ключ шифрования (должен быть 16, 24 или 32 байта)</param>
        /// <param name="iv">Вектор инициализации (должен быть 16 байт)</param>
        /// <returns>Расшифрованный текст</returns>
        public static string Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Декодируем из base64
            Console.WriteLine("\nПРОМЕЖУТОЧНЫЕ ШАГИ ДЕШИФРОВАНИЯ:");
            Console.WriteLine($"1. Зашифрованный текст в Base64: {cipherText}");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            Console.WriteLine($"2. Зашифрованные данные в байтах: {BitConverter.ToString(cipherBytes).Replace("-", " ")}");
            Console.WriteLine($"3. Размер зашифрованных данных: {cipherBytes.Length} байт");
            Console.WriteLine($"4. Используемый ключ: {BitConverter.ToString(key).Replace("-", " ")}");
            Console.WriteLine($"5. Используемый вектор инициализации: {BitConverter.ToString(iv).Replace("-", " ")}");
            
            string plaintext = string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                
                // Выводим параметры AES
                Console.WriteLine($"6. Режим шифрования: {aesAlg.Mode}");
                Console.WriteLine($"7. Режим дополнения: {aesAlg.Padding}");

                // Создаем дешифратор для выполнения преобразования потока
                Console.WriteLine("8. Создание дешифратора...");
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Создаем потоки для дешифрования
                Console.WriteLine("9. Создание потоков и начало процесса дешифрования...");
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Читаем расшифрованные данные из потока
                            Console.WriteLine("10. Чтение расшифрованных данных...");
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            byte[] decryptedBytes = Encoding.UTF8.GetBytes(plaintext);
            Console.WriteLine($"11. Расшифрованные данные в байтах: {BitConverter.ToString(decryptedBytes).Replace("-", " ")}");
            Console.WriteLine($"12. Размер расшифрованных данных: {decryptedBytes.Length} байт");
            Console.WriteLine("\nРЕЗУЛЬТАТ ДЕШИФРОВАНИЯ:");
            
            return plaintext;
        }

        /// <summary>
        /// Генерирует случайный ключ нужного размера для AES
        /// </summary>
        /// <param name="keySize">Размер ключа в битах (128, 192 или 256)</param>
        /// <returns>Случайный ключ</returns>
        public static byte[] GenerateRandomKey(int keySize = 256)
        {
            if (keySize != 128 && keySize != 192 && keySize != 256)
                throw new ArgumentException("Размер ключа должен быть 128, 192 или 256 бит", nameof(keySize));

            byte[] key = new byte[keySize / 8];
            RandomNumberGenerator.Fill(key);
            return key;
        }

        /// <summary>
        /// Генерирует случайный вектор инициализации для AES
        /// </summary>
        /// <returns>Случайный IV размером 16 байт</returns>
        public static byte[] GenerateRandomIV()
        {
            byte[] iv = new byte[16]; // В AES всегда 16 байт
            RandomNumberGenerator.Fill(iv);
            return iv;
        }
    }

    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("=== AES (Advanced Encryption Standard) ===");
                Console.WriteLine("AES - симметричный блочный шифр, принятый в качестве стандарта шифрования правительством США.");
                Console.WriteLine("Особенности AES:");
                Console.WriteLine("- Блочный шифр с размером блока 128 бит (16 байт)");
                Console.WriteLine("- Поддерживаемые размеры ключа: 128, 192 или 256 бит");
                Console.WriteLine("- Состоит из повторяющихся раундов преобразования");
                Console.WriteLine("- Каждый раунд включает: SubBytes, ShiftRows, MixColumns и AddRoundKey");
                Console.WriteLine("- Количество раундов зависит от размера ключа: 10 для 128-бит, 12 для 192-бит, 14 для 256-бит");
                Console.WriteLine("- Требует вектор инициализации (IV) размером 16 байт\n");
                
                // Генерируем ключ и вектор инициализации
                Console.WriteLine("Генерация параметров шифрования...");
                byte[] key = AESEncryption.GenerateRandomKey();
                byte[] iv = AESEncryption.GenerateRandomIV();

                // Запрашиваем ввод текста для шифрования
                Console.WriteLine("\nВведите текст для шифрования:");
                string originalText = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(originalText))
                {
                    Console.WriteLine("Вы не ввели текст. Будет использована пустая строка.");
                }

                Console.WriteLine($"Исходный текст: {originalText}");

                // Шифруем текст
                string encryptedText = AESEncryption.Encrypt(originalText, key, iv);
                Console.WriteLine($"Зашифрованный текст: {encryptedText}");

                // Расшифровываем текст
                string decryptedText = AESEncryption.Decrypt(encryptedText, key, iv);
                Console.WriteLine($"Расшифрованный текст: {decryptedText}");
                
                Console.WriteLine("\nПроцесс шифрования/дешифрования завершен успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите Enter для завершения...");
            Console.ReadLine();
        }
    }
} 