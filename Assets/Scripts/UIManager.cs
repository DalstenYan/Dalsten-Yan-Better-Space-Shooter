using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private Sprite[] _lifeSprites;
    [SerializeField]
    private Image[] _currentLivesDisplays;

    private Dictionary<string, Image> _playerLivesImages;

    private void Start()
    {
        //Creates a Dictionary of player names and their corresponding lives images
        _playerLivesImages = new Dictionary<string, Image>();
        int assignIndex = 0;

        //iterate through all player's names and assign the value to an image ref
        //this way, inputting the player's name will return the linked lives image
        foreach (var plyr in GameManager.gm.GetAllPlayers()) 
        {
            _playerLivesImages[plyr.name] = _currentLivesDisplays[assignIndex];
            assignIndex++;
        }
    }

    public void UpdateScore(int score) 
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives, string hurtPlayerName) 
    {
        //find a way to connect the lives images with the player index
        //due to removal of players on death, the index will change and will
        //target wrong lives image
        _playerLivesImages[hurtPlayerName].sprite = _lifeSprites[currentLives];
    }
}
