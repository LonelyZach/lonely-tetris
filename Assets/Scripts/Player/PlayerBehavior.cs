using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehavior : NetworkBehaviour
{
  void Update()
  {
    if (!isLocalPlayer)
    {
      return;
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
  void CmdMoveLeft()
  {
    gameObject.transform.Translate(new Vector3(-1, 0, 0));
  }

  [Command]
  void CmdMoveRight()
  {
    gameObject.transform.Translate(new Vector3(1, 0, 0));
  }

  [Command]
  void CmdMoveDown()
  {
    gameObject.transform.Translate(new Vector3(0, -1, 0));
  }
}