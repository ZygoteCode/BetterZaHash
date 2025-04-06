using System;

namespace BetterZaHash
{
    public class BetterZaHasher
    {
        private const ulong Prime1 = 11400714785074694791UL;
        private const ulong Prime2 = 14029467366897019727UL;
        private const ulong Prime3 = 1609587929392839161UL;
        private const int RotateAmount = 56;

        private static ulong RotateLeft(ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

        private static byte[] ComputeHashFirst(byte[] data)
        {
            ulong hashState = Prime3;

            foreach (byte b in data)
            {
                hashState += b;
                hashState *= Prime1;
                hashState = RotateLeft(hashState, RotateAmount);
                hashState ^= (hashState >> 27);
            }

            hashState ^= (hashState >> 33);
            hashState *= Prime2;
            hashState ^= (hashState >> 29);
            hashState *= Prime3;
            hashState ^= (hashState >> 32);

            return BitConverter.GetBytes(hashState);
        }

        public static byte[] ComputeHash(byte[] data)
        {
            ulong hashState = Prime3;

            for (int i = 0; i < data.Length; i++)
            {
                hashState += (ulong)data[i] * Prime1;
                hashState = RotateLeft(hashState, i % 64);
                hashState ^= hashState >> 31;
            }

            hashState ^= (ulong)data.Length * Prime2;

            hashState ^= (hashState >> 33);
            hashState *= Prime2;
            hashState ^= (hashState >> 29);
            hashState *= Prime3;
            hashState ^= (hashState >> 32);

            return Combine(BitConverter.GetBytes(hashState), ComputeHashFirst(data));
        }
    }
}