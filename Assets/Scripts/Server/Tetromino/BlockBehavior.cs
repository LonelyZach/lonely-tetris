using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
  public bool IsSettled = false;
 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void Delete()
  {
    Destroy(gameObject);
  }
}
