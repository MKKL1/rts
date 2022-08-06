
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public struct LineEquation
    {
        public float a;
        public float b;
    }

    public class Utils
    {
        public static bool inRange(RangeAttribute range, float value)
        {
            return value >= range.min && value <= range.max;
        }

        public static float smoothStep(float x)
        {
            return (3 * Mathf.Pow(x, 2)) - (2 * Mathf.Pow(x, 3));
        }

        public static LineEquation lineThruTwoPoints(float Ax, float Ay, float Bx, float By)
        {
            LineEquation lineEquation;
            lineEquation.a = (By - Ay) / (Bx - Ax);
            lineEquation.b = Ay - (Ax * lineEquation.a);
            return lineEquation;
        }
    }
}