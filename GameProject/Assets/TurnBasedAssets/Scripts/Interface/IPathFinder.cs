﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedAssets.Scripts.Interface
{
    public interface IPathFinder
    {
        IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition, bool is3D, float movementRadius, Action<IEnumerable<Vector3>> onCompletion);
    }
}
