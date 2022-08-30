
using System;
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
        public static bool inRange(float min, float max, float value)
        {
            return value >= min && value <= max;
        }

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

        public static float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }

        public static Vector2 RandomMove(Vector2 position, float maxX, float maxY)
        {
            return new Vector2(position.x + UnityEngine.Random.Range(-maxX, maxX), position.y + UnityEngine.Random.Range(-maxY, maxY));
        }

        public static int[] randomOrder(int count, System.Random random)
        {
            int[] elements = new int[count];
            //Generated sorted list (0,1,2,3)
            for (int i = 0; i < count; i++)
                elements[i] = i;

            //Fisher-Yates shuffle
            for (int i = count - 1; i >= 0; i--)
            {
                int j = random.Next(0, i + 1);
                Utils.Swap(ref elements, i, j);
            }
            return elements;
        }

        public static int[] randomOrder(int count, int seed)
        {
            return randomOrder(count, new System.Random(seed));
        }

        public static void Swap<T>(ref T[] arr, int p1, int p2)
        {
            (arr[p1], arr[p2]) = (arr[p2], arr[p1]);
        }

        public static void SetArrayTo<T>(ref T[,] arr, T value)
        {
            for(int i = 0; i < arr.GetLength(0); i++)
                for(int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = value;
                }
        }

        public static void SetArrayTo<T>(ref T[] arr, T value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
                arr[i] = value;
        }

        public static Vector3 GetOnTopOfTerrain(Vector2 pos, Terrain terrain)
        {
            Vector3 onTop = new Vector3(pos.x, 0, pos.y);
            onTop.y = terrain.SampleHeight(onTop);
            return onTop;
        }
    }
}