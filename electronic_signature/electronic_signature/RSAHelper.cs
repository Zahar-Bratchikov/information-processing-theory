using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace DigitalSignature
{
    public class RSAHelper
    {
        private (BigInteger e, BigInteger n) publicKey;
        private (BigInteger d, BigInteger n) privateKey;

        public (BigInteger e, BigInteger n) PublicKey => publicKey;
        public (BigInteger d, BigInteger n) PrivateKey => privateKey;

        public RSAHelper()
        {
            var keys = GenerateKeys();
            publicKey = keys.Item1;
            privateKey = keys.Item2;
        }

        public void SetPublicKey(BigInteger e, BigInteger n) => publicKey = (e, n);
        public void SetPrivateKey(BigInteger d, BigInteger n) => privateKey = (d, n);

        private BigInteger GeneratePrime(int start = 10000, int end = 100000, Random rnd = null)
        {
            if (rnd == null) rnd = new Random();
            while (true)
            {
                int candidate = rnd.Next(start, end);
                if (IsPrime(candidate)) return candidate;
            }
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            int bound = (int)Math.Sqrt(number);
            for (int i = 3; i <= bound; i += 2)
                if (number % i == 0) return false;
            return true;
        }

        private BigInteger ModInverse(BigInteger a, BigInteger m)
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

        private Tuple<(BigInteger, BigInteger), (BigInteger, BigInteger)> GenerateKeys()
        {
            BigInteger p, q, n, phi_n, e, d;
            Random rnd = new Random();
            do
            {
                do
                {
                    p = GeneratePrime(10000, 100000, rnd);
                    q = GeneratePrime(10000, 100000, rnd);
                } while (p == q);
                n = p * q;
                phi_n = (p - 1) * (q - 1);
                e = 65537;
            } while (BigInteger.GreatestCommonDivisor(e, phi_n) != 1 || e >= phi_n);

            d = ModInverse(e, phi_n);
            return Tuple.Create((e, n), (d, n));
        }

        public byte[] ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
                return sha256.ComputeHash(stream);
        }

        // Критически важно: одинаковое формирование BigInteger из хеша!
        private static BigInteger HashToBigInteger(byte[] hash)
        {
            byte[] unsigned = new byte[hash.Length + 1];
            Array.Copy(hash, 0, unsigned, 1, hash.Length);
            Array.Reverse(unsigned); // Теперь big-endian => little-endian
            return new BigInteger(unsigned);
        }

        public BigInteger SignFile(string filePath)
        {
            byte[] hash = ComputeFileHash(filePath);
            BigInteger hashValue = HashToBigInteger(hash);
            return BigInteger.ModPow(hashValue, privateKey.d, privateKey.n);
        }

        public bool VerifySignature(string filePath, BigInteger signature)
        {
            byte[] hash = ComputeFileHash(filePath);
            BigInteger hashValue = HashToBigInteger(hash);
            BigInteger decryptedHash = BigInteger.ModPow(signature, publicKey.e, publicKey.n);
            return decryptedHash == hashValue;
        }

        public void SavePublicKey(string filePath)
        {
            File.WriteAllText(filePath, $"{publicKey.e}\n{publicKey.n}");
        }

        public void LoadPublicKey(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) throw new Exception("Некорректный формат файла публичного ключа");
            var e = BigInteger.Parse(lines[0]);
            var n = BigInteger.Parse(lines[1]);
            SetPublicKey(e, n);
        }

        public void SavePrivateKey(string filePath)
        {
            File.WriteAllText(filePath, $"{privateKey.d}\n{privateKey.n}");
        }

        public void LoadPrivateKey(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) throw new Exception("Некорректный формат файла приватного ключа");
            var d = BigInteger.Parse(lines[0]);
            var n = BigInteger.Parse(lines[1]);
            SetPrivateKey(d, n);
        }

        public void SaveSignature(BigInteger signature, string filePath)
        {
            File.WriteAllText(filePath, signature.ToString());
        }

        public BigInteger LoadSignature(string filePath)
        {
            string signatureStr = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(signatureStr)) throw new Exception("Файл подписи пустой или поврежден");
            return BigInteger.Parse(signatureStr);
        }
    }
}