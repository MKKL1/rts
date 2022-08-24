using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public class EntityOrderTool
    {
        public float raycastMaxDistance = 500f;
        private SelectionTool selectionTool;
        private readonly Camera camera = Camera.main;
        private readonly int TerrainLayer;
        public EntityOrderTool()
        {
            TerrainLayer = LayerMask.NameToLayer("Terrain");
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(1) && selectionTool.selectedCount() > 0)
            {
                //TODO select any object that can be pathfinded to
                // create some kind of system for holding multiple tags for single gameobject
                RaycastHit[] hits;
                if ((hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), raycastMaxDistance)) != null)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.gameObject.layer == TerrainLayer)
                        {
                            GameMain.instance.mainGrid.GetGridPostion(hit.transform.position);
                            break;
                        }
                    }
                }
            }
        }
    }
}