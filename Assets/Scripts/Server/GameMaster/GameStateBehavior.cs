using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameStateBehavior : NetworkBehaviour
{
  public Text scoreText;

  [SyncVar]
  private int _score;

  // Use this for initialization
  void Start()
  {
    SetScore(0);
  }

  // Update is called once per frame
  void Update()
  {
    scoreText.text = "Score: " + _score.ToString();
  }

  public void AddToScore(int points)
  {
    SetScore(_score + points);
  }

  private void SetScore(int newScore)
  {
    _score = newScore;
  }
}
