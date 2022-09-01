using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public class GridVisualizer : MonoBehaviour
    {
        public bool showGrid = false;
        public float visibleGridSize = 20f;
        public float raycastMaxDistance = 200f;
        public GameObject shaderCube;

        private Camera _camera;
        private void Start()
        {
            _camera = GameMain.instance.localCamera;
        }

        private void OnValidate()
        {
            Debug.Log("A");
            shaderCube.SetActive(showGrid);
            shaderCube.transform.localScale = Vector3.one * visibleGridSize;
        }

        private void Update()
        {
            if (showGrid)
            {
                RaycastHit hit;
                if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit, raycastMaxDistance))
                {
                    shaderCube.transform.position = hit.point;
                }

            }
        }
    }
}