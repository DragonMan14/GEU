using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance;
        public Grid CurrentGrid;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }
}