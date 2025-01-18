using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum PowerupEffect
{
    Singular,
    Global
}
public class Powerup : MonoBehaviour
{
    [SerializeField]
    PowerupEffect powerupType = PowerupEffect.Singular;
    [SerializeField]
    private float _speed = 3;
    [SerializeField]
    private string _powerupName;
    [SerializeField]
    private float _duration = 5;


    // Update is called once per frame
    void Update()
    {
        //move down at speed of 3 (adjust in the inspector)
        transform.Translate(_speed * Time.deltaTime * Vector3.down);
        //When we leave the screen, destroy this object
        if (transform.position.y <= -5)
        {
            Destroy(gameObject);
        }
    }

    //On Trigger Collision
    //Only collectible by tags, then be destroyed
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (powerupType == PowerupEffect.Global)
            {

            }
            else 
            {
                collision.GetComponent<Player>().StartPowerup(_powerupName, _duration);
            }
            Destroy(gameObject);
        }
    }
}
