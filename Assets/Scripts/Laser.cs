using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private string _sourceName;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
        if (transform.position.y >= 7 || transform.position.y <= -5) 
        {
            DestroyLasers();
        }
    }

    public void DestroyLasers() 
    {
        Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
    }

    public static string GetFiredSourceName(string laserName) 
    {
        return laserName.Substring(0, laserName.IndexOf('_'));
    }
}
