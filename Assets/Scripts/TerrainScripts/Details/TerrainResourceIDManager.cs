using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public enum TerrainDetailListType
    {
        TREE,
        BUSH,
        ROCK,
        GOLD
    }

    [CreateAssetMenu]
    public class TerrainResourceIDManager : ScriptableObject
    {
        public List<GameObject> trees = new List<GameObject>();

        public GameObject GetResourceByID(TerrainDetailListType type, int id)
        {
            //TODO
            switch (type) {
                case TerrainDetailListType.TREE:
                    return trees[id];
                    break;
            }
            return null;
        }
    }
}