using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public class EntityOrderTool : MonoBehaviour
    {
        public float raycastMaxDistance = 500f;
        private SelectionTool selectionTool;
        private Camera _camera;
        private int TerrainLayer;
        private void Start()
        {
            selectionTool = GameMain.instance.localSelectionTool;
            TerrainLayer = LayerMask.NameToLayer("Terrain");
            _camera = GameMain.instance.localCamera;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(1) && selectionTool.selectedCount() > 0)
            {
                Debug.Log("RIGHT");
                //TODO select any object that can be pathfinded to
                // create some kind of system for holding multiple tags for single gameobject
                RaycastHit[] hits = Physics.RaycastAll(_camera.ScreenPointToRay(Input.mousePosition), raycastMaxDistance);
                if (hits != null)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.gameObject.layer == TerrainLayer)
                        {
                            
                            List<uint> networkIdentity = new List<uint>();
                            foreach(var tra in selectionTool.selected)
                            {
                                networkIdentity.Add(tra.GetComponent<Entity>().netId);
                            }

                            GameMain.instance.entityManager.CmdSetEntityGoal(networkIdentity.ToArray(), GameMain.instance.mainGrid.GetGridPostion(hit.point));
                            break;
                        }
                    }
                }
            }
        }
    }
}