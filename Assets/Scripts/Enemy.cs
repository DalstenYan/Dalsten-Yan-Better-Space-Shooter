using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private int _scoreValue = 10;

    private Animator _enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _enemyAnimator = GetComponent<Animator>();
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
            EnemyDeath();
        }

        if (collision.CompareTag("Laser")) 
        {
            Destroy(collision.gameObject);
            GameObject.Find("Canvas").GetComponent<UIManager>().AddScore(_scoreValue);
            EnemyDeath();
        }
    }

    public void EnemyDeath() 
    {
        GetComponent<ExplosionVFXandSFX>().PlayExplosion();
        Freeze();
        GetComponent<BoxCollider2D>().enabled = false;
        _enemyAnimator.SetTrigger("EnemyDead");
    }

    public void AnimationEvent() 
    {
        Destroy(gameObject);
    }

    public void Freeze() 
    {
        _speed = 0;
    }
}
