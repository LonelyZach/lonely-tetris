﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameMasterBehavior : NetworkBehaviour
{
  public GameObject TetrominoFactory;

  private TetrominoFactoryBehavior _tetrominoFactory;
  private IList<PlayerBehavior> _players;
  private IList<TetrominoBehavior> _tetrominos;

  private IDictionary<PlayerBehavior, TetrominoBehavior> _activeTetrominoByPlayer;

  // Use this for initialization
  public void Start () {
    _players = new List<PlayerBehavior>();
    _tetrominos = new List<TetrominoBehavior>();
    _activeTetrominoByPlayer = new Dictionary<PlayerBehavior, TetrominoBehavior>();
    _tetrominoFactory = TetrominoFactory.GetComponent<TetrominoFactoryBehavior>();
  }

  // Update is called once per frame
  public void Update () {
    if(FindObjectsOfType<PlayerBehavior>().Count() < 2)
    {
      // Do not start the game until we have 2 palyers connected.
      return;
    }

    SpawnNewTetrominosIfNeeded();
  }

  public void RegisterPlayer(PlayerBehavior playerToRegister)
  {
    _players.Add(playerToRegister);
  }

  public void RegisterTetromino(TetrominoBehavior tetrominoToRegister)
  {
    _tetrominos.Add(tetrominoToRegister);
  }

  public void ProcessPlayerInput(PlayerBehavior player, Direction input)
  {
    if(!_activeTetrominoByPlayer.ContainsKey(player))
    {
      // The player has not been assigned an active tetromino. Do nothing.
      return;
    }

    var activeTetromino = _activeTetrominoByPlayer[player];
    activeTetromino.Move(input);
  }

  private void SpawnNewTetrominosIfNeeded()
  {
    if (_players.All(p => _activeTetrominoByPlayer.ContainsKey(p) && _activeTetrominoByPlayer[p] != null))
    {
      // If any players are still controlling an active tetromino, do not spawn new tetrominos.
      return;
    }

    foreach(var player in _players)
    {
       var tetromino = _tetrominoFactory.SpawnTetromino();

      if (!_activeTetrominoByPlayer.ContainsKey(player))
      {
        _activeTetrominoByPlayer.Add(player, tetromino);
      }
      else
      {
        _activeTetrominoByPlayer[player] = tetromino;
      }
    }
  }
}