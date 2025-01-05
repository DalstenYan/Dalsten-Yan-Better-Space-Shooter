using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingUnit : MonoBehaviour
{
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    protected abstract void CalculateMovement();
    protected abstract void ShootLaser();
    protected abstract void TakeDamage();
    protected abstract void OnDeath();
}
