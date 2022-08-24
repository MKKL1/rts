using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public class MouseDragDetection
    {
        public event Action clickEvent;
        public event Action holdUpdateEvent;
        public event Action holdStartEvent;
        public event Action holdEndEvent;

        public float holdMinDistance = 2f;

        public bool buttonHeld = false;
        public Vector3 mousestartpos;
        

        public void DragUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                buttonHeld = false;
                mousestartpos = Input.mousePosition;
                holdStartEvent?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!buttonHeld)
                    clickEvent?.Invoke();
                else
                {
                    buttonHeld = false;
                    holdEndEvent?.Invoke();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!buttonHeld && manhattanDistance2(mousestartpos, Input.mousePosition) > holdMinDistance)
                    buttonHeld = true;
                if (buttonHeld)
                    holdUpdateEvent?.Invoke();
            }
        }

        private float manhattanDistance2(Vector3 a, Vector3 b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.z - b.z);
        }
    }
}