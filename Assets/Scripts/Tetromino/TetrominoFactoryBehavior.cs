using UnityEngine;
using UnityEngine.Networking;

public class TetrominoFactoryBehavior : NetworkBehaviour
{
  public GameObject TetrominoPrefab;

  // Use this for initialization
  public void Start()
  {
  }

  // Update is called once per frame
  public void Update()
  {

  }

  public TetrominoBehavior SpawnTetromino()
  {
    var tetromino = GameObject.Instantiate(TetrominoPrefab);
    NetworkServer.Spawn(tetromino);
    return tetromino.GetComponent<TetrominoBehavior>();
  }
}
