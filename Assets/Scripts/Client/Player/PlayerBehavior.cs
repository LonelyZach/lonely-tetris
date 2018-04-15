using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehavior : NetworkBehaviour
{
  public float movementKeyDelay = 0.1f;
  private float movementTimePassed = 0f;

  public float rotateKeyDelay = 0.2f;
  private float rotateTimePassed = 0f;

  private static KeyCode MOVE_ROTATE = KeyCode.W;
  private static KeyCode MOVE_LEFT = KeyCode.A;
  private static KeyCode MOVE_DOWN = KeyCode.S;
  private static KeyCode MOVE_RIGHT = KeyCode.D;

  public void Start()
  {
    if (isServer)
    {
      FindObjectOfType<GameMasterBehavior>().RegisterPlayer(this);
    }
  }

  public void Update()
  {
    movementTimePassed -= Time.deltaTime;
    rotateTimePassed -= Time.deltaTime;

    if (!isLocalPlayer)
    {
      return;
    }

    if(movementTimePassed <= 0)
    {
      movementTimePassed = movementKeyDelay;

      float movementTimeMultiplier = CheckKeyAndInitialPress(MOVE_DOWN);
      movementTimeMultiplier = Mathf.Max(movementTimeMultiplier, CheckKeyAndInitialPress(MOVE_LEFT));
      movementTimeMultiplier = Mathf.Max(movementTimeMultiplier, CheckKeyAndInitialPress(MOVE_RIGHT));

      movementTimePassed *= movementTimeMultiplier;
    }
    if (rotateTimePassed <= 0)
    {
      rotateTimePassed = rotateKeyDelay;
      rotateTimePassed *= CheckKeyAndInitialPress(MOVE_ROTATE);
    }
  }

  /// <summary>
  /// Returns multiplier based on whether the key is being held or just now being pressed
  /// </summary>
  /// <param name="key">Key being checked</param>
  /// <returns></returns>
  private float CheckKeyAndInitialPress(KeyCode key)
  {
    float returnValue = 0;
    if (Input.GetKey(key))
    {
      if (Input.GetKeyDown(key))
      {
        returnValue = 2f;
      }
      else
      {
        returnValue = 1f;
      }
      RunCommand(key);
    }
    return returnValue;
  }

  /// <summary>
  /// Runs appropriate network command based on the key input
  /// </summary>
  /// <param name="key"></param>
  private void RunCommand(KeyCode key)
  {
    if (key == MOVE_ROTATE)
    {
      CmdRotate();
    }
    else if (key == MOVE_LEFT)
    {
      CmdMoveLeft();
    }
    else if (key == MOVE_DOWN)
    {
      CmdMoveDown();
    }
    else if (key == MOVE_RIGHT)
    {
      CmdMoveRight();
    }
    else
    {
      Debug.LogError("Received invalid key code command");
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