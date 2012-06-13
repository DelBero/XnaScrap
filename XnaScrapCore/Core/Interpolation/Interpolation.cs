using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Interpolation
{
    public class Interpolation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="t">Normalized</param>
        /// <returns></returns>
        public static float lerp(float x1, float x2, float t)
        {
            return x1 * (1.0f - t) + x2 * t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="t">Normalized</param>
        /// <returns></returns>
        public static float clerp(float x1, float x2, float t)
        {
            return x1 * (float)System.Math.Cos(1.0f - t) + x2 * (float)System.Math.Cos(t);
        }
    }
}
