using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ErrorResistantCodingApp
{
    public partial class MainWindow : Window
    {
        private string selectedFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик для выбора файла
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

        // Обработчик для кодирования файла
        private void btnEncodeFile_Click(object sender, RoutedEventArgs e)
        {
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

        // Обработчик для декодирования файла
        private void btnDecodeFile_Click(object sender, RoutedEventArgs e)
        {
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

        // Метод для преобразования массива байтов в последовательность битов
        private List<int> BytesToBits(byte[] bytes)
        {
            List<int> bits = new List<int>();
            foreach (byte b in bytes)
            {
                string binary = Convert.ToString(b, 2).PadLeft(8, '0');
                foreach (char bit in binary)
                {
                    bits.Add(bit == '1' ? 1 : 0);
                }
            }
            return bits;
        }

        // Метод для преобразования последовательности битов в массив байтов
        private byte[] BitsToBytes(List<int> bits)
        {
            int remainder = bits.Count % 8;
            if (remainder != 0)
            {
                bits = bits.Take(bits.Count - remainder).ToList();
            }
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < bits.Count; i += 8)
            {
                string byteStr = string.Join("", bits.Skip(i).Take(8));
                bytes.Add(Convert.ToByte(byteStr, 2));
            }
            return bytes.ToArray();
        }

        // Метод для кодирования повторным кодом (Repetition Code) с коэффициентом повторения 3
        // Каждый бит исходного сообщения повторяется трижды для повышения устойчивости к ошибкам.
        private List<int> EncodeRepetition(List<int> dataBits)
        {
            List<int> encoded = new List<int>();
            foreach (int bit in dataBits)
            {
                encoded.Add(bit);
                encoded.Add(bit);
                encoded.Add(bit);
            }
            return encoded;
        }

        // Метод для декодирования повторного кода (Repetition Code) с коэффициентом повторения 3
        // Декодирование осуществляется с использованием правила большинства: для каждой группы из 3 бит возвращается значение,
        // которое встречается чаще (если количество 1 больше, чем 0, то 1, иначе 0).
        private List<int> DecodeRepetition(List<int> receivedBits)
        {
            List<int> decoded = new List<int>();
            for (int i = 0; i < receivedBits.Count; i += 3)
            {
                int countOnes = 0;
                int countZeros = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (receivedBits[i + j] == 1)
                        countOnes++;
                    else
                        countZeros++;
                }
                decoded.Add(countOnes > countZeros ? 1 : 0);
            }
            return decoded;
        }
    }
}