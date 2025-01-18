using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : FlyingUnit
{
    [SerializeField]
    private int _maxFireRate;
    [SerializeField]
    private int _scoreValue = 10;
    

    private Animator _enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("cooldown time set: " + _cooldown + " seconds");
        _enemyAnimator = GetComponent<Animator>();
    }

    protected override void CalculateMovement() 
    {

        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y <= -5.41f)
        {
            transform.position = SpawnManager.RandomTopPosition();
        }
    }

    protected override void ShootLaser()
    {
        if (CalculateCooldown()) 
        {
            SetRandomFireRate();
            Instantiate(_laserPrefab, transform);
        }
    }

    private void SetRandomFireRate() 
    {
        _cooldown = Random.Range((int)_fireRate, (_maxFireRate + 1));
    }

    public override void TakeDamage() 
    {
        _lives--;
        if (_lives <= 0) 
        {
            StartCoroutine(OnDeath());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;

        if (collision.CompareTag("Player")) 
        {
            var player = col.GetComponent<Player>();
            if (player != null) 
            {
                player.TakeDamage();
            }
            TakeDamage();
        }

        if (collision.CompareTag("Laser")) 
        {
            col.SetActive(false);
            GameManager.gm.AddScore(_scoreValue, Laser.GetFiredSourceName(col.name));
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }

    protected override IEnumerator OnDeath() 
    {
        GetComponent<ExplosionVFXandSFX>().PlayExplosion();
        Freeze();
        GetComponent<BoxCollider2D>().enabled = false;
        _enemyAnimator.SetTrigger("EnemyDead");
        yield return null;
    }
}
