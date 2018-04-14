using UnityEngine;

public enum Direction
{
  Up,
  Down,
  Left,
  Right,
  None
}

public static class DirectionMethods
{
  public static Direction Opposite(this Direction original)
  {
    if (original == Direction.Up)
      return Direction.Down;
    if (original == Direction.Down)
      return Direction.Up;
    if (original == Direction.Left)
      return Direction.Right;
    if (original == Direction.Right)
      return Direction.Left;

    return Direction.None;
  }
  public static Vector3 UnityVector(this Direction original)
  {
    if (original == Direction.Up)
      return new Vector3(0, 1, 0);
    if (original == Direction.Down)
      return new Vector3(0, -1, 0);
    if (original == Direction.Left)
      return new Vector3(-1, 0, 0);
    if (original == Direction.Right)
      return new Vector3(1, 0, 0);

    return new Vector3(0,0,0);
  }
}

