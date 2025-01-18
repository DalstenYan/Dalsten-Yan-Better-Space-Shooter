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
    private List<GameObject> players;
    private Dictionary<string, Coroutine> _playerPowerupCoroutines;
    private UIManager _ui;

    [Header("Events")]
    public UnityEvent endCurrentGame;

    //delegate system
    private Coroutine _antiDoubleCallCoroutine;

    private void Awake()
    {
        
        //PlayerInput.all[0].SwitchCurrentControlScheme("KeyboardWASD", Keyboard.current);
        //PlayerInput.all[1].SwitchCurrentControlScheme("KeyboardArrows", Keyboard.current);
        _playerPowerupCoroutines = new Dictionary<string, Coroutine>();
        _ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();


        for (int i = 0; i < PlayerInput.all.Count; i++) 
        {
            PlayerInput.all[i].SwitchCurrentControlScheme(playerControlSchemes[i], Keyboard.current, Mouse.current); ;
        }

        //Singleton Assign
        gm = this;
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

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
        players.Remove(defeatedPlayer);
        if (players.Count == 0) 
        {
            _gameOver = true;
            endCurrentGame.Invoke();
        }
    }

    public void PauseGame(InputAction.CallbackContext context) 
    {
        if (context.performed && gm._antiDoubleCallCoroutine == null) 
        {
            gm._antiDoubleCallCoroutine =  gm.StartCoroutine(PauseAndUnpause());
            //coroutines can't be called from callback contexts
            //gotta figure out an alternate way to stop second input
        }
    }

    private IEnumerator PauseAndUnpause() 
    {
        yield return null;
        gm._gamePaused = !gm._gamePaused;
        Time.timeScale = gm._gamePaused ? 0 : 1;
        foreach (var p in gm.players)
        {
            p.GetComponent<PlayerInput>().SwitchCurrentActionMap(gm._gamePaused ? "UI" : "Player");
        }

        gm._antiDoubleCallCoroutine = null;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void StartPlayerPowerup(string powerupName, float powerupDuration) 
    {

        //Set an out value for the POSSIBLY existing powerup
        //Check if the coroutine actually exists in the dictionary, if it does, stop the current coroutine
        if (_playerPowerupCoroutines.TryGetValue(powerupName, out Coroutine activePowerupCouroutine))
        {
            Debug.Log(powerupName + " Stopped & Extended!");
            StopCoroutine(activePowerupCouroutine);
        }
        else { Debug.Log(powerupName + " Started!"); }

        //In both edge cases, the coroutine will be started (or restarted) and assigned to the dictionary
        _playerPowerupCoroutines[powerupName] = StartCoroutine(PowerupTimer(powerupName, powerupDuration));

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
        int ind = GetPlayerIndexByName(sourceName);
        Player playerToScore = players[ind].GetComponent<Player>();
        
        if (playerToScore.GetPowerup("TripleScorePowerup"))
        {
            score *= 3;
        }
        else if (playerToScore.GetPowerup("DoubleScorePowerup"))
        {
            score *= 2;
        }

        _score += score;
        _ui.UpdateScore(_score);
    }

    public int GetPlayerIndexByName(string playerName) 
    {
        return players.FindIndex(p => p.name == playerName);
    }

    public List<GameObject> GetAllPlayers() 
    {
        return players;
    }
}
