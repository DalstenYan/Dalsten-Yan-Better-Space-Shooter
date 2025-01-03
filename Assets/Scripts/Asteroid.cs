using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    public UnityEvent StartGame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser")) 
        {
            GetComponent<CircleCollider2D>().enabled = false;
            Destroy(Instantiate(_explosionPrefab, transform.position, Quaternion.identity), 2.4f);
            Destroy(collision.gameObject);
            StartGame.Invoke();
            Destroy(gameObject, .25f);
        }
    }
}
