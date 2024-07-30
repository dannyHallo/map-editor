using System;
using UnityEngine;
using System.Collections.Generic;

public class Grid3D
{
    public Vector3Int gridSize;
    public GridUnit[] gridUnits;
    public struct GridUnit
    {
        public GameObject cubePrefab;
        public int cubeType;
        public int navIndex;
        public int rotationEncoded;
    }

    [Serializable]
    public struct GridDataContainer
    {
        public Vector3Int gridSize;
        public int[] cubeTypes;
        public int[] navIndex;
        public int[] rotationEncoded;
    }

    public Grid3D(Vector3Int gridSize)
    {
        this.gridSize = gridSize;
        gridUnits = new GridUnit[gridSize.x * gridSize.y * gridSize.z];
        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            gridUnits[j].navIndex = -1;
        }
    }

    public Vector3Int numToVec(int n)
    {
        return new Vector3Int(
            Mathf.FloorToInt(n % (gridSize.x)),
            Mathf.FloorToInt(n / (gridSize.x * gridSize.z)),
            Mathf.FloorToInt((n % (gridSize.x * gridSize.z)) / gridSize.x));
    }

    public int vecToNum(Vector3Int v)
    {
        return v.x + v.z * gridSize.x + v.y * gridSize.x * gridSize.z;
    }

    public void RegisterCube(Vector3Int gridId, int cubeType, GameObject cubePrefab, int navIndex, int rotationEncoded)
    {
        gridUnits[vecToNum(gridId)].cubeType = cubeType;
        gridUnits[vecToNum(gridId)].cubePrefab = cubePrefab;
        gridUnits[vecToNum(gridId)].navIndex = navIndex;
        gridUnits[vecToNum(gridId)].rotationEncoded = rotationEncoded;
    }

    public bool SpaceOccupied(int n)
    {
        if (gridUnits[n].cubeType != 0) return true;
        return false;
    }

    public bool SpaceOccupied(Vector3Int gridId)
    {
        if (GetCubeTypeByGridId(gridId) != 0) return true;
        return false;
    }

    public int GetCubeTypeByGridId(Vector3Int gridId)
    {
        return gridUnits[vecToNum(gridId)].cubeType;
    }

    public void DeregisterCube(int n)
    {
        if (SpaceOccupied(n))
        {
            gridUnits[n].cubePrefab = null;
            gridUnits[n].cubeType = 0;
            gridUnits[n].navIndex = -1;
        }
    }

    public void DeregisterCube(Vector3Int gridId)
    {
        if (SpaceOccupied(gridId))
        {
            gridUnits[vecToNum(gridId)].cubePrefab = null;
            gridUnits[vecToNum(gridId)].cubeType = 0;
            gridUnits[vecToNum(gridId)].navIndex = -1;
        }
    }

    public List<GameObject> DeregisterWithType(int cubeType)
    {
        List<GameObject> objectsNeedsToBeDestroyed = new List<GameObject>();

        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            GridUnit gridUnit = gridUnits[j];

            if (gridUnit.cubeType == cubeType)
            {
                DeregisterCube(j);
                objectsNeedsToBeDestroyed.Add(gridUnit.cubePrefab);
            }
        }
        return objectsNeedsToBeDestroyed;
    }

    public GameObject GetCube(Vector3Int gridId)
    {
        if (SpaceOccupied(gridId))
            return gridUnits[vecToNum(gridId)].cubePrefab;
        return null;
    }

    public string GetJson()
    {
        GridDataContainer gridDataContainer = new GridDataContainer
        {
            gridSize = this.gridSize,
            cubeTypes = new int[gridSize.x * gridSize.y * gridSize.z],
            navIndex = new int[gridSize.x * gridSize.y * gridSize.z],
            rotationEncoded = new int[gridSize.x * gridSize.y * gridSize.z]
        };
        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            gridDataContainer.cubeTypes[j] = gridUnits[j].cubeType;
            gridDataContainer.navIndex[j] = gridUnits[j].navIndex;
            gridDataContainer.rotationEncoded[j] = gridUnits[j].rotationEncoded;
        }
        return JsonUtility.ToJson(gridDataContainer, true);
    }
}
