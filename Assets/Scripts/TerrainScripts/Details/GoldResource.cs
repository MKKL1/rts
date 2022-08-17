using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class GoldResource : TerrainResource
    {
        public GoldResource()
        {
            type = ResourceType.GOLD;
            resourceCount = 200;
        }
    }
}