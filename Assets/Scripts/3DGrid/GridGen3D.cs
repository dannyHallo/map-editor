using UnityEngine;
using System;
using System.IO;
using TMPro;

public class GridGen3D : GridBuilding
{
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] TextMeshProUGUI CubeIndecatorText;
    [SerializeField] UIManager uIManager;
    [SerializeField] AudioManager audioManager;
    public GameObject popMenu;
    public bool menuPoped;
    public bool isEnabled = false;

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

    public void Generate(Vector3Int gridSize)
    {
        this.gridSize = gridSize;
        cameraMovement.StartLooking();
        isEnabled = true;

        grid3D = new Grid3D(gridSize);

        // Generate floor
        for (int z = 0; z < gridSize.z; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3Int thisCubeGridId = new Vector3Int(x, 0, z);
                InstantiateAndRegister(currentCube, thisCubeGridId, currentCubeId);
            }
        }
        uIManager.BreakBlock(spacing, gridSize.x);
    }

    public void ExportJson()
    {
        String jsonStr = grid3D.GetJson();
        File.WriteAllText(Application.dataPath + "/save.json", jsonStr);
    }

    public void ReadJson()
    {
        cameraMovement.StartLooking();
        isEnabled = true;

        String jsonStr = File.ReadAllText(Application.dataPath + "/save.json");

        GenerateGridByJson(jsonStr);

        currentCube = cubeRegisteries[0].cube;
        currentCubeId = cubeRegisteries[0].id;
        uIManager.BreakBlock(spacing, gridSize.x);
    }

    private void Update()
    {
        if (isEnabled)
        {
            // Select cube
            CubeSelector();

            // Get cursor
            GetSelectedGridId();
            CleanAndDrawCurrentCursor();

            // Draw
            ClickAndDragHandler();
        }

        OuterInputDetection();
    }
    void OuterInputDetection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPoped)
            {
                cameraMovement.StopLooking();
                isEnabled = false;
                DestroyAllCursors();
                uIManager.MenuPop(popMenu);

                cursorStartId = new Vector3Int();
                lastCursorStartId = new Vector3Int();
                cursorEndId = new Vector3Int();
                lastCursorEndId = new Vector3Int();
                cursorIds = new Vector3Int[0];

                menuPoped = true;
            }
            else
            {
                cameraMovement.StartLooking();
                isEnabled = true;

                uIManager.MenuFade(popMenu);
                menuPoped = false;
            }
        }
        if (Input.GetMouseButtonDown(0) && menuPoped)
        {
            cameraMovement.StartLooking();
            isEnabled = true;

            uIManager.MenuFade(popMenu);
            menuPoped = false;
        }

    }

    protected void CubeSelector()
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

    protected void ClickAndDragHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cursorStartId = cursorId;
        }
        if (Input.GetMouseButton(0))
        {
            cursorEndId = cursorId;
        }
        if (Input.GetMouseButtonUp(0))
        {
            DestroyAllCursors();
            DrawCursorIds();
        }

        if (CursorIdsUpdated())
        {
            CleanAndDrawAllCursors();
        }
    }

    protected void DrawCursorIds()
    {
        foreach (Vector3Int cursorId in cursorIds)
        {
            if (grid3D.SpaceOccupied(cursorId))
            {
                Destroy(grid3D.GetCube(cursorId));
                grid3D.DeregisterCube(cursorId);
            }
            // Replace or build mode
            if (drawMode != 1)
            {
                InstantiateAndRegister(currentCube, cursorId, currentCubeId);
            }
        }
        if (cursorIds.Length != 0)
            audioManager.PlayClip("click");
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
}


