using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public enum ResourceType : byte
    {
        WOOD,
        STONE,
        GOLD
    }

    public class TerrainResource : MonoBehaviour
    {
        public short gridPosX;
        public short gridPosY;
        public ResourceType type;
        public short resourceCount;
    }
}