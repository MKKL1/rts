using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    [CreateAssetMenu]
    public class TerrainGenSettings : ScriptableObject
    {
        public List<GameObject> trees = new List<GameObject>();
    }
}