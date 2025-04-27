using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ErrorResistantCodingApp
{
    /// <summary>
    /// Главное окно приложения, реализующее логику помехоустойчивого кодирования
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Путь к выбранному пользователем файлу
        /// </summary>
        private string selectedFilePath = string.Empty;

        /// <summary>
        /// Конструктор класса MainWindow
        /// Инициализирует компоненты пользовательского интерфейса
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку выбора файла
        /// Открывает диалог выбора файла и сохраняет путь к выбранному файлу
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                txtSelectedFile.Text = $"Выбранный файл: {selectedFilePath}";
                txtStatus.Text = "Статус: Файл выбран";
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку кодирования файла
        /// Выполняет кодирование выбранного файла, используя повторный код (3x)
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        private void btnEncodeFile_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия выбранного файла
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Пожалуйста, выберите существующий файл.");
                return;
            }

            try
            {
                // Чтение всех байтов файла
                byte[] fileBytes = File.ReadAllBytes(selectedFilePath);

                // Преобразуем байты в последовательность битов
                List<int> bits = BytesToBits(fileBytes);

                // Кодирование с использованием повторного кода (каждый бит повторяется трижды)
                List<int> encodedBits = EncodeRepetition(bits);

                // Преобразуем закодированные биты обратно в массив байтов
                byte[] encodedBytes = BitsToBytes(encodedBits);

                // Диалог для сохранения закодированного файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Сохранить закодированный файл";
                saveFileDialog.Filter = "Все файлы (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, encodedBytes);
                    txtStatus.Text = $"Статус: Файл успешно закодирован и сохранен по адресу: {saveFileDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                txtStatus.Text = "Статус: Ошибка во время кодирования";
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку декодирования файла
        /// Выполняет декодирование закодированного файла с восстановлением исходных данных
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        private void btnDecodeFile_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия выбранного файла
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Пожалуйста, выберите существующий файл для декодирования.");
                return;
            }

            try
            {
                // Чтение закодированного файла
                byte[] fileBytes = File.ReadAllBytes(selectedFilePath);

                // Преобразуем байты в последовательность битов
                List<int> bits = BytesToBits(fileBytes);

                // Декодирование повторного кода (каждый бит восстановлен по правилу большинства)
                List<int> decodedBits = DecodeRepetition(bits);

                // Преобразуем декодированные биты обратно в массив байтов
                byte[] decodedBytes = BitsToBytes(decodedBits);

                // Диалог для сохранения декодированного файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Сохранить декодированный файл";
                saveFileDialog.Filter = "Все файлы (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, decodedBytes);
                    txtStatus.Text = $"Статус: Файл успешно декодирован и сохранен по адресу: {saveFileDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                txtStatus.Text = "Статус: Ошибка во время декодирования";
            }
        }

        /// <summary>
        /// Преобразует массив байтов в список битов (0 и 1)
        /// </summary>
        /// <param name="bytes">Массив байтов для преобразования</param>
        /// <returns>Список, содержащий биты (0 и 1)</returns>
        private List<int> BytesToBits(byte[] bytes)
        {
            List<int> bits = new List<int>();
            foreach (byte b in bytes)
            {
                // Преобразование байта в 8-битную строку с ведущими нулями
                string binary = Convert.ToString(b, 2).PadLeft(8, '0');
                // Преобразование каждого символа в бит (0 или 1)
                foreach (char bit in binary)
                {
                    bits.Add(bit == '1' ? 1 : 0);
                }
            }
            return bits;
        }

        /// <summary>
        /// Преобразует список битов обратно в массив байтов
        /// </summary>
        /// <param name="bits">Список битов для преобразования</param>
        /// <returns>Массив байтов, полученный из списка битов</returns>
        private byte[] BitsToBytes(List<int> bits)
        {
            // Убираем лишние биты, если их количество не кратно 8
            int remainder = bits.Count % 8;
            if (remainder != 0)
            {
                bits = bits.Take(bits.Count - remainder).ToList();
            }
            
            List<byte> bytes = new List<byte>();
            // Группируем биты по 8 и преобразуем каждую группу в байт
            for (int i = 0; i < bits.Count; i += 8)
            {
                string byteStr = string.Join("", bits.Skip(i).Take(8));
                bytes.Add(Convert.ToByte(byteStr, 2));
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Кодирует данные с использованием повторного кода (3x)
        /// Каждый бит повторяется 3 раза для повышения устойчивости к ошибкам
        /// </summary>
        /// <param name="dataBits">Исходный список битов</param>
        /// <returns>Закодированный список битов, где каждый бит повторен трижды</returns>
        private List<int> EncodeRepetition(List<int> dataBits)
        {
            List<int> encoded = new List<int>();
            foreach (int bit in dataBits)
            {
                // Повторяем каждый бит трижды
                encoded.Add(bit);
                encoded.Add(bit);
                encoded.Add(bit);
            }
            return encoded;
        }

        /// <summary>
        /// Декодирует данные, закодированные повторным кодом (3x)
        /// Использует правило большинства: если в группе из 3 бит больше единиц, то результат 1, иначе 0
        /// </summary>
        /// <param name="receivedBits">Закодированный список битов</param>
        /// <returns>Декодированный список битов</returns>
        private List<int> DecodeRepetition(List<int> receivedBits)
        {
            List<int> decoded = new List<int>();
            // Группируем биты по 3 и применяем правило большинства
            for (int i = 0; i < receivedBits.Count; i += 3)
            {
                int countOnes = 0;
                int countZeros = 0;
                // Подсчитываем количество единиц и нулей в группе из 3 бит
                for (int j = 0; j < 3; j++)
                {
                    if (i + j < receivedBits.Count) // Защита от выхода за границы списка
                    {
                        if (receivedBits[i + j] == 1)
                            countOnes++;
                        else
                            countZeros++;
                    }
                }
                // Применяем правило большинства
                decoded.Add(countOnes > countZeros ? 1 : 0);
            }
            return decoded;
        }
    }
}