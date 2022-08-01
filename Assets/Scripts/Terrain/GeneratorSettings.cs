using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class GeneratorSettings: MonoBehaviour
    {
        public float waterTreshold = 0.4f;
        public float waterBlending = 0.07f;
        public float plainsBlending = 0.04f;
        public float plainsHeightMultiplier = 0.1f;
        public float plainsHeightAdd = -0.5f;
        public int seed = 50;

        public static GeneratorSettings instance;

        private void Awake()
        {
            instance = this;
        }

    }
}