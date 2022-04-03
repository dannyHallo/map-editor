using UnityEngine;

public class GridBuilding : GridManagement
{
    [SerializeField] GameObject selector;

    protected Vector3Int cursorId;
    protected Vector3Int lastCursorId;
    protected Vector3Int cursorStartId;
    protected Vector3Int lastCursorStartId;
    protected Vector3Int cursorEndId;
    protected Vector3Int lastCursorEndId;
    protected Vector3Int[] cursorIds;
    protected GameObject currentCube;
    protected int currentCubeId;
    protected int drawMode = 0;

    protected void GetSelectedGridId()
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
                position = hit.transform.position + spacing * hit.normal;
                drawMode = 0;
            }

            cursorId = WorldPosToGridId(position);
        }
    }

    protected void CleanAndDrawCurrentCursor()
    {
        // Refresh cursor when necessary
        if (cursorId != lastCursorId && gridIdIsValid(cursorId))
        {
            DestroyAllCursors();
            Instantiate(selector, GridIdToWorldPos(cursorId), Quaternion.identity);
        }
        lastCursorId = cursorId;
    }

    protected void CleanAndDrawAllCursors()
    {
        DestroyAllCursors();
        foreach (Vector3Int cursorId in cursorIds)
        {
            Instantiate(selector, GridIdToWorldPos(cursorId), Quaternion.identity);
        }
    }

    protected void DestroyAllCursors()
    {
        GameObject[] allCursors = GameObject.FindGameObjectsWithTag("Cursor");
        foreach (GameObject cursor in allCursors)
        {
            Destroy(cursor);
        }
    }
}