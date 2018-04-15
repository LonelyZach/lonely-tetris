using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehavior : NetworkBehaviour
{
  public float movementKeyDelay = 0.1f;
  private float movementTimePassed = 0f;

  public float rotateKeyDelay = 0.2f;
  private float rotateTimePassed = 0f;

  //These help us get instant feedback for a keypress
  private bool movementKeyPressedLately = false;
  private bool rotateKeyPressedLately = false;

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

    if(movementTimePassed <= 0 || !movementKeyPressedLately)
    {
      movementTimePassed = movementKeyDelay;
      movementKeyPressedLately = false;

      if (Input.GetKey(KeyCode.S))
      {
        movementKeyPressedLately = true;
        if (Input.GetKeyDown(KeyCode.S))
        {
          movementTimePassed = 2 * movementKeyDelay;
        }

        CmdMoveDown();
      }

      //Can only move left or right but not both
      if (Input.GetKey(KeyCode.A))
      {
        movementKeyPressedLately = true;
        if (Input.GetKeyDown(KeyCode.A))
        {
          movementKeyPressedLately = true;
          movementTimePassed = 2 * movementKeyDelay;
        }
        CmdMoveLeft();
      }
      else if (Input.GetKey(KeyCode.D))
      {
        movementKeyPressedLately = true;
        if (Input.GetKeyDown(KeyCode.D))
        {
          movementKeyPressedLately = true;
          movementTimePassed = 2 * movementKeyDelay;
        }
        CmdMoveRight();
      }
    }
    if (rotateTimePassed <= 0 || !rotateKeyPressedLately)
    {
      rotateTimePassed = rotateKeyDelay;
      if (Input.GetKey(KeyCode.W))
      {
        CmdRotate();
        rotateKeyPressedLately = true;
      }
      else
      {
        rotateKeyPressedLately = false;
      }
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