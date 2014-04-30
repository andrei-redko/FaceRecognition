using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Help
{
    public static class YCbCrColor
    {
        public static byte GetY(Color color)
        {
            return Convert.ToByte(0.299*color.R +
                                  0.587*color.G + 0.114*color.B);
        }

        public static byte GetCb(Color color)
        {
            return Convert.ToByte(128 - 0.168736*color.R -
                                  0.331264*color.G + 0.5*color.B);
        }

        public static byte GetCr(Color color)
        {
            return Convert.ToByte(128 + 0.5*color.R -
                                  0.418688*color.G - 0.081312*color.B);
        }
    }
}
