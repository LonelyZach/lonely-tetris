using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates {

  public int X { get; set; }
  public int Y { get; set; }

	public Coordinates(int x, int y)
  {
    X = x;
    Y = y;
  }
  public static Coordinates add(Coordinates first, Coordinates second)
  {
    return new Coordinates(first.X + second.X, first.Y + second.Y);
  }
  public static Coordinates subtract(Coordinates first, Coordinates second)
  {
    return new Coordinates(first.X - second.X, first.Y - second.Y);
  }
  public Vector3 getVector3()
  {
    return new Vector3(X, Y, 0);
  }
  public Vector2 getVector2()
  {
    return new Vector2(X, Y);
  }
}
