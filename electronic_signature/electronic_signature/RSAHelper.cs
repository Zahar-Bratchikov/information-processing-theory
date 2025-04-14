using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

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
            Debug.WriteLine($"[RSAHelper] Generated keys - Public: (e: {publicKey.e}, n: {publicKey.n}), Private: (d: {privateKey.d}, n: {privateKey.n})");
        }

        public void SetPublicKey(BigInteger e, BigInteger n)
        {
            publicKey = (e, n);
            Debug.WriteLine($"[SetPublicKey] Set public key to: (e: {e}, n: {n})");
        }

        public void SetPrivateKey(BigInteger d, BigInteger n)
        {
            privateKey = (d, n);
            Debug.WriteLine($"[SetPrivateKey] Set private key to: (d: {d}, n: {n})");
        }

        private BigInteger GeneratePrime(int start = 10000, int end = 100000, Random rnd = null)
        {
            if (rnd == null) rnd = new Random();
            while (true)
            {
                int candidate = rnd.Next(start, end);
                if (IsPrime(candidate))
                {
                    Debug.WriteLine($"[GeneratePrime] Candidate prime found: {candidate}");
                    return candidate;
                }
            }
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            int bound = (int)Math.Sqrt(number);
            for (int i = 3; i <= bound; i += 2)
            {
                if (number % i == 0) return false;
            }
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
            Debug.WriteLine($"[ModInverse] Computed modular inverse: {x1}");
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
                e = 65537; // Standard exponent
                Debug.WriteLine($"[GenerateKeys] p: {p}, q: {q}, n: {n}, phi(n): {phi_n}, chosen e: {e}");
            } while (BigInteger.GreatestCommonDivisor(e, phi_n) != 1 || e >= phi_n);

            d = ModInverse(e, phi_n);
            Debug.WriteLine($"[GenerateKeys] Final keys - Public: (e: {e}, n: {n}), Private: (d: {d}, n: {n})");
            return Tuple.Create((e, n), (d, n));
        }

        public byte[] ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                Debug.WriteLine($"[ComputeFileHash] File: {filePath}, Hash: {BitConverter.ToString(hash)}");
                return hash;
            }
        }

        // Converts the hash (big-endian) into a BigInteger (little-endian) with an extra zero to ensure positivity.
        public static BigInteger HashToBigInteger(byte[] hash)
        {
            Debug.WriteLine($"[HashToBigInteger] Original hash bytes: {BitConverter.ToString(hash)}");
            byte[] unsigned = new byte[hash.Length + 1];
            Array.Copy(hash, 0, unsigned, 1, hash.Length);
            Debug.WriteLine($"[HashToBigInteger] After prepending zero: {BitConverter.ToString(unsigned)}");
            Array.Reverse(unsigned);
            Debug.WriteLine($"[HashToBigInteger] Reversed bytes: {BitConverter.ToString(unsigned)}");
            BigInteger result = new BigInteger(unsigned);
            Debug.WriteLine($"[HashToBigInteger] Resulting BigInteger: {result}");
            return result;
        }

        public BigInteger SignFile(string filePath)
        {
            byte[] hash = ComputeFileHash(filePath);
            BigInteger hashValue = HashToBigInteger(hash);
            // Note: RSA operations work modulo n, so the actual signed value is (hashValue mod n)^d mod n.
            Debug.WriteLine($"[SignFile] Computed hashValue: {hashValue}");
            BigInteger signature = BigInteger.ModPow(hashValue, privateKey.d, privateKey.n);
            Debug.WriteLine($"[SignFile] Signature: {signature}");
            return signature;
        }

        public bool VerifySignature(string filePath, BigInteger signature)
        {
            byte[] hash = ComputeFileHash(filePath);
            BigInteger hashValue = HashToBigInteger(hash);
            // Reduce the hash modulo n because RSA works in the finite field Z/n.
            BigInteger m = hashValue % publicKey.n;
            BigInteger decryptedHash = BigInteger.ModPow(signature, publicKey.e, publicKey.n);
            Debug.WriteLine($"[VerifySignature] Computed hashValue: {hashValue}");
            Debug.WriteLine($"[VerifySignature] (hashValue mod n): {m}");
            Debug.WriteLine($"[VerifySignature] Decrypted hash from signature: {decryptedHash}");
            return decryptedHash == m;
        }

        public void SavePublicKey(string filePath)
        {
            File.WriteAllText(filePath, $"{publicKey.e}\n{publicKey.n}");
            Debug.WriteLine($"[SavePublicKey] Public key saved to {filePath}");
        }

        public void LoadPublicKey(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) throw new Exception("Некорректный формат файла публичного ключа");
            var e = BigInteger.Parse(lines[0]);
            var n = BigInteger.Parse(lines[1]);
            SetPublicKey(e, n);
            Debug.WriteLine($"[LoadPublicKey] Public key loaded from {filePath}");
        }

        public void SavePrivateKey(string filePath)
        {
            File.WriteAllText(filePath, $"{privateKey.d}\n{privateKey.n}");
            Debug.WriteLine($"[SavePrivateKey] Private key saved to {filePath}");
        }

        public void LoadPrivateKey(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) throw new Exception("Некорректный формат файла приватного ключа");
            var d = BigInteger.Parse(lines[0]);
            var n = BigInteger.Parse(lines[1]);
            SetPrivateKey(d, n);
            Debug.WriteLine($"[LoadPrivateKey] Private key loaded from {filePath}");
        }

        public void SaveSignature(BigInteger signature, string filePath)
        {
            File.WriteAllText(filePath, signature.ToString());
            Debug.WriteLine($"[SaveSignature] Signature saved to {filePath}");
        }

        public BigInteger LoadSignature(string filePath)
        {
            string signatureStr = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(signatureStr)) throw new Exception("Файл подписи пустой или поврежден");
            BigInteger signature = BigInteger.Parse(signatureStr);
            Debug.WriteLine($"[LoadSignature] Signature loaded from {filePath}: {signature}");
            return signature;
        }
    }
}