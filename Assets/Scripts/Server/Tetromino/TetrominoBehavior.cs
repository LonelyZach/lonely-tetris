using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TetrominoBehavior : MonoBehaviour
{
  public IList<BlockBehavior> Blocks = new List<BlockBehavior>();

  // Use this for initialization
  public void Start()
  {
  }
  
  // Update is called once per frame
  public void Update () {
		
	}

  public void AddBlock(BlockBehavior block)
  {
    Blocks.Add(block);
  }
}
