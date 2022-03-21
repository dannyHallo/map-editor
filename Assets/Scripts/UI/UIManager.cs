using System.Collections;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    float fadeTime = 1f;
    int stepTimes = 60;
    [SerializeField] AnimationCurve menuFadeCurve;
    public ScenesRegister[] scenesRegisters;

    [Serializable]
    public struct ScenesRegister
    {
        public GameObject scene;
        public bool active;
    }

    private void Awake()
    {
        foreach (ScenesRegister scenesRegister in scenesRegisters)
        {
            scenesRegister.scene.SetActive(scenesRegister.active);
        }
    }

    public void CallFadeMenu(GameObject sceneToFade, GameObject sceneToShow)
    {
        StartCoroutine(FadeMenu(sceneToFade, sceneToShow));
    }
    IEnumerator FadeMenu(GameObject sceneToFade, GameObject sceneToShow)
    {
        sceneToShow.SetActive(true);
        for (float i = 0; i < 1; i += 1f / stepTimes)
        {
            sceneToFade.transform.localPosition = new Vector3(0, menuFadeCurve.Evaluate(i) * Screen.height, 0);
            sceneToShow.transform.localPosition = new Vector3(0, (1 + menuFadeCurve.Evaluate(i)) * Screen.height, 0);
            yield return new WaitForSeconds(fadeTime / stepTimes);
        }
        sceneToFade.SetActive(false);
    }
}
