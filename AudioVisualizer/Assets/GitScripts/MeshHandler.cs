using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHandler : MonoBehaviour
{
    public  Material placeholderMat;
    GameObject meshObject;
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {   
        meshObject = new GameObject();
        meshObject.AddComponent<MeshFilter>();
        meshObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();

        newVertices = new Vector3[3];
        newVertices[0] = new Vector3(0.5f,1f,1f);
        newVertices[1] = new Vector3(0f,0f,1f);
        newVertices[2] = new Vector3(1f,0f,1f);
        
        mesh.vertices = newVertices;
        mesh.triangles = new int[] {0,1,2};
        placeholderMat.SetColor("_Color",new Color(1,0,0));

        meshObject.GetComponent<MeshRenderer>().material = placeholderMat;

        meshObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        placeholderMat.SetColor("_Color",new Color(Mathf.Sin(Time.time),0,0));

        meshObject.GetComponent<MeshRenderer>().material = placeholderMat;

        meshObject.GetComponent<MeshFilter>().mesh = mesh;
    }
}
