using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingUnit : MonoBehaviour
{
    protected enum PlayerMode { Single, Multiplayer }

    [SerializeField]
    protected int _lives = 3;
    [SerializeField]
    protected float _speed = 5f;
    [SerializeField]
    protected GameObject _laserPrefab;
    [SerializeField]
    protected float _fireRate = 0.5f;
    [SerializeField]
    protected float _cooldown = 0;
    [SerializeField]
    protected PlayerMode gameMode;

    protected abstract void CalculateMovement();
    protected abstract void ShootLaser();
    [ContextMenu("Hurt")]
    public abstract void TakeDamage();
    protected abstract IEnumerator OnDeath();

    // Update is called once per frame
    protected virtual void Update()
    {
        _cooldown = Mathf.Clamp(_cooldown - Time.deltaTime, 0, _cooldown);
        ShootLaser();
        CalculateMovement();
    }

    public void Freeze()
    {
        _speed = 0;
    }
    public void AnimationEvent()
    {
        Destroy(gameObject);
    }

    protected bool CalculateCooldown() 
    {
        if (_cooldown <= 0 && _lives > 0) 
        {
            _cooldown = _fireRate;
            return true;
        }
        return false;

    }
}
