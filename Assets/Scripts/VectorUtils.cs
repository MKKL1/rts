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
    }
}