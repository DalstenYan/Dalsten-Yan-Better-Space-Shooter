using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string[] playerControlSchemes;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private bool _gameOver = false;

    private void Awake()
    {
        //PlayerInput.all[0].SwitchCurrentControlScheme("KeyboardWASD", Keyboard.current);
        //PlayerInput.all[1].SwitchCurrentControlScheme("KeyboardArrows", Keyboard.current);
        for (int i = 0; i < PlayerInput.all.Count; i++) 
        {
            PlayerInput.all[i].SwitchCurrentControlScheme(playerControlSchemes[i], Keyboard.current, Mouse.current); ;
        }
    }

    private void Start()
    {
        //var p2 = PlayerInput.Instantiate(playerPrefab, 1, "KeyboardRight", -1, Keyboard.current);

    }

    public void PlayerRestart(InputAction.CallbackContext obj)
    {
        if (_gameOver) 
        {
            _gameOver = false;
            RestartGame();
        }
    }

    public void PauseGame(InputAction.CallbackContext obj) 
    {
        Debug.Log("Pause Game");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver() 
    {
        _gameOver = true;
    }
}
