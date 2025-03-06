using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityStats stats;

    public GeneralScript general;

    public Animator anim;
    public AudioSource audioSource;
    public AudioClip recallAudio;
    public AudioClip dieAudio;
    public AudioClip respawnAudio;
    public AudioClip[] walkingAudioClips = new AudioClip[3];
    public AudioClip[] attackingAudioClips = new AudioClip[3];
    public GameObject weapon;

    public int cost;
}
