using System;
using System.Collections;
using System.Collections.Generic;

namespace Binary
{
    /// <summary>
    /// Manages a compact array of bit values, which are represented as Booleans.
    /// </summary>
    public class BitArray : IEnumerable
    {
        /// <summary>
        /// Total Amount of Bits
        /// </summary>
        public int Count { get; internal set; }
        /// <summary>
        /// Total Amount of Bytes
        /// </summary>
        public int ByteCount { get => internalBits.Count; }

        private List<byte> internalBits;
        private int byteIndex; // currently active byte, increased every 8 bits
        private byte bitIndex; // currently active bit of a byte range[0..7]
        
        /// <summary>
        /// Create BitArray
        /// </summary>
        public BitArray()
        {
            internalBits = new List<byte>();
            internalBits.Add(0);
            bitIndex = 0;
        }

        /// <summary>
        /// Create BitArray
        /// </summary>
        /// <param name="bits">Given bits</param>
        public BitArray(IEnumerable<bool> bits)
        {
            internalBits = new List<byte>();
            internalBits.Add(0);
            bitIndex = 0;

            AddRange(bits);
        }

        /// <summary>
        /// Create BitArray
        /// </summary>
        /// <param name="bytes">Given bytes</param>
        public BitArray(IEnumerable<byte> bytes)
        {
            internalBits = new List<byte>(bytes);
            Count = internalBits.Count * 8;
            bitIndex = 0;
            byteIndex = internalBits.Count - 1 ;
        }

        /// <summary>
        /// Create BitArray
        /// </summary>
        /// <param name="bytes">Given bytes</param>
        /// <param name="bitCount">Amount of bits in bytes array</param>
        public BitArray(IEnumerable<byte> bytes, int bitCount)
        {
            internalBits = new List<byte>(bytes);
            Count = bitCount;
            bitIndex = (byte)(Count % 8);
            byteIndex = (Count - 1) / 8;
        }

        /// <summary>
        /// Adds a bit to the end of the array.
        /// </summary>
        /// <param name="bit">Bit to be added</param>
        public void Add(bool bit)
        {
            if (bitIndex >= 8)
            {
                bitIndex = 0;
                byteIndex++;
                internalBits.Add(0);
            }

            if (bit)
            {
                internalBits[byteIndex] += (byte)(0b10000000 >> bitIndex); // right shift bit by current bit index and add to active byte
            }

            bitIndex++;
            Count++;
        }

        /// <summary>
        /// Adds a range of bits to the end of the array.
        /// </summary>
        /// <param name="bits">Bits to be added</param>
        public void AddRange(IEnumerable<bool> bits)
        {
            foreach (var bit in bits)
            {
                Add(bit);
            }
        }

        /// <summary>
        /// Returns all bits as an array of booleans
        /// </summary>
        public bool[] GetBits()
        {
            bool[] bits = new bool[Count];

            for (int i = 0; i < Count; i++)
            {
                byte compare = (byte)(0b10000000 >> i % 8);

                bits[i] = (internalBits[i / 8] & compare) == compare;
            }

            return bits;
        }

        /// <summary>
        /// Returns all bits as an array of bytes
        /// </summary>
        public byte[] GetBytes()
        {
            return internalBits.ToArray();
        }

        /// <summary>
        /// Returns bit by index
        /// </summary>
        public bool GetValue(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            byte compare = (byte)(0b10000000 >> index % 8);

            return (internalBits[index / 8] & compare) == compare;
        }

        /// <summary>
        /// Sets bit by index
        /// </summary>
        public void SetValue(int index, bool bit)
        {
            bool val = GetValue(index);

            if (val != bit && bit)
            {
                internalBits[index / 8] += (byte)(0b10000000 >> index % 8);
            }
            else if (val != bit && !bit)
            {
                internalBits[index / 8] -= (byte)(0b10000000 >> index % 8);
            }
        }

        /// <summary>
        /// Get IEnumerator to use foreach loops
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetValue(i);
            }
        }

        /// <summary>
        /// Sets/Gets bit by index
        /// </summary>
        public bool this[int index]
        {
            get
            {
                return GetValue(index);
            }

            set
            {
                SetValue(index, value);
            }
        }

    }
}
