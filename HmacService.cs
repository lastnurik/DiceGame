using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace DiceGame
{
    public class HmacService
    {
        public byte[] GenerateSecureKey(int byteLength = 32)
        {
            byte[] key = new byte[byteLength];
            RandomNumberGenerator.Fill(key);
            return key;
        }

        public int GenerateSecureRandomInt(int min, int maxInclusive)
        {
            return RandomNumberGenerator.GetInt32(min, maxInclusive + 1);
        }

        public string ComputeHmacSha3(byte[] key, byte[] messageBytes)
        {
            var hmac = new HMac(new Sha3Digest(256));
            hmac.Init(new KeyParameter(key));

            hmac.BlockUpdate(messageBytes, 0, messageBytes.Length);

            byte[] result = new byte[hmac.GetMacSize()];
            hmac.DoFinal(result, 0);
            return ByteArrayToHex(result);
        }

        public byte[] IntToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
        }
    }
}