using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FieldBehavior : NetworkBehaviour
{
  public GameObject BlockPrefab;

  public static int Width = 21;
  public static int Height = 35;

  private BlockBehavior[,] _blocks = new BlockBehavior[Width, Height];

  public void Start()
  {
    gameObject.transform.localScale = new Vector3(Width, Height, 1);
  }

  public BlockBehavior SpawnBlock(Coordinates coordinates)
  {
    if (_blocks[coordinates.X, coordinates.Y] != null)
    {
      Debug.LogWarning(string.Format("Could not spawn block at {0},{1} because another block is already there.", coordinates.X, coordinates.Y));
      return null;
    }

    var block = Instantiate(BlockPrefab).GetComponent<BlockBehavior>();
    block.transform.localPosition = CoordinatesToGameWorldPosition(coordinates);
    NetworkServer.Spawn(block.gameObject);
    _blocks[coordinates.X, coordinates.Y] = block;
    return block;
  }

  /// <summary>
  /// Moves a block, but only if there is not another block or a wall in the way.
  /// </summary>
  public void TryMoveBlock(BlockBehavior block, Direction direction)
  {
    if (IsWallAdjacent(block, direction) || IsblockAdjacent(block, direction))
    {
      return;
    }

    MoveBlocks(new List<BlockBehavior>() { block }, direction);
  }


  /// <summary>
  /// Moves a group of block, but only if there is not another block or a wall in the way.
  /// If any of the blocks cannot move, none of them move.
  /// </summary>
  public void TryMoveBlocksAsGroup(TetrominoBehavior tetromino, Direction direction)
  {
    var blocks = tetromino.GetBlocks();
    foreach (var block in blocks)
    {
      if (IsWallAdjacent(block, direction))
      {
        return;
      }

      // We need to check if the blocks in the way of our block are part of the block group. Because, if they are, it's OK to move the block, since the adjacent block will move too.
      var adjacentBlock = GetBlockAdjacent(block, direction);
      if (adjacentBlock != null && !blocks.Contains(adjacentBlock))
      {
        return;
      }
    }

    Vector3 netTranslation = MoveBlocks(blocks, direction);
    tetromino.Translate(netTranslation);
  }

  private Vector3 MoveBlocks(IList<BlockBehavior> blocks, Direction direction)
  {
    IDictionary<BlockBehavior, Coordinates> newBlockCoordinates = new Dictionary<BlockBehavior, Coordinates>();
    Vector3 netTranslation = new Vector3(0, 0, 0);

    switch(direction)
    {
      case Direction.Down:
        netTranslation = new Vector3(0, -1, 0);
        break;
      case Direction.Left:
        netTranslation = new Vector3(-1, 0, 0);
        break;
      case Direction.Right:
        netTranslation = new Vector3(1, 0, 0);
        break;

      default:
        netTranslation = new Vector3(0, 0, 0);
        break;
    }

    foreach (var block in blocks)
    {
      var coordinates = CoordinatesForBlock(block);

      _blocks[coordinates.X, coordinates.Y] = null;

      newBlockCoordinates.Add(block, new Coordinates(coordinates.X + (int)netTranslation.x, coordinates.Y + (int)netTranslation.y));
      block.transform.Translate(netTranslation);
    }

    // We don't record the new locations of the blocks until we've moved all of them. Otherwise, we might overwrite blocks that we just moved.
    foreach(var newCoordinates in newBlockCoordinates)
    {
      _blocks[newCoordinates.Value.X, newCoordinates.Value.Y] = newCoordinates.Key;
    }
    return netTranslation;
  }

  /// <summary>
  /// Rotates a group of blocks if possible
  /// </summary>
  /// <param name="tetromino"></param>
  public void TryRotateBlocksAsGroup(TetrominoBehavior tetromino, float theta)
  {
    theta *= Mathf.Deg2Rad;
    var blocks = tetromino.GetBlocks();
    var rotatePoint = tetromino.GetRotatePoint();
    float sinTheta = Mathf.Sin(theta);
    float cosTheta = Mathf.Cos(theta);
    IDictionary<BlockBehavior, Coordinates> newBlockCoordinates = new Dictionary<BlockBehavior, Coordinates>();

    foreach (var block in blocks)
    {
      Coordinates originBlock = CoordinatesForBlock(block);
      _blocks[originBlock.X, originBlock.Y] = null;
      
      Coordinates transformedBlock = Coordinates.subtract(originBlock, rotatePoint);

      Coordinates newBlock = new Coordinates(0, 0);

      newBlock.X = Mathf.RoundToInt(transformedBlock.X * cosTheta - transformedBlock.Y * sinTheta);
      newBlock.Y = Mathf.RoundToInt(transformedBlock.Y * cosTheta + transformedBlock.X * sinTheta);

      newBlock = Coordinates.add(rotatePoint, newBlock);

      Vector3 netTranslation = new Vector3(newBlock.X - originBlock.X, newBlock.Y - originBlock.Y, 0);

      newBlockCoordinates.Add(block, new Coordinates(newBlock.X, newBlock.Y));
      block.transform.Translate(netTranslation);
    }

    // We don't record the new locations of the blocks until we've moved all of them. Otherwise, we might overwrite blocks that we just moved.
    foreach (var newCoordinates in newBlockCoordinates)
    {
      _blocks[newCoordinates.Value.X, newCoordinates.Value.Y] = newCoordinates.Key;
    }
  }

  private BlockBehavior GetBlockAdjacent(BlockBehavior block, Direction direction)
  {
    var coordinates = CoordinatesForBlock(block);

    if(IsWallAdjacent(coordinates, direction))
    {
      return null; // If there is a wall in the target direction, there is not a block there. Also, we're protecting against out-of-bounds errors below.
    }

    switch (direction)
    {
      case Direction.Up:
        return BlockAtCoordinates(new Coordinates(coordinates.X, coordinates.Y + 1));
      case Direction.Down:
        return BlockAtCoordinates(new Coordinates(coordinates.X, coordinates.Y - 1));
      case Direction.Left:
        return BlockAtCoordinates(new Coordinates(coordinates.X - 1, coordinates.Y));
      case Direction.Right:
      default:
        return BlockAtCoordinates(new Coordinates(coordinates.X + 1, coordinates.Y));
    }
  }

  private bool IsblockAdjacent(BlockBehavior block, Direction direction)
  {
    return GetBlockAdjacent(block, direction) != null;
  }

  private bool IsWallAdjacent(BlockBehavior block, Direction direction)
  {
    var coordinates = CoordinatesForBlock(block);
    return IsWallAdjacent(coordinates, direction);
  }

  private bool IsWallAdjacent(Coordinates coordinates, Direction direction)
  {
    switch (direction)
    {
      case Direction.Up:
        return coordinates.Y == Height - 1;
      case Direction.Down:
        return coordinates.Y == 0;
      case Direction.Left:
        return coordinates.X == 0;
      case Direction.Right:
      default:
        return coordinates.X == Width - 1;
    }
  }

  private BlockBehavior BlockAtCoordinates(Coordinates coordinates)
  {
    return _blocks[coordinates.X, coordinates.Y];
  }

  private Vector3 CoordinatesToGameWorldPosition(Coordinates coordinates)
  {
    var xOffset = Width / 2;
    var yOffset = Height / 2;

    // Currently only supports odd values for Width and Height :)
    return new Vector3(coordinates.X - xOffset, coordinates.Y - yOffset);
  }

  private Coordinates CoordinatesForBlock(BlockBehavior block)
  {
    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {
        if(_blocks[x, y] == block)
        {
          return new Coordinates(x, y);
        }
      }
    }

    Debug.LogError("Could not find coordinates for a block");
    return new Coordinates(0, 0);
  }
}
