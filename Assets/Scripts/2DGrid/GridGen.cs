using UnityEngine;

public class GridGen : MonoBehaviour
{
    public Vector2Int resolution;
    [SerializeField] Color32 darkCol;
    [SerializeField] Color32 lightCol;
    [SerializeField] bool darkMode;

    float marginTop = 0.02f;
    float marginBottom = 0.02f;
    float marginRight = 0.01f;

    RectTransform canvasTransform;
    float canvasScale;
    Vector3 canvasPos;

    Sprite pixelSprite;
    TexGrid texGrid;

    public void Generate(int x, int y)
    {
        resolution.x = x;
        resolution.y = y;

        // transform.localScale = new Vector3(displayScale, displayScale, displayScale);
        canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        canvasScale = canvasTransform.localScale.x;
        canvasPos = canvasTransform.position;

        ScreenAdapter();
    }

    void ScreenAdapter()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Vector3 bottomLeftWS =
            Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRightWS =
            Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight, 10f));

        float gridTotalHeight = (topRightWS.y - bottomLeftWS.y) * (1 - marginTop - marginBottom);
        float gridTotoWidth = gridTotalHeight * (resolution.x / resolution.y);
        transform.position = new Vector3(topRightWS.x - gridTotoWidth - ((topRightWS.x - bottomLeftWS.x) * marginRight),
            (topRightWS.y - (topRightWS.y - bottomLeftWS.y) * marginTop), 0);
        float gridPixelScale = gridTotalHeight / resolution.x;
        Color bgCol = darkMode ? darkCol : lightCol;
        texGrid = new TexGrid(resolution, bgCol, this.transform, gridPixelScale, canvasScale, canvasPos);
        Log(canvasScale, gridPixelScale);
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
            CheckDrawing();
    }

    void CheckDrawing()
    {
        if (Input.GetMouseButton(0))
        {
            texGrid.SetColor(Input.mousePosition, darkMode ? lightCol : darkCol);
        }
    }

    void CheckSaveImage()
    {
        Texture2D texture2D = new Texture2D(texGrid.width, texGrid.height, TextureFormat.ARGB32, false);
        texture2D.filterMode = FilterMode.Point;

        for (int y = 0; y < texGrid.height; y++)
        {
            for (int x = 0; x < texGrid.width; x++)
            {
                texture2D.SetPixel(x, y, texGrid.GetColor(x, y));
            }
        }
        texture2D.Apply();
        byte[] byteArray = texture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "pixelArt.png", byteArray);

    }

    void Log<T>(T s1)
    {
        Debug.Log(s1);
    }

    void Log<T>(T s1, T s2)
    {
        Debug.Log(s1 + ", " + s2);
    }
}
