using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class responsible for the trail particle effects on the tunnel
public class TrailLine : MonoBehaviour
{
    public float speed = 0.05f;
    public float t1 = 0;
    public float t2 = 0;
    public LineRenderer LineRendererTemp;
    public Vertex startVertex;
    public Vertex endVertex;
    public GameObject parent;
    public PolygonTunnel rootObject;
    LineRenderer trail;

    Vector3 currentPos1;
    Vector3 currentPos2;
    float delay1 = 0.3f;
    float lifeTime = 0f;
    public int depth;


    LineRenderer newTrail;
    // Start is called before the first frame update
    void Start()
    {
        
        //Initialize the positions of the trailLine
        currentPos1 = startVertex.position;
        currentPos2 = startVertex.position;   

    }

    // Update is called once per frame
    void Update()
    {
        //Set the speed so that it reacts to the current amplitude of the low frequencies
        speed = Mathf.Pow(MainAudio._audioBandBuffer[0],2)*0.05f+0.01f;
        if(startVertex == null || endVertex == null){
            //If the vertices dont exist, destroy self
            Destroy(LineRendererTemp.gameObject);
            Destroy(this);
        }
        
        //Update one of the points position by using linear interpolation
        currentPos1 = Vector3.Lerp(startVertex.position,endVertex.position,t1);

        //Check if the second point should start moving by looking if the first position has moved far enough
        if(t1 > delay1){
            currentPos2 = Vector3.Lerp(startVertex.position,endVertex.position,t2);
            t2 += speed;
        }else{
            currentPos2 = startVertex.position;
            lifeTime += Time.deltaTime;
        }

        //Set the positions in the linerenderer 
        //This is the part that actually makes them visible
        LineRendererTemp.SetPosition(0, currentPos1);
        LineRendererTemp.SetPosition(1, currentPos2);


        //increase one of the points location
        t1 += speed;

        //if the final desination is reached, stay there
        if(t1 >= 1){
            currentPos1 = endVertex.position;
        }

        //when the second point reaches its desination, spawn a new particle if the max depth is not reached
        //then it destroys itself
        if(t2 > 1){
            rootObject.chainTrail(endVertex,depth,startVertex);
            Destroy(LineRendererTemp.gameObject);
            Destroy(this);
            //Destroy(parent.transform);
        }
    }

    //Sets the linerenderer to the one passed in the arguemtn
    public void setLineTemplate(LineRenderer trail){
        LineRendererTemp = (LineRenderer) Instantiate(trail);
        LineRendererTemp.SetPosition(0, currentPos1);
        LineRendererTemp.SetPosition(1, currentPos2);
    }
}
