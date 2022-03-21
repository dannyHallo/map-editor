using UnityEngine;
using TMPro;

public class ButtonToolsAndFunctions : MonoBehaviour
{
    string buttonText;
    public GameObject sceneToFade;
    public GameObject sceneToShow;
    AudioManager audioManager;
    UIManager uIManager;
    TextMeshProUGUI text;

    // Special funcs
    public TMP_InputField xInput;
    public TMP_InputField zInput;
    public TMP_InputField cubeTypes;
    public GridGen3D gridGen3D;

    public ButtonName buttonName;
    public enum ButtonName { Start, Create, Read, Export }



    private void Awake()
    {
        audioManager = GameObject.Find("Main Camera").GetComponent<AudioManager>();
        uIManager = GameObject.Find("Main Camera").GetComponent<UIManager>();
        text = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        buttonText = text.text;
    }

    public void onPointerEnter()
    {
        audioManager.PlayClip("select");
        text.fontSize++;
    }

    public void onPointerExit()
    {
        text.fontSize--;
    }

    public void onPointerDown()
    {
        audioManager.PlayClip("click");
        text.text = "*" + buttonText + "*";
    }

    public void onPointerUp()
    {
        if (buttonName == ButtonName.Create)
        {
            gridGen3D.Generate(
                new Vector3Int(int.Parse(xInput.text), 5, int.Parse(zInput.text)));
        }
        else if (buttonName == ButtonName.Read)
        {
            gridGen3D.ReadJson();
        }
        else if (buttonName == ButtonName.Export)
        {
            gridGen3D.ExportJson();
        }


        if (sceneToShow && sceneToFade)
            uIManager.CallFadeMenu(sceneToFade, sceneToShow);
        text.text = buttonText;
    }
}
