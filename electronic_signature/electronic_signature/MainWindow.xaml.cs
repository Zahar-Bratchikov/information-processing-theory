using Microsoft.Win32;
using System;
using System.Numerics;
using System.Windows;

namespace DigitalSignature
{
    public partial class MainWindow : Window
    {
        private RSAHelper rsaHelper;
        private BigInteger currentSignature;

        public MainWindow()
        {
            InitializeComponent();
            rsaHelper = new RSAHelper();
            UpdateKeyInfo();
            btnSaveSignature.IsEnabled = false;
        }

        private void UpdateKeyInfo()
        {
            txtPublicKey.Text = $"e = {rsaHelper.PublicKey.e}\nn = {rsaHelper.PublicKey.n}";
            txtPrivateKey.Text = $"d = {rsaHelper.PrivateKey.d}\nn = {rsaHelper.PrivateKey.n}";
        }

        private void btnGenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rsaHelper = new RSAHelper();
                UpdateKeyInfo();
                txtStatus.Text = "Новая пара ключей успешно сгенерирована";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации ключей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при генерации ключей";
            }
        }

        private void btnSavePublicKey_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Сохранить публичный ключ", FileName = "public_key.txt" };
            if (dlg.ShowDialog() == true)
            {
                try { rsaHelper.SavePublicKey(dlg.FileName); txtStatus.Text = $"Публичный ключ сохранен в {dlg.FileName}"; }
                catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении публичного ключа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); txtStatus.Text = "Ошибка при сохранении публичного ключа"; }
            }
        }

        private void btnSavePrivateKey_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Приватный ключ должен храниться в надежном месте и никогда не передаваться третьим лицам.\n\nВы уверены, что хотите сохранить приватный ключ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var dlg = new SaveFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Сохранить приватный ключ", FileName = "private_key.txt" };
                if (dlg.ShowDialog() == true)
                {
                    try { rsaHelper.SavePrivateKey(dlg.FileName); txtStatus.Text = $"Приватный ключ сохранен в {dlg.FileName}"; }
                    catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении приватного ключа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); txtStatus.Text = "Ошибка при сохранении приватного ключа"; }
                }
            }
        }

        private void btnLoadPublicKey_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Загрузить публичный ключ" };
            if (dlg.ShowDialog() == true)
            {
                try { rsaHelper.LoadPublicKey(dlg.FileName); UpdateKeyInfo(); txtStatus.Text = $"Публичный ключ загружен из {dlg.FileName}"; }
                catch (Exception ex) { MessageBox.Show($"Ошибка при загрузке публичного ключа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); txtStatus.Text = "Ошибка при загрузке публичного ключа"; }
            }
        }

        private void btnLoadPrivateKey_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Загрузить приватный ключ" };
            if (dlg.ShowDialog() == true)
            {
                try { rsaHelper.LoadPrivateKey(dlg.FileName); UpdateKeyInfo(); txtStatus.Text = $"Приватный ключ загружен из {dlg.FileName}"; }
                catch (Exception ex) { MessageBox.Show($"Ошибка при загрузке приватного ключа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); txtStatus.Text = "Ошибка при загрузке приватного ключа"; }
            }
        }

        private void btnSelectFileToSign_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Все файлы (*.*)|*.*", Title = "Выбрать файл для подписи" };
            if (dlg.ShowDialog() == true)
            {
                txtSignFilePath.Text = dlg.FileName;
                txtStatus.Text = $"Выбран файл для подписи: {dlg.FileName}";
            }
        }

        private void btnSignFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSignFilePath.Text))
            {
                MessageBox.Show("Пожалуйста, выберите файл для подписи", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                currentSignature = rsaHelper.SignFile(txtSignFilePath.Text);
                txtSignature.Text = currentSignature.ToString();
                btnSaveSignature.IsEnabled = true;
                txtStatus.Text = "Файл успешно подписан";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подписании файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при подписании файла";
            }
        }

        private void btnSaveSignature_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Файлы подписи (*.sig)|*.sig|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Сохранить цифровую подпись", FileName = "signature.sig" };
            if (dlg.ShowDialog() == true)
            {
                try { rsaHelper.SaveSignature(currentSignature, dlg.FileName); txtStatus.Text = $"Подпись сохранена в {dlg.FileName}"; }
                catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении подписи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); txtStatus.Text = "Ошибка при сохранении подписи"; }
            }
        }

        private void btnSelectFileToVerify_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Все файлы (*.*)|*.*", Title = "Выбрать файл для проверки" };
            if (dlg.ShowDialog() == true)
            {
                txtVerifyFilePath.Text = dlg.FileName;
                txtStatus.Text = $"Выбран файл для проверки: {dlg.FileName}";
            }
        }

        private void btnSelectSignatureFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Файлы подписи (*.sig)|*.sig|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", Title = "Выбрать файл подписи" };
            if (dlg.ShowDialog() == true)
            {
                txtSignatureFilePath.Text = dlg.FileName;
                txtStatus.Text = $"Выбран файл подписи: {dlg.FileName}";
            }
        }

        private void btnVerifySignature_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtVerifyFilePath.Text))
            {
                MessageBox.Show("Пожалуйста, выберите файл для проверки", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtSignatureFilePath.Text))
            {
                MessageBox.Show("Пожалуйста, выберите файл подписи", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                BigInteger signature = rsaHelper.LoadSignature(txtSignatureFilePath.Text);
                bool isValid = rsaHelper.VerifySignature(txtVerifyFilePath.Text, signature);
                if (isValid)
                {
                    txtVerificationResult.Text = "Подпись верна";
                    txtVerificationResult.Foreground = System.Windows.Media.Brushes.Green;
                    txtStatus.Text = "Подпись успешно проверена и является действительной";
                }
                else
                {
                    txtVerificationResult.Text = "Подпись не верна";
                    txtVerificationResult.Foreground = System.Windows.Media.Brushes.Red;
                    txtStatus.Text = "Подпись недействительна или была изменена";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке подписи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при проверке подписи";
                txtVerificationResult.Text = "Ошибка проверки";
                txtVerificationResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}