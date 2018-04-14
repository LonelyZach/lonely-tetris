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
}
