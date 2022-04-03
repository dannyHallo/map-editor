using System;
using UnityEngine;

public class GridManagement : MonoBehaviour
{
    public CubeRegistery[] cubeRegisteries;
    protected Vector3Int gridSize;
    protected Grid3D grid3D;
    protected float spacing = 2f;

    [Serializable]
    public struct CubeRegistery
    {
        public int id;
        public String text;
        public GameObject cube;
    }

    /// <summary>
    /// Initialize from a json file
    /// </summary>
    /// <param name="json">Formatted json string</param>
    protected void GenerateGridByJson(String json)
    {
        Grid3D.GridDataContainer gridDataContainer = JsonUtility.FromJson<Grid3D.GridDataContainer>(json);
        gridSize = gridDataContainer.gridSize;
        grid3D = new Grid3D(gridSize);

        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            if (gridDataContainer.cubeTypes[j] != 0)
            {
                GameObject currentCube = cubeRegisteries[gridDataContainer.cubeTypes[j] - 1].cube;
                int currentCubeId = cubeRegisteries[gridDataContainer.cubeTypes[j] - 1].id;
                InstantiateAndRegister(currentCube, serialNumToGridId(j), currentCubeId);
            }
        }
    }

    /// <summary>
    /// The only way to register a cube into the grid system.
    /// The cube will be instantiated, too.
    /// </summary>
    /// <param name="cubePrefab">This prefab is used to instantiate cube into the world</param>
    /// <param name="gridId">The position of the cube in grid space</param>
    /// <param name="cubeType">The serial number of the cube type</param>
    protected void InstantiateAndRegister(GameObject cubePrefab, Vector3Int gridId, int cubeType)
    {
        GameObject thisCube = Instantiate(cubePrefab, GridIdToWorldPos(gridId), Quaternion.Euler(new Vector3(-90, 0, 0)));
        thisCube.transform.parent = this.transform;
        grid3D.RegisterCube(gridId, cubeType, thisCube);
    }

    /// <summary>
    /// Converts grid id to world position
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    protected Vector3 GridIdToWorldPos(Vector3Int gridId)
    {
        return new Vector3(gridId.x * spacing, gridId.y * spacing + spacing / 2, gridId.z * spacing);
    }

    /// <summary>
    /// Converts world position to grid id
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    protected Vector3Int WorldPosToGridId(Vector3 worldPos)
    {
        Vector3Int gridId = new Vector3Int(
            Mathf.FloorToInt((worldPos.x + spacing / 2) / spacing),
            Mathf.FloorToInt(worldPos.y / spacing),
            Mathf.FloorToInt((worldPos.z + spacing / 2) / spacing)
        );
        return gridId;
    }

    /// <summary>
    /// Converts serial number to grid id.
    /// This function is also implemented in Grid3D
    /// </summary>
    /// <param name="serialNum"></param>
    /// <returns></returns>
    protected Vector3Int serialNumToGridId(int serialNum)
    {
        return new Vector3Int(
            Mathf.FloorToInt(serialNum % (gridSize.x)),
            Mathf.FloorToInt(serialNum / (gridSize.x * gridSize.z)),
            Mathf.FloorToInt((serialNum % (gridSize.x * gridSize.z)) / gridSize.x));
    }

    /// <summary>
    /// Converts grid id to serial number.
    /// This function is also implemented in Grid3D
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    protected int gridIdToSerialNum(Vector3Int gridId)
    {
        return gridId.x + gridId.z * gridSize.x + gridId.y * gridSize.x * gridSize.z;
    }

    /// <summary>
    /// Check if the given grid id is valid
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public bool gridIdIsValid(Vector3Int gridId)
    {
        // In bound
        if (gridId.x < gridSize.x && gridId.y < gridSize.y && gridId.z < gridSize.z &&
            gridId.x >= 0 && gridId.y >= 0 && gridId.z >= 0)
            return true;
        else
            return false;
    }
}
