using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Player : FlyingUnit
{
    //Variables
    [Header("Variable Stats")]
    [SerializeField]
    private float _powerupSpeed = 8.5f;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [Header("Player VFX")]
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private List<GameObject> _playerEngines;
    
    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _laserSoundEffect;
    [SerializeField]
    private AudioClip _playerDamageEffect;
    [SerializeField]
    private AudioClip _shieldPowerDownEffect;
    [SerializeField]
    private List<AudioClip> _pickupSoundEffects;

    [SerializeField]
    private bool _takeDamageOnce;

    private Coroutine hitCouroutine;
    private AudioSource _audioSource;
    public UnityEvent onPlayerDeath;

    private Dictionary<string, Coroutine> _powerupCoroutines;
    private void Start()
    {
        _powerupCoroutines = new Dictionary<string, Coroutine>();
        transform.position = new Vector3(0, 0, 0);
        _audioSource = GetComponent<AudioSource>();
        //_spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    protected override void CalculateMovement ()
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

    protected override void ShootLaser() 
    {
        if (!Input.GetKey(KeyCode.Space)) 
        {
            return;
        }
        Debug.Log("Held down");
        // && Time.time > _canFire
        //This code is basically taking the current numerical value time (e.g 1) and adding the cooldown float to the current time
        //and thats when we can fire next. If a cooldown is .15 seconds, you will only be able to fire again when time reaches 1.15
        //so the _canFire keeps track of the next Time.time value that you can fire and is after the cooldown.
        if (CalculateCooldown())
        {

            PlaySound(_laserSoundEffect);

            Instantiate(IsPowerupActive("TripleShotPowerup") ? _tripleShotPrefab : _laserPrefab, 
                transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity) ;
        }
    }

    public override void TakeDamage() 
    {
        if (_powerupCoroutines.ContainsKey("ShieldPowerup")) 
        {
            _powerupCoroutines.Remove("ShieldPowerup");
            _playerShield.SetActive(false);
            _audioSource.PlayOneShot(_shieldPowerDownEffect);
            Debug.Log("Shield Used and Broke!");
            return;
        }
        
        //Update Lives
        _lives--;
        GameObject.Find("Canvas").GetComponent<UIManager>().UpdateLives(_lives < 0 ? 0 : _lives);

        //Check for Death
        if (_lives <= 0) 
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Freeze();
            _powerupCoroutines.Clear();
            StopAllCoroutines();
            GetComponent<ExplosionVFXandSFX>().PlayExplosion();
            StartCoroutine(OnDeath());
            return;
        }

        //Play Visual and Audio Effects
        PlaySound(_playerDamageEffect);
        int engineIndex = Random.Range(0, _playerEngines.Count);
        _playerEngines[engineIndex].SetActive(true);
        _playerEngines.RemoveAt(engineIndex);

        GetComponent<BoxCollider2D>().enabled = true;
    }

    protected override IEnumerator OnDeath() 
    {
        //disable all child sprites first
        yield return new WaitForSeconds(.3f);
        for (int i = 0; i < transform.childCount; i++) 
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //then invoke player death for other scripts and then destroy self
        yield return new WaitForSeconds(.4f);
        onPlayerDeath.Invoke();
        Destroy(gameObject);
    }

    public void StartPowerup(string powerupName, float powerupDuration) 
    {
        PlaySound(GetPowerupSFX(powerupName));

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

    private AudioClip GetPowerupSFX(string powerupName) 
    {
        //Multiply Score Powerup
        if (powerupName.Contains("ScorePowerup")) 
        {
            return _pickupSoundEffects[1];
        }

        //Default SFX
        return _pickupSoundEffects[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyLaser"))
        {
            EnemyLaserHit(collision.gameObject);
        }
    }

    private void EnemyLaserHit(GameObject enemyLaser) 
    {
        //When both lasers should register damage
        if (!_takeDamageOnce)
        {
            Destroy(enemyLaser);
            TakeDamage();
            return;
        }

        //Only one laser should register damage, second is deleted
        if (hitCouroutine != null)
        {
            Debug.Log("Stopping Coroutine");
            StopCoroutine(hitCouroutine);
        }

        Destroy(enemyLaser.transform.parent.gameObject);
        hitCouroutine = StartCoroutine(AntiDoubleHit());

    }

    private IEnumerator AntiDoubleHit() 
    {
        yield return null;
        hitCouroutine = null;
        TakeDamage();
    }


    private void PlaySound(AudioClip clip) 
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
