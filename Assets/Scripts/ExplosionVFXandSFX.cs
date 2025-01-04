using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFXandSFX : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private AudioClip _explosionSFX;

    private AudioSource _aud;
    private void Start()
    {
        _aud = GetComponent<AudioSource>();
    }

    /// <summary>
    /// If no prefab is provided, then just play sound effects, otherwise instantiate the prefab explosion which will automatically play the sound effects
    /// </summary>
    public void PlayExplosion()
    {
        if (_explosionPrefab != null) 
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            return;
        }    
        _aud.clip = _explosionSFX;
        _aud.Play();
        
    }

    /// <summary>
    /// For Animation Event use only
    /// </summary>
    public void DeleteExplosion()
    {
        Destroy(gameObject);
    }
}
