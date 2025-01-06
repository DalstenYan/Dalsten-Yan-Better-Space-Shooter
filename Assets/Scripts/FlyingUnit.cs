﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingUnit : MonoBehaviour
{
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

    protected abstract void CalculateMovement();
    protected abstract void ShootLaser();
    [ContextMenu("Hurt")]
    public abstract void TakeDamage();
    protected abstract IEnumerator OnDeath();

    // Update is called once per frame
    void Update()
    {
        _cooldown -= Time.deltaTime;
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
