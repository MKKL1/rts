using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    public class MovementPath
    {
        private bool firstPoint = true;
        private Vector2 nextPoint;
        public Queue<Vector2> points;

        public MovementPath(Queue<Vector2> points)
        {
            this.points = points;
            nextPoint = points.Peek();
        }

        public Vector2 GetNextPoint()
        {
            if (firstPoint)
            {
                firstPoint = false;
                return nextPoint;
            }
            points.Dequeue();
            nextPoint = points.Peek();
            return nextPoint;
        }

        public bool ShouldChangeDirection(Vector2 currentPosition)
        {
            if (firstPoint) return true;
            return Utils.ManhattanDistance(nextPoint, currentPosition) < 0.4f;
        }

        public bool ShouldGetNext()
        {
            return points.Count > 0;
        }
    }
}