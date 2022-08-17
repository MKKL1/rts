using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    [CreateAssetMenu]
    public class ResourceIDManager : ScriptableObject
    {
        public List<GameObject> trees = new List<GameObject>();
        public List<GameObject> rocks = new List<GameObject>();
        public List<GameObject> goldOre = new List<GameObject>();

        public GameObject GetDetailByID(ResourcePrefabsList type, int id)
        {
            switch (type) {
                case ResourcePrefabsList.TREE:
                    return trees[id];
                case ResourcePrefabsList.ROCK:
                    return rocks[id];
                case ResourcePrefabsList.GOLD:
                    return goldOre[id];
            }
            return null;
        }
    }
}