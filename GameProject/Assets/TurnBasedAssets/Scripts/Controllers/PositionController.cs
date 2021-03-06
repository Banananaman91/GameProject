﻿using TurnBasedAssets.Scripts.Interface;
using UnityEngine;

namespace TurnBasedAssets.Scripts.Controllers
{
    public class PositionController : IPosition
    {
        private Vector3 _newPosition;
        public Vector3 Reposition(Vector3 position, Vector3 planePosition) => _newPosition = new Vector3(Mathf.Round(position.x), planePosition.y, Mathf.Round(position.z));
        
    }
}
