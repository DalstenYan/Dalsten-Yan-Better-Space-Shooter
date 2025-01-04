using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _gameOverClip;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void GameOver() 
    {
        source.Stop();
        source.clip = _gameOverClip;
        source.loop = false;
        source.PlayDelayed(.25f);
    }
}
