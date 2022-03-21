using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClipDict[] audioClipDicts;
    [System.Serializable]
    public struct AudioClipDict
    {
        public string clipName;
        public AudioClip clip;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(string clipName)
    {
        bool played = false;
        foreach (AudioClipDict audioClipDict in audioClipDicts)
        {
            if (audioClipDict.clipName == clipName)
            {
                audioSource.clip = audioClipDict.clip;
                audioSource.Play();
                played = true;
                break;
            }
        }
        if (!played)
        {
            print("Failed to find corresponding audio clip.");
        }
    }


}
