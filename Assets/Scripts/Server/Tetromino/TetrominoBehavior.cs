using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TetrominoBehavior : MonoBehaviour
{
  private IList<BlockBehavior> _blocks = new List<BlockBehavior>();

  // Use this for initialization
  public void Start()
  {
  }

  // Update is called once per frame
  public void Update () {
		
	}

  public void AddBlock(BlockBehavior block)
  {
    _blocks.Add(block);
  }

  public IList<BlockBehavior> GetBlocks()
  {
    return _blocks;
  }
}
