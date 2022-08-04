
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
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
    }
}