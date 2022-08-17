using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class StoneResource : TerrainResource
    {
        public StoneResource()
        {
            type = ResourceType.STONE;
            resourceCount = 500;
        }
    }
}