using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Player : MonoBehaviour
{
    //Variables
    [Header("Variable Stats")]
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _powerupSpeed = 8.5f;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [Header("Player VFX")]
    [SerializeField]
    private GameObject _playerShield;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    public UnityEvent onPlayerDeath;

    private Dictionary<string, Coroutine> _powerupCoroutines;
    private void Start()
    {
        _powerupCoroutines = new Dictionary<string, Coroutine>();
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ShootLaser();
    }
    void CalculateMovement ()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate((IsPowerupActive("SpeedPowerup") ? _powerupSpeed : _speed) * Time.deltaTime * new Vector3(horizontalInput, verticalInput, 0));
        //optimize this with GameObject 3Dcolliders later

        //Clamping Vector3 values so that the y positon stays in between 0 and -3.8f
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void ShootLaser() 
    {
        //This code is basically taking the current numerical value time (e.g 1) and adding the cooldown float to the current time
        //and thats when we can fire next. If a cooldown is .15 seconds, you will only be able to fire again when time reaches 1.15
        //so the _canFire keeps track of the next Time.time value that you can fire and is after the cooldown.
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;

            Instantiate(IsPowerupActive("TripleShotPowerup") ? _tripleShotPrefab : _laserPrefab, 
                transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity) ;
        }
    }

    public void Damage() 
    {
        if (_powerupCoroutines.ContainsKey("ShieldPowerup")) 
        {
            _powerupCoroutines.Remove("ShieldPowerup");
            _playerShield.SetActive(false);
            Debug.Log("Shield Used and Broke!");
            return;
        }
        _lives--;
        GameObject.Find("Canvas").GetComponent<UIManager>().UpdateLives(_lives);

        if (_lives <= 0) 
        {
            _powerupCoroutines.Clear();
            onPlayerDeath.Invoke();
            Destroy(gameObject);
        }
    }

    public void StartPowerup(string powerupName, float powerupDuration) 
    {
        //Infinite durration powerups get assigned to null, and will activate a VFX for the player if needed.
        //For a shield powerup, we will assign a null value instead and just add the key to the dictionary since it doesn't rely on a timer.
        if (powerupDuration <= -1) //if the duration is supposed to be infinite
        {
            _powerupCoroutines[powerupName] = null;
            switch (powerupName) 
            {
                case "ShieldPowerup":
                    _playerShield.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("Unrecognized powerup, check your powerup strings");
                    break;
            }
            return;
        }

        //Set an out value for the POSSIBLY existing powerup
        //Check if the coroutine actually exists in the dictionary, if it does, stop the current coroutine
        if (_powerupCoroutines.TryGetValue(powerupName, out Coroutine activePowerupCouroutine))
        {
            Debug.Log(powerupName + " Stopped & Extended!");
            StopCoroutine(activePowerupCouroutine);
        }
        else { Debug.Log(powerupName + " Started!"); }

        //In both edge cases, the coroutine will be started (or restarted) and assigned to the dictionary
        _powerupCoroutines[powerupName] = StartCoroutine(PowerupTimer(powerupName, powerupDuration));
    }

    private IEnumerator PowerupTimer(string powerupName, float powerupDuration = 5) 
    {
        yield return new WaitForSeconds(powerupDuration);
        Debug.Log(powerupName + " Ended!");
        _powerupCoroutines.Remove(powerupName);
    }

    public bool IsPowerupActive(string powerupName) 
    {
        return _powerupCoroutines.ContainsKey(powerupName);
    }

}
