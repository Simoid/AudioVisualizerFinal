using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public CurveSegment[] segments = new CurveSegment[51];
    public Vector3 p0 = new Vector3(0, 0, 0);
    public Vector3 p1 = new Vector3(0, 0, 250f);
    public Vector3 p2 = new Vector3(250f, -250f, 0);
    public GameObject _sampleCubePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Deprecated
    public void generateOuterTunnels(GameObject _sampleCubePrefab, int count, float radius){
        for(int i = 0; i < count; i++){
            
            Vector3[] vertices = new Vector3[8];
            float t = i * (float)1/count;
            GameObject bcObj = (GameObject) Instantiate(_sampleCubePrefab);
            bcObj.transform.position = this.transform.position;
            bcObj.transform.parent = this.transform;
            bcObj.name = "OuterTunnel " + i;
            bcObj.AddComponent<LineRenderer>();
            bcObj.GetComponent<LineRenderer>().positionCount = 8;
            bcObj.GetComponent<LineRenderer>().loop = true;
            //CurveSegment cs = new CurveSegment();
            Vector3 position = new Vector3((1-t)*(1-t)*p0.x + 2*t*(1-t)*p1.x + t*t*p2.x, (1-t)*(1-t)*p0.y + 2*t*(1-t)*p1.y + t*t*p2.y, (1-t)*(1-t)*p0.z + 2*t*(1-t)*p1.z + t*t*p2.z);
            bcObj.transform.position = position;
            Vector3 tangent = new Vector3((2*t-2)*p0.x + (2-4*t)*p1.x + 2*t*p2.x, (2*t-2)*p0.y + (2-4*t)*p1.y + 2*t*p2.y, (2*t-2)*p0.z + (2-4*t)*p1.z + 2*t*p2.z);
            Vector3 perp = new Vector3(0,0,0);
            if(tangent.x == 0){
                perp.y = -1f * tangent.z;
                perp.z = tangent.y;
            } else if (tangent.y == 0){
                perp.x = -1f * tangent.z;
                perp.z = tangent.x;
            } else {
                perp.x = -1f * tangent.y;
                perp.y = tangent.x;
            }

            /* 
            Debug.DrawLine(position,this.transform.up*radius + position, Color.white, 500f);

            Debug.DrawLine(position,tangent, Color.white, 500f);
            
            for(int j = 0; j < 8; j++){
                float degrees = 360f/8f;
                bcObj.transform.RotateAround(position, tangent, degrees);
                vertices[j] =  Vector3.Normalize(bcObj.transform.up) * radius + position;
            } */

            for(int j = 0; j < 8; j++){
                float degrees = 360f/8f;
                GameObject vertexObj = (GameObject) Instantiate(_sampleCubePrefab);
                vertexObj.transform.position = position; // + (perpt * 5f));
                vertexObj.transform.parent = this.transform;
                vertexObj.name = "Vertex" + "_" + i;
                Vertex vertex = vertexObj.AddComponent(typeof(Vertex)) as Vertex;
                vertex.transform.rotation = Quaternion.LookRotation(tangent);
                vertexObj.transform.RotateAround(vertexObj.transform.position, tangent , degrees);
                vertexObj.transform.position += vertexObj.transform.up * 20f;
                float offset = Random.Range(-2f, 2f);
                vertexObj.transform.position += tangent * offset;
                vertex.position = vertexObj.transform.position;
                vertex.radian = 0;
                vertices[j] =  Vector3.Normalize(bcObj.transform.up) * radius + position;
            } 

            bcObj.GetComponent<LineRenderer>().SetPositions(vertices);
        }
    }


    //Creates 50 segments spread along the bezier curve using the definition for the bezier curve
    //Not even distribution of segments along the curve
    //Saves the segments but doesn't fill them with vertices yet
    public void generateSegments(GameObject _sampleCubePrefab){
        for(int i = 0; i < 51; i++){
            float t = i*0.02f;
            GameObject segmentObj = (GameObject) Instantiate(_sampleCubePrefab);
            segmentObj.transform.position = this.transform.position;
            segmentObj.transform.parent = this.transform;
            segmentObj.name = "CurveSegment " + i;
            CurveSegment cs = segmentObj.AddComponent(typeof(CurveSegment)) as CurveSegment;
            Vector3 position = new Vector3((1-t)*(1-t)*p0.x + 2*t*(1-t)*p1.x + t*t*p2.x, (1-t)*(1-t)*p0.y + 2*t*(1-t)*p1.y + t*t*p2.y, (1-t)*(1-t)*p0.z + 2*t*(1-t)*p1.z + t*t*p2.z);
            segmentObj.transform.position = position;
            Vector3 tangent = new Vector3((2*t-2)*p0.x + (2-4*t)*p1.x + 2*t*p2.x, (2*t-2)*p0.y + (2-4*t)*p1.y + 2*t*p2.y, (2*t-2)*p0.z + (2-4*t)*p1.z + 2*t*p2.z);
            segmentObj.transform.rotation = Quaternion.LookRotation(tangent);
            cs.position = position;
            cs.tangent = Vector3.Normalize(tangent);
            Vector3 perp = new Vector3(0,0,0);
            if(tangent.x == 0){
                perp.y = -1f * cs.tangent.z;
                perp.z = cs.tangent.y;
            } else if (tangent.y == 0){
                perp.x = -1f * cs.tangent.z;
                perp.z = cs.tangent.x;
            } else {
                perp.x = -1f * cs.tangent.y;
                perp.y = cs.tangent.x;
            }
            cs.perpt = perp;
            cs.segmentN = i;
            segments[i] = cs;
            //cs.generateVertices(_sampleCubePrefab);
        }
    }

    //Returns a position along the curve from a variable t ranging from 0 to 1
    public Vector3 getPosition(float t){
        Vector3 position = new Vector3((1-t)*(1-t)*p0.x + 2*t*(1-t)*p1.x + t*t*p2.x, (1-t)*(1-t)*p0.y + 2*t*(1-t)*p1.y + t*t*p2.y, (1-t)*(1-t)*p0.z + 2*t*(1-t)*p1.z + t*t*p2.z);
        return position;
    }

    //Returns a tangent along the curve from a variable t ranging from 0 to 1
    public Vector3 getRotation(float t){
        Vector3 rotation = new Vector3((2*t-2)*p0.x + (2-4*t)*p1.x + 2*t*p2.x, (2*t-2)*p0.y + (2-4*t)*p1.y + 2*t*p2.y, (2*t-2)*p0.z + (2-4*t)*p1.z + 2*t*p2.z);
        return Vector3.Normalize(rotation);
    }
}
