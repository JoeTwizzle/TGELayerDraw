using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TGELayerDraw
{
    static class Extentions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(this ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(this T[] destinationArray, params T[] value)
        {
            if (value.Length >= destinationArray.Length)
            {
                throw new ArgumentException("Length of value array must be less than length of destination");
            }

            // set the initial array value
            Array.Copy(value, destinationArray, value.Length);

            int arrayToFillHalfLength = destinationArray.Length / 2;
            int copyLength;

            for (copyLength = value.Length; copyLength < arrayToFillHalfLength; copyLength <<= 1)
            {
                Array.Copy(destinationArray, 0, destinationArray, copyLength, copyLength);
            }

            Array.Copy(destinationArray, 0, destinationArray, copyLength, destinationArray.Length - copyLength);
        }
    }
}
