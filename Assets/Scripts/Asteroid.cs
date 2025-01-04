using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    public UnityEvent StartGame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser")) 
        {
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<ExplosionVFXandSFX>().PlayExplosion();
            Destroy(collision.gameObject);
            StartGame.Invoke();
            Destroy(gameObject, .25f);
        }
    }
}
