using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y <= -5.41f) 
        {
            transform.position = SpawnManager.RandomTopPosition();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player != null) 
            {
                player.Damage();
            }
            
            Destroy(gameObject);
        }

        if (collision.CompareTag("Laser")) 
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void Freeze() 
    {
        _speed = 0;
    }
}
