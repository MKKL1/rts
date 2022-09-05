using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public static class VectorUtils
    {
        public static Vector2 GetWithoutY(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        public static Vector2Int GetWithoutY(this Vector3Int vector3)
        {
            return new Vector2Int(vector3.x, vector3.z);
        }

        public static Vector3 GetWithY(this Vector2 vector2, float height)
        {
            return new Vector3(vector2.x, height, vector2.y);
        }
    }
}