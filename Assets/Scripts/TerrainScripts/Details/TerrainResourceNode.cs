using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public struct TerrainResourceNode : NetworkMessage
    {
        public ResourcePrefabsList prefabsList;
        public byte resourceTypeID;

        public bool isEmpty()
        {
            return prefabsList == ResourcePrefabsList.NONE;
        }
    }
}