using UnityEngine;

public class TexGrid
{
    public int width;
    public int height;
    Texture2D pixelTex;
    Vector3 origin;
    float scale;
    float offsetPerPixInWS;
    Vector3 originOffsetInWS;
    GameObject[,] gridArray;

    public TexGrid(Vector2Int resolution, Color bgCol, Transform parent, float gridPixelScale, float canvasScale, Vector3 canvasPos)
    {
        pixelTex = Resources.Load<Texture2D>("Textures/pixel");

        this.width = resolution.x;
        this.height = resolution.y;

        this.origin = parent.position;
        this.scale = gridPixelScale / canvasScale;
        Log(this.scale);

        Rect rect = new Rect(0, 0, 1, 1);
        Sprite pixelSprite = Sprite.Create(pixelTex, rect, new Vector2(0, 1), 1);
        gridArray = new GameObject[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                gridArray[x, y] = new GameObject("(" + x + ", " + y + ")");

                gridArray[x, y].AddComponent<SpriteRenderer>();
                gridArray[x, y].GetComponent<SpriteRenderer>().sprite = pixelSprite;
                gridArray[x, y].GetComponent<SpriteRenderer>().sprite.name = "pixel";
                gridArray[x, y].GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                gridArray[x, y].GetComponent<SpriteRenderer>().color = bgCol;
                gridArray[x, y].transform.SetParent(parent);
                gridArray[x, y].transform.localPosition = new Vector3(x * scale, -y * scale, 0);
                gridArray[x, y].transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        offsetPerPixInWS = (gridArray[1, 0].transform.position.x
            - gridArray[0, 0].transform.position.x);

        originOffsetInWS = gridArray[0, 0].transform.position;
        Log(gridArray[1, 0].transform.localPosition, gridArray[0, 0].transform.localPosition);

    }


    public void SetColor(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y].GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetColor(Vector3 mousePos, Color color)
    {
        int x, y;
        GetXY(mousePos, out x, out y);
        SetColor(x, y, color);
    }

    public Color32 GetColor(int x, int y)
    {
        return gridArray[x, y].GetComponent<SpriteRenderer>().color;
    }

    private void GetXY(Vector3 mouseSS, out int x, out int y)
    {
        mouseSS.z = 10f;
        Vector3 mouseWS = Camera.main.ScreenToWorldPoint(mouseSS);

        Vector3 mousePosInWorldSpaceWithOrigin = mouseWS - origin;

        x = Mathf.FloorToInt((mouseWS - originOffsetInWS).x / offsetPerPixInWS);
        y = Mathf.FloorToInt(-(mouseWS - originOffsetInWS).y / offsetPerPixInWS);
        // Debug.Log(x + ", " + y);
    }

    void Log<T>(T str)
    {
        Debug.Log(str);
    }

    void Log<T>(T s1, T s2)
    {
        Debug.Log(s1 + ", " + s2);
    }
}

