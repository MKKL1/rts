using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Assets.Scripts.Simulation;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Server only script
    /// </summary>
    public class ServerScript : NetworkBehaviour
    {
        public EntityMovement entityMovement;

        public static ServerScript instance;
        private void Awake()
        {
            instance = this;
        }

        public void InitServer()
        {
            //TODO wait for terrain to generate synchronously
            GameMain.instance.terrainManager.terrainGenerated += () =>
            {
                entityMovement = new EntityMovement();
            };
            GameMain.instance.terrainManager.initTerrain();
        }

        private void FixedUpdate()
        {
            //TODO better way to start updating entityMovement
            if(entityMovement != null)
            {
                entityMovement.FixedUpdate();
            }
        }
    }
}
