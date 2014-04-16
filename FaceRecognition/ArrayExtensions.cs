using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public static class ArrayExtensions
    {
        public static T[] GetRow<T>(this T[,] data, int i)
        {
            T[] row = new T[data.GetLength(1)];
            for (int j = 0; j < row.Length; j++)
            {
                row[j] = data[i, j];
            }
            return row;
        }
    }
}
