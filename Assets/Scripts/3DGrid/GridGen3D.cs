using UnityEngine;
using System;
using System.IO;
using TMPro;

public class GridGen3D : MonoBehaviour
{
    public Vector3Int gridSize;
    Grid3D buildingGrid;
    [SerializeField] GameObject selector;
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] TextMeshProUGUI CubeIndecatorText;
    [SerializeField] UIManager uIManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] GameObject sceneToFade;
    [SerializeField] GameObject sceneToShow;

    public bool isEnabled = false;
    float spacing = 2f;
    Vector3Int cursorId;
    Vector3Int lastCursorId;
    bool mouseDragging = false;
    Vector3Int cursorStartId;
    Vector3Int lastCursorStartId;
    Vector3Int cursorEndId;
    Vector3Int lastCursorEndId;
    Vector3Int[] cursorIds;

    GameObject currentCube;
    int currentCubeId;
    int drawMode = 0;
    public CubeRegistery[] cubeRegisteries;
    [Serializable]
    public struct CubeRegistery
    {
        public int id;
        public String text;
        public GameObject cube;
    }

    [Serializable]
    public struct GridDataContainer
    {
        public Vector3Int gridSize;
        public int[] cubeTypes;
    }

    // Debug function
    private void Awake()
    {
        gridSize = new Vector3Int(20, 5, 20);

        currentCube = cubeRegisteries[0].cube;
        currentCubeId = cubeRegisteries[0].id;
        CubeIndecatorText.text = cubeRegisteries[0].text;

        cursorIds = new Vector3Int[0];
        // Generate(resolution);
    }

    public void Generate(Vector3Int resolution)
    {
        this.gridSize = resolution;
        cameraMovement.isEnabled = true;
        this.isEnabled = true;

        buildingGrid = new Grid3D(resolution);

        // Generate floor
        for (int z = 0; z < resolution.z; z++)
        {
            for (int x = 0; x < resolution.x; x++)
            {
                Vector3Int thisCubeGridId = new Vector3Int(x, 0, z);
                InstantiateAndRegister(currentCube, thisCubeGridId, currentCubeId);

            }
        }
    }



    public void ExportJson()
    {
        String jsonStr = buildingGrid.GetJson();
        File.WriteAllText(Application.dataPath + "/save.json", jsonStr);
    }

    public void ReadJson()
    {
        String jsonStr = File.ReadAllText(Application.dataPath + "/save.json");
        Grid3D.GridDataContainer gridDataContainer = JsonUtility.FromJson<Grid3D.GridDataContainer>(jsonStr);
        gridSize = gridDataContainer.gridSize;

        cameraMovement.isEnabled = true;
        this.isEnabled = true;

        buildingGrid = new Grid3D(gridSize);
        for (int j = 0; j < gridSize.x * gridSize.y * gridSize.z; j++)
        {
            if (gridDataContainer.cubeTypes[j] != 0)
            {
                currentCube = cubeRegisteries[gridDataContainer.cubeTypes[j] - 1].cube;
                currentCubeId = cubeRegisteries[gridDataContainer.cubeTypes[j] - 1].id;
                InstantiateAndRegister(currentCube, buildingGrid.numToVec(j), currentCubeId);
            }
        }
        currentCube = cubeRegisteries[0].cube;
        currentCubeId = cubeRegisteries[0].id;
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        KeyboardInput();

        // Select cube
        CubeSelector();

        // Get cursor
        GetSelectedGridId();
        CleanAndDrawCurrentCursor();

        // Draw
        ClickAndDragHandler();
    }
    void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEnabled = false;
            cameraMovement.isEnabled = false;

            uIManager.CallFadeMenu(sceneToFade, sceneToShow);
        }
    }
    void CubeSelector()
    {
        for (int i = 0; i < cubeRegisteries.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                audioManager.PlayClip("select");
                currentCube = cubeRegisteries[i].cube;
                currentCubeId = cubeRegisteries[i].id;
                CubeIndecatorText.text = cubeRegisteries[i].text;
            }
        }
    }

    void GetSelectedGridId()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int LayerMask = 1 << 2;
        LayerMask = ~LayerMask;

        if (Physics.Raycast(ray, out hit, 2000, LayerMask))
        {
            if (hit.transform.tag != "Cube")
                return;

            Vector3 position;
            // Replace
            if (Input.GetKey(KeyCode.LeftControl))
            {
                position = hit.transform.position;
                drawMode = 2;
            }
            // Delete
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                position = hit.transform.position;
                drawMode = 1;
            }
            // Build upon
            else
            {
                position = hit.transform.position + hit.normal;
                drawMode = 0;
            }

            cursorId = HitPositionToGridId(position);
        }
    }

    void ClickAndDragHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cursorStartId = cursorId;
        }
        if (Input.GetMouseButton(0))
        {
            cursorEndId = cursorId;
            if (cursorStartId != cursorEndId)
                mouseDragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            DestroyAllCursors();
            DrawCursorIds();
        }

        if (mouseDragging)
        {
            if (CursorIdsUpdated())
            {
                CleanAndDrawAllCursors();
            }
        }
    }

    void DrawCursorIds()
    {
        foreach (Vector3Int cursorId in cursorIds)
        {
            audioManager.PlayClip("click");
            if (buildingGrid.SpaceOccupied(cursorId))
            {
                Destroy(buildingGrid.GetCube(cursorId));
                buildingGrid.DeregisterCube(cursorId);
            }

            // Replace or build mode
            if (drawMode != 1)
            {
                InstantiateAndRegister(currentCube, cursorId, currentCubeId);
            }

        }
    }

    void InstantiateAndRegister(GameObject cubePrefab, Vector3Int gridId, int cubeType)
    {
        GameObject thisCube = Instantiate(cubePrefab, GridIdToWorldPos(gridId), Quaternion.Euler(new Vector3(-90, 0, 0)));
        thisCube.transform.parent = this.transform;
        buildingGrid.RegisterCube(gridId, cubeType, thisCube);
    }

    bool CursorIdsUpdated()
    {
        int strechX = 0, strechZ = 0;

        if ((cursorStartId.y == cursorEndId.y) &&
            ((cursorStartId != lastCursorStartId) || (cursorEndId != lastCursorEndId)))
        {
            strechX = Mathf.Abs(cursorEndId.x - cursorStartId.x) + 1;
            strechZ = Mathf.Abs(cursorEndId.z - cursorStartId.z) + 1;
        }
        else
        {
            return false;
        }

        cursorIds = new Vector3Int[strechX * strechZ];
        for (int z = 0; z < strechZ; z++)
        {
            for (int x = 0; x < strechX; x++)
            {
                cursorIds[x + z * strechX] = new Vector3Int(
                    Mathf.Min(cursorEndId.x, cursorStartId.x) + x,
                    cursorStartId.y,
                    Mathf.Min(cursorEndId.z, cursorStartId.z) + z);
            }
        }
        lastCursorStartId = cursorStartId;
        lastCursorEndId = cursorEndId;
        return true;
    }

    void CleanAndDrawCurrentCursor()
    {
        // Refresh cursor when necessary
        if (cursorId != lastCursorId && gridIdIsValid(cursorId))
        {
            DestroyAllCursors();
            Instantiate(selector, GridIdToWorldPos(cursorId), Quaternion.identity);
        }
        lastCursorId = cursorId;
    }

    void CleanAndDrawAllCursors()
    {
        DestroyAllCursors();
        foreach (Vector3Int cursorId in cursorIds)
        {
            Instantiate(selector, GridIdToWorldPos(cursorId), Quaternion.identity);
        }
    }

    void DestroyAllCursors()
    {
        GameObject[] allCursors = GameObject.FindGameObjectsWithTag("Cursor");
        foreach (GameObject cursor in allCursors)
        {
            Destroy(cursor);
        }
    }

    public Vector3Int HitPositionToGridId(Vector3 pos)
    {
        Vector3Int gridId = new Vector3Int(
            Mathf.FloorToInt((pos.x + spacing / 2) / spacing),
            Mathf.FloorToInt(pos.y / spacing),
            Mathf.FloorToInt((pos.z + spacing / 2) / spacing)
        );
        return gridId;
    }

    public bool gridIdIsValid(Vector3Int gridId)
    {
        // In bound
        if (gridId.x < gridSize.x && gridId.y < gridSize.y && gridId.z < gridSize.z &&
            gridId.x >= 0 && gridId.y >= 0 && gridId.z >= 0)
            return true;
        else
            return false;
    }

    public Vector3 GridIdToWorldPos(Vector3Int id)
    {
        return new Vector3(id.x * spacing, id.y * spacing + spacing / 2, id.z * spacing);
    }
}
