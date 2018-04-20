using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TetrominoBehavior : MonoBehaviour
{
  public IList<BlockBehavior> Blocks = new List<BlockBehavior>();
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
    _rotatePoint.X += Mathf.RoundToInt(translation.x);
    _rotatePoint.Y += Mathf.RoundToInt(translation.y);
  }

  public void AddBlock(BlockBehavior block)
  {
    Blocks.Add(block);
  }
}
