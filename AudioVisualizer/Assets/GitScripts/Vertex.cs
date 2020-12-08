using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Class representing a point in 3d space
public class Vertex : MonoBehaviour
{
	public Vector3 position = new Vector3(0,0,0);
	public float radian;
  public Vector3 origin = new Vector3(0,0,0);
  public bool isSource = false;
  public List<Vertex> neighbours = new List<Vertex>();
  public int neighbourCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Deprecated
    public void rotate(float rotationSpeed, float tunnelRadius, float audioSum, bool rotationdir)
    {
    	audioSum = 1f;
    	if(rotationdir == true)
  			radian += rotationSpeed;
      else
        radian -= rotationSpeed;
        
      position.x = audioSum * tunnelRadius * Mathf.Sin(radian);
      position.y = audioSum * tunnelRadius * Mathf.Cos(radian);
    }
}
