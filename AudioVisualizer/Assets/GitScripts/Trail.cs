using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 currentStartPos;
    public Vector3 endPos;
    public Vector3 currentEndPos; 
    public Vector3 currentPos;
    public float speed;
    public float t = 0;
    public GameObject TrailRenderer;
    public Vector3 upVector;
    public Vertex startVertex;
    public Vertex endVertex;
    GameObject newTrail;
/*     GameObject gm = new GameObject();
 */    // Start is called before the first frame update
    void Start()
    {
        newTrail = Instantiate(TrailRenderer);
        newTrail.transform.position = startPos;
        currentStartPos = startPos;
        currentEndPos = endPos;
    }

    // Update is called once per frame
    void Update()
    {
        TrailRenderer trail = newTrail.GetComponent<TrailRenderer>();
        Vector3[] oldPositions = new Vector3[trail.positionCount];
        currentStartPos = startVertex.position;
        currentEndPos = endVertex.position;
        newTrail.transform.position = Vector3.Lerp(currentStartPos,currentEndPos,t);

         for(int i = 0; i < trail.positionCount; i++){
            oldPositions[i] = trail.GetPosition(i);
            //trail.SetPosition(i,oldPositions[i] );
            trail.SetPosition(i,oldPositions[i] + Vector3.Normalize( upVector) * MainAudio._audioBandBuffer[0] * 7f );
        }
         

        t += 0.1f;

        if(t > 1){
            Destroy(this);
        }
    }
}
