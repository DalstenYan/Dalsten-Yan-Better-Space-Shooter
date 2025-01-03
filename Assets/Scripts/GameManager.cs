using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private bool _gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _gameOver)
        {
            _gameOver = false;
            RestartGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void GameOver() 
    {
        _gameOver = true;
    }
}
