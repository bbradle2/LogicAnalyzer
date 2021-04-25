using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteExtensionMethods
{
    //(myByte & (1 << position)) != 0 read byte at position
    //myByte | (1 << position) set byte at position
    //myByte & ~(1 << position) unsetting byte at position
    //myByte ^ (1 << position) toggling byte at position
    public static class ByteExtensions
    {
        public static bool IsBitSet(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");
            
            return (b & (1 << pos)) != 0;
        }

        public static byte SetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");

            return (byte)(b | (1 << pos));
        }

        public static byte UnsetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");

            return (byte)(b & ~(1 << pos));
        }

        public static byte ToggleBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");

            return (byte)(b ^ (1 << pos));
        }

        public static string ToBinaryString(this byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');

            
        }
    }
}

