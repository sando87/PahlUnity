using System;
using System.Collections.Generic;

namespace PahlUnity
{
    public class MyDecoding
    {
        public static string Encrypt(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] encoded = Encrypt(bytes);
            return Convert.ToBase64String(encoded);
        }
        private static byte[] Encrypt(byte[] input)
        {
            byte[] output = new byte[input.Length];
            byte key = 0x4f;
            output[0] = (byte)(key ^ input[0]);
            for (int i = 1; i < input.Length; ++i)
            {
                output[i] = (byte)(output[i - 1] ^ (key ^ input[i]));
            }
            return output;
        }
        public static string Decrypt(string data)
        {
            byte[] encoded = Convert.FromBase64String(data);
            byte[] decodedData = Decrypt(encoded);
            return System.Text.Encoding.UTF8.GetString(decodedData);
        }
        public static byte[] Decrypt(byte[] input)
        {
            byte[] output = new byte[input.Length];
            byte key = 0x4f;
            for (int i = input.Length - 1; i > 0; --i)
            {
                output[i] = (byte)((input[i] ^ input[i - 1]) ^ key);
            }
            output[0] = (byte)(key ^ input[0]);
            return output;
        }

        // public static byte[] HexStringToByteArray(string hex)
        // {
        //     int hexFactor = 16; //string format is hex
        //     List<byte> rets = new List<byte>();
        //     for (int i = 0; i < hex.Length; i += 2)
        //     {
        //         rets.Add(Convert.ToByte(hex.Substring(i, 2), hexFactor));
        //     }
        //     return rets.ToArray();
        // }
        // public static string ByteArrayToHexString(byte[] data)
        // {
        //     return BitConverter.ToString(data).Replace("-", string.Empty);
        // }
    }
}