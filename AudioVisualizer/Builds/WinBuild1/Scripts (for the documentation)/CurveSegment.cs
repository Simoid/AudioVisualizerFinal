using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class representing a segment of a bezier curve
public class CurveSegment : MonoBehaviour
{
	public Vector3 position = new Vector3(0,0,0);
	public Vector3 tangent = new Vector3(0,0,0);
	public Vector3 perpt = new Vector3(0,0,0);
	public Vertex[] vertices = new Vertex[5];
    public Vertex[] verticesOrigin = new Vertex[5];
	public int segmentN;
    public float baseRadius;
    public float expansionConst = 7f;
    bool empty = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //  Updates the position of all vertices depending on the strength of the audio
    void Update()
    {
        if(empty == false){
            for(int i = 0; i < 5; i ++){
                vertices[i].position = vertices[i].origin + Vector3.Normalize( vertices[i].transform.up) * MainAudio._audioBandBuffer[0] * expansionConst;
            }
        }
    }

    //Creates 5 vertices for this segment randomly rotated around the tangent of this segment
    //Randomly offsets to create a more uneven surface of the tunnel walls
    public void generateVertices(GameObject _sampleCubePrefab, float tunnelRadius){
        baseRadius = tunnelRadius;
    	for(int i = 0; i < 5; i ++){
	        GameObject vertexObj = (GameObject) Instantiate(_sampleCubePrefab);
            vertexObj.transform.position = this.position; // + (perpt * 5f));
            vertexObj.transform.parent = this.transform;
            vertexObj.name = "Vertex" + "_" + i;
            Vertex vertex = vertexObj.AddComponent(typeof(Vertex)) as Vertex;
            //float degrees = Random.Range(0, 360f);
            float degrees = Random.Range(0, 360f);
            if(Random.Range(0f,5f) < 1f){
                vertex.isSource = true;
            }
			vertex.transform.rotation = Quaternion.LookRotation(tangent);
            vertexObj.transform.RotateAround(vertexObj.transform.position, tangent , degrees);
            vertexObj.transform.position += vertexObj.transform.up * tunnelRadius + vertexObj.transform.up * MainAudio._audioBandBuffer[i] * expansionConst ;
			float offset = Random.Range(-2f, 2f);
			vertexObj.transform.position += tangent * offset;
            vertex.position = vertexObj.transform.position;
            vertex.radian = 0;
            vertices[i] = vertex;
            vertices[i].origin = vertex.position;
    	}
        empty = false;

    }

    //Deprecated
    public void rotateVertices(float rotationSpeed){
    	int childN = this.transform.childCount;
    	for(int i = 0; i < childN; i++){
    		Transform currChild = this.transform.GetChild(i);
    		currChild.position += currChild.up * -20f;
    		currChild.RotateAround(currChild.position, tangent, rotationSpeed);
    		currChild.position += currChild.up * 20f;
            vertices[i].position = currChild.position;
    	}
    }

    public void changeRadius(){

    }
}
