using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static GameManager gm;

    [SerializeField]
    public GameManager test;

    [Header("Game Variables")]
    [SerializeField]
    private int _score;
    [SerializeField]
    private bool _gameOver, _gamePaused = false;

    [SerializeField]
    private string[] playerControlSchemes;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private List<GameObject> Players;
    //Decentralize this once again, since global powerups can
    //communicate using singleton instance
    private Dictionary<string, Coroutine> _playerPowerupCoroutines;
    private Dictionary<string, int> _playerIndividualScores;
    private UIManager _ui;

    [Header("Events")]
    public UnityEvent endCurrentGame;

    [SerializeField]
    private GameObject PauseScreen;
    //delegate system
    private Coroutine _antiDoubleCallCoroutine;

    private void Awake()
    {
        
        //PlayerInput.all[0].SwitchCurrentControlScheme("KeyboardWASD", Keyboard.current);
        //PlayerInput.all[1].SwitchCurrentControlScheme("KeyboardArrows", Keyboard.current);
        _playerPowerupCoroutines = new Dictionary<string, Coroutine>();
        _playerIndividualScores = new Dictionary<string, int>();
        Players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        _ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        foreach (var player in Players) 
        {
            _playerIndividualScores.Add(player.name, 0);
        }

        for (int i = 0; i < PlayerInput.all.Count; i++) 
        {
            PlayerInput.all[i].SwitchCurrentControlScheme(playerControlSchemes[i], Keyboard.current, Mouse.current); ;
        }

        //Singleton Assign
        gm = this;
        test = this;
    }

    #region Debugging
    [ContextMenu("Show All Coroutines")]
    public void DebugAllCoroutines() 
    {
        Debug.Log("Dictionary Size: " + _playerPowerupCoroutines.Count);
        foreach (var item in _playerPowerupCoroutines)
        {
            Debug.Log("Powerup Name (Key): " + item.Key + " || Powerup Coroutine: " + item.Value);
        }
    }
    #endregion

    #region GameFunctions

    public void PlayerRestart(InputAction.CallbackContext obj)
    {
        if (_gameOver) 
        {
            _gameOver = false;
            RestartGame();
        }
    }

    public void PlayerDeath(GameObject defeatedPlayer) 
    {
        Players.Remove(defeatedPlayer);
        if (Players.Count == 0) 
        {
            _gameOver = true;
            endCurrentGame.Invoke();
        }
    }

    public void PauseGame(InputAction.CallbackContext context) 
    {
        if (context.performed && _antiDoubleCallCoroutine == null) 
        {
            _antiDoubleCallCoroutine =  StartCoroutine(PauseAndUnpause());
        }
    }

    public void ManualPause() 
    {
        StartCoroutine(PauseAndUnpause());
    }

    private IEnumerator PauseAndUnpause() 
    {
        yield return null;
        _gamePaused = !_gamePaused;
        Time.timeScale = _gamePaused ? 0 : 1;
        PauseScreen.SetActive(_gamePaused);
        foreach (var p in Players)
        {
            p.GetComponent<PlayerInput>().SwitchCurrentActionMap(_gamePaused ? "UI" : "Player");
        }
        _antiDoubleCallCoroutine = null;
    }

    public void ManualQuit() 
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void StartPlayerPowerup(string powerupName, float powerupDuration, string playerName = "") 
    {
        string truePowerupName = playerName + powerupName;
        //Set an out value for the POSSIBLY existing powerup
        //Check if the coroutine actually exists in the dictionary, if it does, stop the current coroutine
        if (_playerPowerupCoroutines.TryGetValue(truePowerupName, out Coroutine activePowerupCouroutine))
        {
            Debug.Log(truePowerupName + " Stopped & Extended!");
            StopCoroutine(activePowerupCouroutine);
        }
        else { Debug.Log(truePowerupName + " Started!"); }

        //In both edge cases, the coroutine will be started (or restarted) and assigned to the dictionary
        _playerPowerupCoroutines[truePowerupName] = powerupDuration <= -1 ? null : StartCoroutine(PowerupTimer(truePowerupName, powerupDuration));

    }

    public void StartGlobalPowerup(string globalPowerupName, float globalPowerupDuration) 
    {
        StartPlayerPowerup(globalPowerupName, globalPowerupDuration);
    }

    private IEnumerator PowerupTimer(string powerupName, float powerupDuration = 5)
    {
        yield return new WaitForSeconds(powerupDuration);
        Debug.Log(powerupName + " Ended!");
        _playerPowerupCoroutines.Remove(powerupName);
    }

    public bool IsPowerupActive(string powerupName)
    {
        return _playerPowerupCoroutines.ContainsKey(powerupName);
    }

    public void AddScore(int score, string sourceName) 
    {
        //** For individual scoring if decided to be essential **
        //int ind = GetPlayerIndexByName(sourceName);
        //Player playerToScore = Players[ind].GetComponent<Player>();

        if (IsPowerupActive("TripleScorePowerup"))
        {
            score *= 3;
        }
        else if (IsPowerupActive("DoubleScorePowerup"))
        {
            score *= 2;
        }

        _playerIndividualScores[sourceName] += score;
        Debug.Log(sourceName + ": " + _playerIndividualScores[sourceName]);

        _score += score;
        _ui.UpdateScore(_score);
    }

    public int GetPlayerIndexByName(string playerName) 
    {
        return Players.FindIndex(p => p.name == playerName);
    }

    public List<GameObject> GetAllPlayers() 
    {
        return Players;
    }

}
