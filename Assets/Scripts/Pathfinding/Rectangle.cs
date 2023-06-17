using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    [Serializable]
    public class Rectangle
    {
        public Vector2 BottomLeftCorner;
        public Vector2 TopRightCorner;
        public float Length => Mathf.Abs(TopRightCorner.x - BottomLeftCorner.x);
        public float Width => Mathf.Abs(TopRightCorner.y - BottomLeftCorner.y);
        public Vector2 Dimensions => new Vector2(Length, Width);

        public Vector2 GetCenter()
        {
            float x = BottomLeftCorner.x + Length / 2;
            float y = BottomLeftCorner.y + Width / 2;
            Vector2 center = new Vector2(x, y);
            return center;
        }

        public bool Contains(Vector2 coordinates)
        {
            bool xInBounds = BottomLeftCorner.x <= coordinates.x && coordinates.x <= TopRightCorner.x;
            bool yInbounds = BottomLeftCorner.y <= coordinates.y && coordinates.y <= TopRightCorner.y;
            return xInBounds && yInbounds;
        }
    }
}
