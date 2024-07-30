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
    protected int navIndex;
    protected int drawMode = 0;

    protected void GetSelectedGridId()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        int LayerMask = 1 << 2;
        LayerMask = ~LayerMask;

        if (Physics.Raycast(ray, out hit, 2000, LayerMask))
        {
            Vector3 position;
            if (hit.transform.tag == "Cube")
            {
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
                print("Cursor: " + cursorId);
            }
            else if (hit.transform.tag == "Plane")
            {
                position = hit.point;
                cursorId = WorldPosToGridId(position + new Vector3(0, 1, 0));
                // print("Cursor: " + cursorId);
            }
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
        if (cursorIds.Length > 11000)
        {
            print("Too many cursors!");
            return;
        }
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