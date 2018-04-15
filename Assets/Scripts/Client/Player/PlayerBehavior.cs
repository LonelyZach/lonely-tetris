using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehavior : NetworkBehaviour
{
  public void Start()
  {
    if (isServer)
    {
      FindObjectOfType<GameMasterBehavior>().RegisterPlayer(this);
    }
  }

  public void Update()
  {
    if (!isLocalPlayer)
    {
      return;
    }
    if (Input.GetKeyDown(KeyCode.W))
    {
      CmdRotate();
    }
    if (Input.GetKeyDown(KeyCode.S))
    {
      CmdMoveDown();
    }
    else if (Input.GetKeyDown(KeyCode.A))
    {
      CmdMoveLeft();
    }
    else if (Input.GetKeyDown(KeyCode.D))
    {
      CmdMoveRight();
    }
  }

  [Command]
  public void CmdRotate()
  {
    FindObjectOfType<GameMasterBehavior>().ProcessPlayerInput(this, Direction.Up);
  }

  [Command]
  public void CmdMoveLeft()
  {
    FindObjectOfType<GameMasterBehavior>().ProcessPlayerInput(this, Direction.Left);
  }

  [Command]
  public void CmdMoveRight()
  {
    FindObjectOfType<GameMasterBehavior>().ProcessPlayerInput(this, Direction.Right);
  }

  [Command]
  public void CmdMoveDown()
  {
    FindObjectOfType<GameMasterBehavior>().ProcessPlayerInput(this, Direction.Down);
  }
}