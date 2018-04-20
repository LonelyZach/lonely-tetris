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
  private IList<TetrominoBehavior> _activeTetrominosNotControlledByPlayers;
  private float _timeToNextTetrominoDropInSeconds;

  // Use this for initialization
  public void Start()
  {
    _players = new List<PlayerBehavior>();
    _activeTetrominoByPlayer = new Dictionary<PlayerBehavior, TetrominoBehavior>();
    _activeTetrominosNotControlledByPlayers = new List<TetrominoBehavior>();
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

      var playerControllingTetromino = _activeTetrominoByPlayer.SingleOrDefault(x => x.Value == landedTetromino).Key;
      if (playerControllingTetromino != null)
      {
        _activeTetrominoByPlayer[playerControllingTetromino] = null;
      }
      else
      {
        _activeTetrominosNotControlledByPlayers.Remove(landedTetromino);
      }

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
    var playerTetrominos = _activeTetrominoByPlayer.Where(x => x.Value != null).Select(x => x.Value).ToList();
    return playerTetrominos.Concat(_activeTetrominosNotControlledByPlayers).ToList();
  }

  private void RemoveCompleteLines()
  {
    var blocksToRemove = _field.FindSettledBlocksComprisingCompleteLines();
    if(blocksToRemove.Any())
    {
      var highestYAxis = blocksToRemove.Max(x => _field.CoordinatesForBlock(x).Y);
      _field.RemoveBlocks(blocksToRemove.ToList());

      // Once we've removed the line, we need to tell the blocks above the line to fall. An easy way to do this is to add them
      // to a new twtromino and let it fall.
      SpawnTetrominosForBlockGroup(_field.GetAllSettledBlocksAboveYAxis(highestYAxis));
    }
  }

  private void SpawnTetrominosForBlockGroup(IEnumerable<BlockBehavior> blocks)
  {
    /*
     * If the block group contains groups of blocks that are separated by empty squares they should be put into their own tetrominos.
     */

    var tetrominoBlocks = GetConnectedSettledBlocks(blocks.First(), blocks);

    var tetromino = _tetrominoFactory.SpawnTetrominoFromExistingblocks(tetrominoBlocks);
    _activeTetrominosNotControlledByPlayers.Add(tetromino);

    var unnusedBlocks = blocks.Where(x => !tetrominoBlocks.Contains(x));

    if(unnusedBlocks.Any())
    {
      SpawnTetrominosForBlockGroup(unnusedBlocks);
    }
  }

  private IEnumerable<BlockBehavior> GetConnectedSettledBlocks(BlockBehavior block, IEnumerable<BlockBehavior> validBlocks)
  {
    var foundBlocks = new List<BlockBehavior>() { block };
    var uncheckedBlocks = new List<BlockBehavior>() { block };

    while (uncheckedBlocks.Any())
    {
      var blockBeingChecked = uncheckedBlocks.First();
      uncheckedBlocks.Remove(blockBeingChecked);

      var adjacentBlocks = _field.GetBlocksAdjacent(block).Where(x => x.IsSettled = true && !foundBlocks.Contains(x) && validBlocks.Contains(x));
      uncheckedBlocks.AddRange(adjacentBlocks);
      foundBlocks.AddRange(adjacentBlocks);
    }

    return foundBlocks;
  }
}
