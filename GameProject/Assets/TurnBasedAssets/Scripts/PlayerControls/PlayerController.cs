﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TurnBasedAssets.Scripts.Controllers;
using TurnBasedAssets.Scripts.Interface;
using TurnBasedAssets.Scripts.MessageBroker;
using TurnBasedAssets.Scripts.MouseController;
using TurnBasedAssets.Scripts.PathFinding;
using UnityEngine;

namespace TurnBasedAssets.Scripts.PlayerControls
{
    public class PlayerController : Controller
    {
        private IPathFinder _iPathFinder;
        private IPosition _iPosition;
        private Vector3 _currentPosition;
        [SerializeField] private MouseSelection mouseSelectionScript;
        [SerializeField] private GameObject pathFinderTiles;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        private IEnumerable<Vector3> path = new List<Vector3>();
        private List<GameObject> pathVisualized = new List<GameObject>();
        private Vector3 previousLocation;
        private Vector3 previousDistance;

        private float SphereRadius => mouseSelectionScript.MoveableRadius;

        private void Start()
        {
            MessageBroker.MessageBroker.Instance.SendMessageOfType(new PathFinderRequestMessage(this));
            MessageBroker.MessageBroker.Instance.SendMessageOfType(new PositionControllerRequestMessage(this));
            SetPosition();
        }

        private void SetPosition()
        {
            transform.position = _iPosition.RePosition(transform.position, mouseSelectionScript.PlanePosition);
        }

        public void Initialise(IPathFinder pathFinder)
        {
            _iPathFinder = pathFinder;
        }

        public override void Initialise(IPosition iPosition)
        {
            _iPosition = iPosition;
        }

        public IEnumerator FindPossibleMovePositions(Vector3 rawGridPoint)
        {
            ClearTiles();
            yield return StartCoroutine(routine: _iPathFinder.FindPath(transform.position, rawGridPoint, false, SphereRadius,newPath => path = newPath));
            foreach (var LOCATION in path)
            {
                var tile = Instantiate(pathFinderTiles, LOCATION, Quaternion.identity);
                pathVisualized.Add(tile);
            }
            yield return null;
        }

        public IEnumerator StartPlayerMovement()
        {
            mouseSelectionScript.enabled = false;
            foreach (var LOCATION in path)
            {
                Vector3 locationDistance = LOCATION - previousLocation;
                if (locationDistance != previousDistance)
                {
                    yield return StartCoroutine(RotatePlayer(LOCATION));
                }

                while (transform.position != LOCATION)
                {
                    yield return StartCoroutine(MoveToNextTile(LOCATION));
                }

                previousDistance = LOCATION - previousLocation;
                previousLocation = LOCATION;

            }

            mouseSelectionScript.enabled = true;
            ClearTiles();
            mouseSelectionScript.Selection.DeSelect();
            yield return null;
            
        }
        
        private IEnumerator MoveToNextTile(Vector3 location)
        {
            transform.position = Vector3.MoveTowards(transform.position, location, movementSpeed * Time.deltaTime);
            yield return null;
        }
        
        private IEnumerator RotatePlayer(Vector3 location)
        {
            Vector3 targetDir = location - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDir);
            do
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                yield return null;
            } while (transform.rotation != rotation);
        }

        private void ClearTiles()
        {
            foreach (GameObject tile in pathVisualized)
            {
                Destroy(tile);
            }

            pathVisualized.Clear();
        }
    }
}
