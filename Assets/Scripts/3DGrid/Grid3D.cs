using System;
using UnityEngine;

public class Grid3D
{
    Vector3Int gridSize;
    public GridUnit[] gridUnits;
    public struct GridUnit
    {
        public GameObject cubePrefab;
        public int cubeType;
    }

    [Serializable]
    public struct GridDataContainer
    {
        public Vector3Int gridSize;
        public int[] cubeTypes;
    }

    public Grid3D(Vector3Int gridSize)
    {
        this.gridSize = gridSize;
        gridUnits = new GridUnit[gridSize.x * gridSize.y * gridSize.z];
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

    public void RegisterCube(Vector3Int gridId, int cubeType, GameObject cubePrefab)
    {
        gridUnits[vecToNum(gridId)].cubeType = cubeType;
        gridUnits[vecToNum(gridId)].cubePrefab = cubePrefab;
    }

    public bool SpaceOccupied(Vector3Int gridId)
    {
        if (gridUnits[vecToNum(gridId)].cubeType != 0)
            return true;
        else
            return false;
    }

    public void DeregisterCube(Vector3Int gridId)
    {
        if (SpaceOccupied(gridId))
        {
            gridUnits[vecToNum(gridId)].cubePrefab = null;
            gridUnits[vecToNum(gridId)].cubeType = 0;
        }
    }

    public GameObject GetCube(Vector3Int gridId)
    {
        if (SpaceOccupied(gridId))
        {
            return gridUnits[vecToNum(gridId)].cubePrefab;
        }
        else
        {
            return null;
        }
    }

    public String GetJson()
    {
        GridDataContainer gridDataContainer = new GridDataContainer();
        gridDataContainer.gridSize = this.gridSize;
        gridDataContainer.cubeTypes = new int[gridSize.x * gridSize.y * gridSize.z];
        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            gridDataContainer.cubeTypes[j] = gridUnits[j].cubeType;
        }
        String str = JsonUtility.ToJson(gridDataContainer, true);
        return str;
    }
}
