using UnityEngine;
using UnityEngine.Networking;

public class TetrominoBehavior : NetworkBehaviour
{

  // Use this for initialization
  public void Start()
  {
    if (isServer)
    {
      FindObjectOfType<GameMasterBehavior>().RegisterTetromino(this);
    }
  }

  // Update is called once per frame
  public void Update () {
		
	}

  public void Move(Direction direction)
  {
    switch(direction)
    {
      case Direction.Up:
        gameObject.transform.Translate(new Vector3(0, 1, 0));
        break;
      case Direction.Down:
        gameObject.transform.Translate(new Vector3(0, -1, 0));
        break;
      case Direction.Left:
        gameObject.transform.Translate(new Vector3(-1, 0, 0));
        break;
      case Direction.Right:
        gameObject.transform.Translate(new Vector3(1, 0, 0));
        break;
    }
  }
}
