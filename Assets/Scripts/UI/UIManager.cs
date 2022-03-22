using System.Collections;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    float fadeTime = 1f;
    int stepTimes = 60;
    [SerializeField] AnimationCurve sceneSlideCurve;
    [SerializeField] GameObject block;
    [SerializeField] Canvas canvas;
    AudioManager audioManager;
    public ScenesRegister[] scenesRegisters;

    [Serializable]
    public struct ScenesRegister
    {
        public GameObject scene;
        public bool active;
    }

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        foreach (ScenesRegister scenesRegister in scenesRegisters)
        {
            scenesRegister.scene.SetActive(scenesRegister.active);
        }
    }

    public void ChangeScene(GameObject sceneToFade, GameObject sceneToShow)
    {
        sceneToShow.SetActive(true);
        sceneToFade.SetActive(false);
    }

    public void SlideDownScene(GameObject sceneToFade, GameObject sceneToShow)
    {
        StartCoroutine(ISlideDownScene(sceneToFade, sceneToShow));
    }

    public void MenuPop(GameObject popMenu)
    {
        audioManager.PlayClip("pop");
        StartCoroutine(IMenuPop(popMenu));
    }

    public void MenuFade(GameObject popMenu)
    {
        audioManager.PlayClip("pop");
        StartCoroutine(IMenuFade(popMenu));
    }

    IEnumerator ISlideDownScene(GameObject sceneToFade, GameObject sceneToShow)
    {
        sceneToShow.SetActive(true);
        for (float i = 0; i < 1; i += 1f / stepTimes)
        {
            sceneToFade.transform.localPosition = new Vector3(
                0, sceneSlideCurve.Evaluate(i) * Screen.height, 0);
            sceneToShow.transform.localPosition = new Vector3(
                0, (1 + sceneSlideCurve.Evaluate(i)) * Screen.height, 0);
            yield return new WaitForSeconds(fadeTime / stepTimes);
        }
        sceneToFade.SetActive(false);
    }

    IEnumerator IMenuPop(GameObject popMenu)
    {
        for (float i = 0; i < 1; i += 1f / stepTimes)
        {
            RectTransform panelRectTransform = popMenu.GetComponent<RectTransform>();
            float panelWidth = panelRectTransform.rect.width;

            panelRectTransform.anchoredPosition3D = new Vector3(
                -panelWidth + sceneSlideCurve.Evaluate(i) * panelWidth, 0, 0);
            yield return new WaitForSeconds(0.5f / stepTimes);
        }
    }

    IEnumerator IMenuFade(GameObject popMenu)
    {
        for (float i = 0; i < 1; i += 1f / stepTimes)
        {
            RectTransform panelRectTransform = popMenu.GetComponent<RectTransform>();
            float panelWidth = panelRectTransform.rect.width;

            panelRectTransform.anchoredPosition3D = new Vector3(
                -sceneSlideCurve.Evaluate(i) * panelWidth, 0, 0);
            yield return new WaitForSeconds(0.5f / stepTimes);
        }
    }

    public void BreakBlock(float spacing, int xResolution)
    {
        float maxForce = 2f;
        transform.position = new Vector3(
            ((xResolution - 1) * spacing) / 2,
            transform.position.y,
            transform.position.z);
        canvas.planeDistance = 5f;
        foreach (Transform subBlockTransform in block.transform)
        {
            subBlockTransform.gameObject.AddComponent<Rigidbody>();
            subBlockTransform.gameObject.GetComponent<Rigidbody>().AddForce(
                new Vector3(UnityEngine.Random.Range(-maxForce, maxForce),
                    UnityEngine.Random.Range(-maxForce, maxForce),
                    UnityEngine.Random.Range(-maxForce, maxForce)),
                    ForceMode.Impulse);
        }
        StartCoroutine(ChangeCameraMode());
        StartCoroutine(DestroyBlock());
    }

    IEnumerator ChangeCameraMode()
    {
        yield return new WaitForSeconds(1f);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    IEnumerator DestroyBlock()
    {
        yield return new WaitForSeconds(1f);
        Destroy(block);
    }
}
