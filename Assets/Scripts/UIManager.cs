using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Player _player;

    [SerializeField]
    private int _score;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _currentLivesImage;

    private void Start()
    {
        _player = GameObject.Find("Player1").GetComponent<Player>();
    }
    public void AddScore(int score) 
    {
        //Edge case of if the player dies and a laser is still travelling
        if (_player == null)
            return;

        if (_player.IsPowerupActive("TripleScorePowerup"))
        {
            score *= 3;
        }
        else if (_player.IsPowerupActive("DoubleScorePowerup"))
        {
            score *= 2;
        }
        
        _score += score;
        _scoreText.text = "Score: " + _score;
    }

    public void UpdateLives(int currentLives) 
    {
        _currentLivesImage.sprite = _liveSprites[currentLives];
    }
}
