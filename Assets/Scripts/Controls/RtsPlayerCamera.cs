﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class RtsPlayerCamera : MonoBehaviour
    {
        public float keyboardMoveSpeedMax = 20f;
        public float keyboardMoveSpeedMin = 2f;
        public float mouseMoveSpeedMax = 20f;
        public float mouseMoveSpeedMin = 2f;
        public float zoomSpeed = 5f;
        public float zoomMinDistFromTerrain = 15f;
        public float minDistFromTerrainBorder = 30f;
        public float smoothTime = 0.3f;

        private UnityEngine.Terrain terrain;
        private TerrainManager terrainManager;

        private Vector3 targetPosition;
        private Vector3 cameraVelocity;

        private float minCameraY;
        private float maxCameraY;

        private LineEquation keyboardSpeedEq;
        private LineEquation mouseSpeedEq;

        private float terrainPosY;
        private Vector2 terrainCornerBottomLeft;
        private Vector2 terrainCornerTopRight;

        public float zoomValue
        {
            get { return targetPosition.y; }
            set
            {
                targetPosition.y = value;
                FixDistFromTerrain();

                targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraY, maxCameraY);
            }
        }
        
        

        private void Start()
        {
            terrain = GameMain.instance.mainTerrain;
            terrainManager = GameMain.instance.terrainManager;
            if (transform.position.y < terrain.transform.position.y) 
                Debug.LogError("Camera should be positioned above terrain");

            //Set initial target position to current position, not (0,0,0)
            targetPosition = transform.position;

            //Setting event handler that updates terrain corners
            OnTerrainGenerated();
            terrainManager.terrainGenerated += OnTerrainGenerated;
            

            //Camera can only go as height as placed at startup and as low as terrain
            maxCameraY = transform.position.y;
            minCameraY = terrainManager.waterLevel;

            //Calculating proportion of speed change based on zoom
            keyboardSpeedEq = Utils.lineThruTwoPoints(minCameraY, keyboardMoveSpeedMin, maxCameraY, keyboardMoveSpeedMax);
            mouseSpeedEq = Utils.lineThruTwoPoints(minCameraY, mouseMoveSpeedMin, maxCameraY, mouseMoveSpeedMax);
        }


        private void FixedUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraVelocity, smoothTime);
        }

        private void Update()
        {
            KeyboardControl();
            ZoomControl();
            MoveByDragMouse();
        }

        /// <summary>
        /// Moves camera to given positon using SmoothDamp
        /// </summary>
        public void SmoothMoveCamera(Vector3 positionToMove)
        {
            targetPosition = positionToMove;
        }

        /// <summary>
        /// Moves camera to given positon
        /// </summary>
        public void MoveCamera(Vector3 positionToMove)
        {
            //2 vectors has to be modifed so camera won't move smoothly to target
            transform.position = positionToMove;
            targetPosition = positionToMove;
        }

        /// <summary>
        /// Checks if camera is above terrain to ensure that, camera doesn't pass thru terrain.
        /// </summary>
        private void FixDistFromTerrain()
        {
            float height = terrain.SampleHeight(transform.position) + zoomMinDistFromTerrain;
            if (targetPosition.y < height)
                zoomValue = height;
        }

        private void OnTerrainGenerated()
        {
            terrainPosY = terrain.transform.position.y;
            terrainCornerBottomLeft = terrainManager.terrainCornerBottomLeft;
            terrainCornerTopRight = terrainManager.terrainCornerTopRight;
        }

        private void FixTerrainBorder()
        {
            targetPosition.x = ClampAdd(targetPosition.x, terrainCornerBottomLeft.x + minDistFromTerrainBorder, terrainCornerTopRight.x - minDistFromTerrainBorder, 5);
            targetPosition.z = ClampAdd(targetPosition.z, terrainCornerBottomLeft.y + minDistFromTerrainBorder, terrainCornerTopRight.y - minDistFromTerrainBorder, 5);
        }

        //Clamping value to given min and max values and adding value to create force effect
        //which keeps camera from going out of terrain bounds
        private float ClampAdd(float value, float min, float max, float add)
        {
            if (value < min)
                return value + add;
            if(value > max) 
                return value - add;
            return value;
        }

        private void KeyboardControl()
        {
            Vector3 addPos = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                addPos.z = 1;
            else if (Input.GetKey(KeyCode.S))
                addPos.z = -1;

            if (Input.GetKey(KeyCode.A))
                addPos.x = -1;
            else if (Input.GetKey(KeyCode.D))
                addPos.x = 1;

            if(addPos != Vector3.zero)
            {
                float _speed = keyboardSpeedEq.a * zoomValue + keyboardSpeedEq.b;
                if (_speed < 0) _speed = 0;
                targetPosition += Vector3.Normalize(addPos) * _speed * Time.deltaTime;
                FixDistFromTerrain();
                FixTerrainBorder();
            }
            
        }

        private void ZoomControl()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                zoomValue -= zoomSpeed * Time.deltaTime;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                zoomValue += zoomSpeed * Time.deltaTime;
            }
        }

        private void MoveByDragMouse()
        {
            if (!Input.GetKey(KeyCode.Mouse2)) return;
            Vector3 mousemovevector = Vector3.zero;
            mousemovevector.x = Input.GetAxis("Mouse X");
            mousemovevector.z = Input.GetAxis("Mouse Y");
            if (mousemovevector != Vector3.zero)
            {
                float _speed = mouseSpeedEq.a * zoomValue + mouseSpeedEq.b;
                if (_speed < 0) _speed = 0;
                targetPosition -= mousemovevector * Time.deltaTime * _speed;
                FixDistFromTerrain();
                FixTerrainBorder();
            }
        }
    }
}