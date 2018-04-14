using UnityEngine;
using UnityEngine.Networking;

public class TetrominoFactoryBehavior : NetworkBehaviour
{
  public GameObject TetrominoPrefab;

  private FieldBehavior _field;

  // Use this for initialization
  public void Start()
  {
    _field = FindObjectOfType<FieldBehavior>();
  }

  // Update is called once per frame
  public void Update()
  {

  }

  public TetrominoBehavior SpawnTetromino(int x)
  {
    var tetromino = GameObject.Instantiate(TetrominoPrefab).GetComponent<TetrominoBehavior>();

    /* Test code spawns always the same tetramino */
    var blockPositions = new Coordinates[] { new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x + 1, 34), new Coordinates(x, 33), };

    //For the test tetromino, always specify this as the relative rotate point
    var rotatePoint = new Coordinates(x, 34);

    foreach(var position in blockPositions)
    {
      var block = _field.SpawnBlock(new Coordinates(position.X, position.Y));
      tetromino.AddBlock(block);
    }
    tetromino.SetRotatePoint(rotatePoint);

    return tetromino.GetComponent<TetrominoBehavior>();
  }
}
