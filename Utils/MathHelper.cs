using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina3D.Utils
{
    public class MathHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180.0);
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (float)(180.0 / Math.PI);
        }
    }
}
