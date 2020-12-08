using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandlerCoolBug1 : MonoBehaviour
{
    public int vertPerLayer= 8;
    public int layers = 50;
    public float layerDist = 30f;
    Vector3[][] vertices;
    GameObject[] renderers;
    public float initRad = 100f;
    float currentRad;
    float currentAngle = 0f;
    public float moveSpeed = 30f;
    float currentTime = 0f;
    // Start is called before the first frame update
    void Start()
    {   
        //Initialize arrays and matrix
        vertices = new Vector3[layers][];
        renderers = new GameObject[layers];

        for(int i = 0; i < layers; i++){
            vertices[i] = new Vector3[vertPerLayer];
            renderers[i] = new GameObject();
        }


        currentRad = initRad;
        for(int i = 0; i < layers; i++){
            renderers[i].AddComponent<LineRenderer>();
            vertices[i] = new Vector3[vertPerLayer];
            renderers[i].GetComponent<LineRenderer>().positionCount = vertPerLayer;
            renderers[i].GetComponent<LineRenderer>().loop = true;
            for(int j = 0; j < vertPerLayer; j++){
                currentAngle = j * Mathf.PI*2f / vertPerLayer;
                vertices[i][j] = new Vector3(Mathf.Cos(currentAngle)*currentRad,Mathf.Sin(currentAngle) * currentRad, i * layerDist);
            }
            renderers[i].GetComponent<LineRenderer>().SetPositions(vertices[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        /* 
        for(int i = 0; i < vertPerLayer; i++){
            currentAngle = i * Mathf.PI*2f / vertPerLayer;
            vertices[i] = new Vector3(Mathf.Cos(currentAngle)*currentRad,Mathf.Sin(currentAngle) * currentRad,300f + MainAudio._audioBandBuffer[i] * 50f);
        }
        print(vertices);
        this.GetComponent<LineRenderer>().SetPositions(vertices);
        */

        for(int i = 0; i < layers; i++){
            if(vertices[i][0].z < 0){
                for(int j = 0; j < vertPerLayer; j++){
                    vertices[i][j] = new Vector3(vertices[i][j].x, vertices[i][j].y, layers * layerDist);
                }
            }

            for(int j = 0; j < vertPerLayer; j++){
                currentAngle = j * Mathf.PI*2f / vertPerLayer  + i*0.1f;
                vertices[i][j] = new Vector3(Mathf.Cos(currentAngle)*currentRad,Mathf.Sin(currentAngle) * currentRad, vertices[i][j].z - currentTime * moveSpeed);
            }
            renderers[i].GetComponent<LineRenderer>().SetPositions(vertices[i]);
        }
    }
}
