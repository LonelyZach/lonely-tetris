using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameMasterBehavior : NetworkBehaviour
{
  public const float TimeBetweenTetriminoDropsInSeconds = 1.00f;
  public GameObject Field;
  public GameObject TetrominoFactory;

  private FieldBehavior _field;
  private TetrominoFactoryBehavior _tetrominoFactory;
  private IList<PlayerBehavior> _players;
  private IDictionary<PlayerBehavior, TetrominoBehavior> _activeTetrominoByPlayer;
  private float _timeToNextTetrominoDropInSeconds;

  // Use this for initialization
  public void Start()
  {
    _players = new List<PlayerBehavior>();
    _activeTetrominoByPlayer = new Dictionary<PlayerBehavior, TetrominoBehavior>();
    _tetrominoFactory = TetrominoFactory.GetComponent<TetrominoFactoryBehavior>();
    _field = Field.GetComponent<FieldBehavior>();
    _timeToNextTetrominoDropInSeconds = TimeBetweenTetriminoDropsInSeconds;
  }

  // Update is called once per frame
  public void Update()
  {
    if (FindObjectsOfType<PlayerBehavior>().Count() < 2)
    {
      // Do not start the game until we have 2 palyers connected.
      return;
    }

    //PunishPlayerIfNeeded();
    SpawnNewTetrominosIfNeeded();
    MoveTetrominosDownIfNeeded();
    RemoveCompleteLines();
  }

  public void RegisterPlayer(PlayerBehavior playerToRegister)
  {
    _players.Add(playerToRegister);
  }

  public void ProcessPlayerInput(PlayerBehavior player, Direction input)
  {
    if (!_activeTetrominoByPlayer.ContainsKey(player) || _activeTetrominoByPlayer[player] == null)
    {
      // The player has not been assigned an active tetromino. Do nothing.
      return;
    }

    var activeTetromino = _activeTetrominoByPlayer[player];

    if (input == Direction.Up)
    {
      _field.TryRotateBlocksAsGroup(activeTetromino, 90);
    }
    else
    {
      _field.TryMoveTetromino(activeTetromino, input);
    }
  }

  /// <summary>
  /// Adds a line of new blocks to the bottom of the screen and moves all blocks up.
  /// Only call this when there are no Tetriminos on the screen.
  /// </summary>
  public void PunishPlayerIfNeeded()
  {
    // TODO call this when punishment is required. 
    _field.AddBottomRow();
  }

  private void SpawnNewTetrominosIfNeeded()
  {
    if (ActiveTetrominos().Any())
    {
      // If any players are still controlling an active tetromino, do not spawn new tetrominos.
      return;
    }

    var distanceBetweenPlayerSpawns = 4;
    var i = 1;
    foreach (var player in _players)
    {
      var tetromino = _tetrominoFactory.SpawnTetromino(distanceBetweenPlayerSpawns * i++);

      if (!_activeTetrominoByPlayer.ContainsKey(player))
      {
        _activeTetrominoByPlayer.Add(player, tetromino);
      }
      else
      {
        _activeTetrominoByPlayer[player] = tetromino;
      }
    }

    ResetTetrominoDropCounter();
  }

  private void MoveTetrominosDownIfNeeded()
  {
    _timeToNextTetrominoDropInSeconds -= Time.deltaTime;
    if (_timeToNextTetrominoDropInSeconds > 0.00f)
    {
      return;
    }

    var results = _field.TryMoveMultipleTetrominos(ActiveTetrominos(), Direction.Down);

    // Each result with a value "false" indicates a tetromino that could not drop down. This means that the tetromino has landed on the bottom.
    // Time to destroy these tetromino (removing palyer control).
    foreach (var result in results.Where(x => x.Value == false))
    {
      var landedTetromino = result.Key;

      foreach (var block in landedTetromino.Blocks)
      {
        block.IsSettled = true;
      }

      var playerControllingTetromino = _activeTetrominoByPlayer.Single(x => x.Value == landedTetromino).Key;
      _activeTetrominoByPlayer[playerControllingTetromino] = null;
      Destroy(landedTetromino);
    }

    ResetTetrominoDropCounter();
  }

  private void ResetTetrominoDropCounter()
  {
    _timeToNextTetrominoDropInSeconds = TimeBetweenTetriminoDropsInSeconds;
  }

  private IList<TetrominoBehavior> ActiveTetrominos()
  {
    return _activeTetrominoByPlayer.Where(x => x.Value != null).Select(x => x.Value).ToList();
  }

  private void RemoveCompleteLines()
  {
    var blocksToRemove = _field.FindSettledBlocksComprisingCompleteLines();
    if(blocksToRemove.Any())
    {
      _field.RemoveBlocks(blocksToRemove.ToList());
    }
  }
}
