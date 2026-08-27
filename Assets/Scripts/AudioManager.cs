using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [System.Serializable]
    public class Sound
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Sound> sounds = new();

    private Dictionary<string, Sound> soundDictionary = new();

    private void Awake()
    {
        instance = this;

        // store sounds
        foreach (Sound sound in sounds)
        {
            soundDictionary.Add(sound.id, sound);
        }
    }

    public void Play(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning($"sound not found: {id}");
            return;
        }

        audioSource.PlayOneShot(
            sound.clip,
            sound.volume
        );
    }
    
    public void Stop()
    {
        audioSource.Stop();
    }
}