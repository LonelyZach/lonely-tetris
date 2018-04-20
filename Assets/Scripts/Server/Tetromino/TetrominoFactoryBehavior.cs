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

    //Now spawns all blocks
    var blockPositions = GetBlock(x);

    //For the test tetromino, always specify this as the relative rotate point
    var rotatePoint = new Coordinates(x, 34);

    foreach(var position in blockPositions)
    {
      var block = _field.SpawnBlock(new Coordinates(position.X, position.Y), isSettled: false);
      tetromino.AddBlock(block);
    }
    tetromino.SetRotatePoint(rotatePoint);

    return tetromino.GetComponent<TetrominoBehavior>();
  }
    private Coordinates[] GetBlock(int x)
    {
        float y = Random.Range(0, 7);
        if (y >= 0 && y < 1) //T
            return new Coordinates[] { new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x + 1, 34), new Coordinates(x, 33), };
        else if (y >= 1 && y < 2)  //Line
            return new Coordinates[] { new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x + 1, 34), new Coordinates(x + 2, 34), };
        else if (y >= 2 && y < 3)  //L-Right
            return new Coordinates[] { new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x + 1, 34), new Coordinates(x + 1, 33), };
        else if (y >= 3 && y < 4)  //L-Left
            return new Coordinates[] { new Coordinates(x - 1, 33), new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x + 1, 34), };
        else if (y >= 4 && y < 5)  //Square
            return new Coordinates[] { new Coordinates(x, 33), new Coordinates(x, 34), new Coordinates(x + 1, 34), new Coordinates(x + 1, 33), };
        else if (y >= 5 && y < 6)  //Z-Left
            return new Coordinates[] { new Coordinates(x - 1, 34), new Coordinates(x, 34), new Coordinates(x, 33), new Coordinates(x + 1, 33), };
        else if (y >= 6 && y <= 7)  //Z-Right
            return new Coordinates[] { new Coordinates(x - 1, 33), new Coordinates(x, 34), new Coordinates(x, 33), new Coordinates(x + 1, 34), };
        else
            Debug.LogError("Could not return a coordinate array for a new block. Not Zeb's fault.");
        return new Coordinates[] { new Coordinates(0, 0) };
    }  
}
