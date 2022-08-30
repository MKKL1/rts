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

        private void Start()
        {
            entityMovement = new EntityMovement();
        }
    }
}
