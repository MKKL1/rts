using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    public class MovementPath
    {
        private Vector2 nextPoint;
        public Queue<Vector2> points;

        public MovementPath(Queue<Vector2> points)
        {
            this.points = points;
            nextPoint = points.Peek();
        }

        public Vector2 GetNextPoint()
        {
            points.Dequeue();
            nextPoint = points.Peek();
            return nextPoint;
        }

        public bool ShouldChangeDirection(Vector2 currentPosition)
        {
            return Utils.ManhattanDistance(nextPoint, currentPosition) < 0.2f;
        }

        public bool ShouldGetNext()
        {
            return points.Count > 0;
        }
    }
}