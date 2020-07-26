using System.Collections;
using System;

namespace Generation.Utils
{
    public static class Maths
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value.CompareTo(min) < 0)
                return min;
            else if(value.CompareTo(max) > 0)
                return max;
            else
                return value;
        }
    }
}