using UnityEngine;

public class Manager_Audio : MonoBehaviour
{
    private static AudioSource m_audioSource;

    // Use this for initialization
    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(AudioClip clip)
    {
        m_audioSource.clip = clip;
        m_audioSource.Play();
    }
}