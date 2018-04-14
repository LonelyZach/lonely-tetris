using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TetrominoBehavior : MonoBehaviour
{
  private IList<BlockBehavior> Blocks = new List<BlockBehavior>();
  private Coordinates _rotatePoint;

  // Use this for initialization
  public void Start()
  {
  }

  // Update is called once per frame
  public void Update () {

	}

  public void SetRotatePoint(Coordinates rotatePoint)
  {
    _rotatePoint = rotatePoint;
  }
  public Coordinates GetRotatePoint()
  {
    return _rotatePoint;
  }
  public void Translate(Vector3 translation)
  {
    _rotatePoint.X += (int)translation.x;
    _rotatePoint.Y += (int)translation.y;
  }

  public void AddBlock(BlockBehavior block)
  {
    Blocks.Add(block);
  }
}
