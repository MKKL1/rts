using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class TreeResource : TerrainResource
    {
        public TreeResource()
        {
            type = ResourceType.WOOD;
            resourceCount = 200;
        }
    }
}