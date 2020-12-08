using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main class controlling the entire tunnel
//Saves all the current Bezier Curves and is responsible for moving the camera along the tunnel
public class PolygonTunnel : MonoBehaviour
{
    public int SegmentsPerCurve = 50;
    int generatedSegmentVertices = 0;
    Vertex[] vertices = new Vertex[750];
    int vertexCount = 0; 
    public float _maxScale;
    public float tunnelRadius = 8;
    public float rotationSpeed = 0.05f;
    public bool rotateRight = true;
    public Vector3 spawnPos = new Vector3(0f, 0f, 50f);
    public float speed = -1f;
    public float maxDistance = 5.0f;
    float maxDistanceSqr;
    public LineRenderer lineRendererTemplate;
    public LineRenderer TLRendererTemplate; //Tral Line renderer

  public GameObject trailRendererTemplate;
    public int maxConnections = 5;
    public int maxLineRenderers = 2000;
    public int outerTunnelCount = 10;
    public int outerTunnelRadius = 40;
    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    List<Vector2> lineIndexes = new List<Vector2>();
    Transform _transform;
    public GameObject _sampleCubePrefab;
    CurveSegment[] segments = new CurveSegment[330];
    BezierCurve[] BezierCurves = new BezierCurve[3];
    int activeCurve = 0;
    int curveN = 0;
    public float cameraSpeed;
    float t;
    Camera m_MainCamera;
    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector3 lastDir = new Vector3(0, 0, 1);
    Quaternion bezierRotation = new Quaternion(0, 0, 1, 0);
    public int vertexDensity = 5;
    Vector3 v1;
    Vector3 v2;
    Color originalColor1 = new Color(192f/255f,10f/255f,6f/255f,1f);
    Color originalColor2 = new Color(255f/255f,123f/255f,81f/255f,1f);
    public Color currentColor1;
    public Color currentColor2;
    public Color oldColor1;

		public int maxChainDepth = 3;

    List<Line> lines = new List<Line>();
    List<GameObject> trailList = new List<GameObject>();
    GameObject trailSource;


    //Spawns there initial BezierCurves and fills them with segments and points
    void Start()
    {
        trailSource = (GameObject) Instantiate(_sampleCubePrefab);
        maxDistanceSqr = maxDistance * maxDistance;
        spawnNewBezierCurve();
        initVertices();
        spawnNewBezierCurve();
        initVertices();
        spawnNewBezierCurve();
        initVertices();
        //drawLines();
        currentColor1 = originalColor1;
        currentColor2 = originalColor2;
        v1 = 2 * BezierCurves[curveN%3].p0 - 4 * BezierCurves[curveN%3].p1 + 2 * BezierCurves[curveN%3].p2; 
        v2 = -2*BezierCurves[curveN%3].p0 + 2 * BezierCurves[curveN%3].p1;
    }

    // Update is called once per frame
    // Changes color of lines to represent current music levels
    // Moves camera forward 
    void Update()
    { 
        maxDistanceSqr = maxDistance * maxDistance;
        oldColor1 = currentColor1;
        currentColor1 = Color.Lerp(oldColor1, (MainAudio._audioBandBuffer[1] * new Color(1f,0f,0f,1f)
        + MainAudio._audioBandBuffer[4] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[6] * new Color(0f,0f,1f,1f)),Time.deltaTime);
        moveCamera();
        generateVertices();
        moveLines();
        trimlist();

        for(int i = 0; i < lines.Count; i++){
            lines[i].lr.startColor = currentColor1;
            lines[i].lr.endColor = currentColor1;
        }
    }

    //Fills initial Bezier Curves with vertices
    void initVertices(){
      for(int i = 0; i < SegmentsPerCurve; i++){
        generateVertices();
      }
    }

    //Remove old lines
    void trimlist(){
        if(lines.Count > maxLineRenderers){
            for(int i = 0; i < lines.Count-maxLineRenderers; i++){
                Destroy(lines[i].lr.gameObject);
            }
            lines.RemoveRange(0, lines.Count - maxLineRenderers);
        }
    }


    //Generates new vertices if current segment not full
    //For each vertice created attempts to make lines between itself and the previous 50 vertices if they are within range
    void generateVertices(){
        if(generatedSegmentVertices == SegmentsPerCurve){
            return;
        }
        BezierCurves[(curveN-1)%3].segments[generatedSegmentVertices].generateVertices(_sampleCubePrefab, tunnelRadius);
        for(int i = 0; i < vertexDensity; i++){
            int vertexIndex = ((curveN-1)%3)*250 + generatedSegmentVertices*5 + i;
            vertices[vertexIndex] = BezierCurves[(curveN-1)%3].segments[generatedSegmentVertices].vertices[i];
            
            Vector3 p1_position = vertices[vertexIndex].position;
            int connections = 0;
            int k = 0;
            int j = vertexIndex;

            while(k < 50){
                k++;
                j--;
                if(vertices[(j+750)%750] != null){
                    Vector3 p2_position = vertices[(j+750)%750].position;
                    float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);
                    if (distanceSqr <= maxDistanceSqr){
                        if (connections == maxConnections){
                            break;
                        }
                        connections++;
                        Line nl = new Line();
                        vertices[vertexIndex].neighbours.Add(vertices[(j+750)%750]);
                        vertices[(j+750)%750].neighbours.Add(vertices[vertexIndex]);
												vertices[(j+750)%750].neighbourCount++;
                        nl.v1 = vertices[vertexIndex];
                        nl.v2 = vertices[(j+750)%750];
                        nl.outVector = Vector3.Normalize(vertices[vertexIndex].gameObject.transform.up);
                        LineRenderer lr = (LineRenderer) Instantiate(lineRendererTemplate);
                        lr.SetPosition(0, p1_position);
                        lr.SetPosition(1, p2_position);
                        lr.enabled = true;
                        nl.lr = lr;
                        lines.Add(nl);

                    }
                }
            }
        }
        generatedSegmentVertices++;
    }

    //Creates new trails towards one random neighbour from a specified vertex
	public void chainTrail(Vertex vert, int depth, Vertex startVertex){
			if(depth >= 0){
				depth = depth - 1;
				if(vert.neighbours.Count > 1){
					Vertex neighbour = vert.neighbours[(int) Random.Range(0 , vert.neighbourCount)];

					for(int i = 0; i < vert.neighbours.Count; i++){
						if(vert.neighbours[i] != startVertex){
							neighbour = vert.neighbours[i];
							break;	
						}
					}

					TrailLine newTrail = trailSource.AddComponent(typeof(TrailLine)) as TrailLine;
					newTrail.setLineTemplate(TLRendererTemplate);
					newTrail.rootObject = this;

					newTrail.depth = depth;
					newTrail.transform.parent = trailSource.transform;
					newTrail.startVertex = vert;
					newTrail.endVertex = neighbour;
					newTrail.LineRendererTemp.startColor = currentColor1 *1.4f;
					newTrail.LineRendererTemp.endColor = currentColor1 *1.6f;
				}
			}
	}

   //Creates new trails towards each neighbour from a specified vertex recursively
	public void chainTrailAll(Vertex vert, int depth, Vertex startVertex){
		if(depth >= 0){
			depth = depth - 1;
			foreach(Vertex neighbour in vert.neighbours){
				if(neighbour != startVertex) {
					TrailLine newTrail = trailSource.AddComponent(typeof(TrailLine)) as TrailLine;
					newTrail.setLineTemplate(TLRendererTemplate);
					newTrail.rootObject = this;

					
					newTrail.depth = depth;
					newTrail.transform.parent = trailSource.transform;
					newTrail.startVertex = vert;
					newTrail.endVertex = neighbour;
					newTrail.LineRendererTemp.startColor = currentColor1 *1.4f;
					newTrail.LineRendererTemp.endColor = currentColor1 *1.6f;
				}
			}
		}
	}

    //Update the position of all lines with respect to the current audio levels
    //Moves lines inward or outward in the tunnel
    //Spawn trails along lines randomly depending on the sound level
    void moveLines(){
    	foreach(Line currLine in lines){
    		currLine.lr.SetPosition(0, currLine.v1.position);
    		currLine.lr.SetPosition(1, currLine.v2.position);

				/* if(Random.Range(0f,100f) < 0.02f *Mathf.Pow(30,MainAudio._audioBand[0])){ */
				if(Random.Range(0f,200f) < 0.2f * Mathf.Pow(MainAudio._audioBand[0],1.5f) && Vector3.Distance(Camera.main.transform.position, currLine.v1.position) < 600f){
					TrailLine newTrail = trailSource.AddComponent(typeof(TrailLine)) as TrailLine;
                    newTrail.setLineTemplate(TLRendererTemplate);
					newTrail.rootObject = this;

					newTrail.depth = maxChainDepth;
					newTrail.transform.parent = trailSource.transform;
					newTrail.startVertex = currLine.v1;
					newTrail.endVertex = currLine.v2;
					newTrail.LineRendererTemp.startColor = currentColor1 *1.4f;
					newTrail.LineRendererTemp.endColor = currentColor1 *1.6f;

				}
    	}
    }

    //Moves camera along the current Bezier Curve
    //Changes cameras rotation to the tangent of the Bezier Curve at the new position
    //Modified to change t faster at start and end of curve.
    void moveCamera(){
            Camera.main.transform.position = BezierCurves[activeCurve%3].getPosition(t);
            Vector3 rotation = BezierCurves[activeCurve%3].getRotation(t);
            Camera.main.transform.rotation = Quaternion.LookRotation(rotation, new Vector3(0, 1, 0));
            //t += cameraSpeed;
            t = t +  cameraSpeed / Vector3.Magnitude(t * v1 + v2) * Time.deltaTime;
            if(t >= 1){
                t = 0;
                activeCurve++;
                spawnNewBezierCurve();
                v1 = 2 * BezierCurves[curveN%3].p0 - 4 * BezierCurves[curveN%3].p1 + 2 * BezierCurves[curveN%3].p2; 
                v2 = -2*BezierCurves[curveN%3].p0 + 2 * BezierCurves[curveN%3].p1;
            }
    }

    //Deprecated
    void rotateSegments(){
      for(int i = 0; i < segments.Length; i++){
        segments[i].rotateVertices(rotationSpeed);
        for(int j = 0; j < 5; j++){
            vertices[i*5 + j] = segments[i].vertices[j];
          }
      }
    }

    //Spawns new Bezier Curve at end of current final Bezier Curve
    //Generated 3 random values (ForwardL, SideL and degress) randomly
    //ForwardL is how far forward the curve goes and dicides the position of the second point
    //SideL and degress decides the position of the third point by moving SideL units orthagonally from the second point and then rotating around the current tangent
    void spawnNewBezierCurve(){

        float forwardL = Random.Range(-100f, 100f) + 300f;
        //float sideL = Random.Range(-50f, 50f) + 200f;
        float sideL = forwardL;
        float degrees = Random.Range(1f, 360f);
        Vector3 p0Pos = lastPos;
        Vector3 p1Pos = lastPos + (forwardL * lastDir);

        GameObject bcObj = (GameObject) Instantiate(_sampleCubePrefab);
        bcObj.transform.position = this.transform.position;
        bcObj.transform.parent = this.transform;
        bcObj.name = "BezierCurve";
        bcObj.SetActive(true);
        BezierCurve newCurve = bcObj.AddComponent(typeof(BezierCurve)) as BezierCurve;

        GameObject p0 = (GameObject) Instantiate(_sampleCubePrefab);
        p0.transform.position = p0Pos;
        p0.transform.parent = bcObj.transform;
        p0.transform.rotation = bezierRotation;
        p0.name = "P0";
        newCurve.p0 = p0Pos;
        GameObject p1 = (GameObject) Instantiate(_sampleCubePrefab);
        p1.transform.position = p1Pos;
        p1.transform.parent = bcObj.transform;
        p1.transform.rotation = bezierRotation;
        p1.name = "P1";
        newCurve.p1 = p1Pos;
        GameObject p2 = (GameObject) Instantiate(_sampleCubePrefab);
        p2.transform.position = p1Pos;
        p2.transform.parent = bcObj.transform;
        p2.transform.rotation = Quaternion.LookRotation(lastDir);
        p2.transform.RotateAround(p2.transform.position, lastDir, degrees);
        p2.transform.position += p2.transform.up * sideL;
        p2.name = "P2";
        newCurve.p2 = p2.transform.position;
        bezierRotation = p2.transform.rotation;

        newCurve.generateSegments(_sampleCubePrefab);

        lastPos = newCurve.segments[SegmentsPerCurve].position;
        lastDir = newCurve.segments[SegmentsPerCurve].tangent;
        if(curveN >= 3)
            Destroy(this.transform.GetChild(0).gameObject);
        BezierCurves[curveN%3] = newCurve;
        curveN++;
        generatedSegmentVertices = 0;
    }

    //Deprecated
    void spawnNewVertex()
    {
			Vertex newV = new Vertex();
			float radian = Random.Range(0f, 2.0f * Mathf.PI);
			newV.radian = radian;
			Vector3 spawnDir = tunnelRadius * new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0);
			Vector3 vertexPos = spawnPos + spawnDir;
			newV.position = vertexPos;
			vertices[vertexCount % 768] = newV;
			vertexCount++;
    }

    //Deprecated
    void moveVertices()
    {
      for(int i = 0; i < vertices.Length; i++){
        if(vertices[i] != null){
          vertices[i].position += new Vector3(0, 0, speed);
        }
      }
    }

    //Deprecated
    void rotateVertices(){
    float audioSum = 0;
        /*
        for(int i = 0; i < MainAudio._freqBand.Length; i++){
          audioSum += MainAudio._freqBand[i];
        } 
        */
      for(int i = 0; i < vertices.Length; i++){
        if(vertices[i] != null){
          vertices[i].rotate(rotationSpeed, tunnelRadius, audioSum, rotateRight);
        }
      }
    }

    //Deprecated
    void drawLines(){
        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;
        //print(lineRendererCount);

        if (lineRendererCount > maxLineRenderers)
        {
            for (int i = maxLineRenderers; i < lineRendererCount; i++)
            {
                Destroy(lineRenderers[i].gameObject);
            }

            int removedCount = lineRendererCount - maxLineRenderers;
            lineRenderers.RemoveRange(maxLineRenderers, removedCount);

            lineRendererCount -= removedCount;
        }

        if (maxConnections > 0 && maxLineRenderers > 0)
        {

            float maxDistanceSqr = maxDistance * maxDistance;
            for (int i = 0; i < 750; i++)
            {
                if (lrIndex == maxLineRenderers)
                {
                    break;
                }
                if(vertices[i] != null) {
                    Vector3 p1_position = vertices[i].position;
                    int connections = 0;

                    int j = i;
                    int k = 0;
                    while(k < 50){
                        j++;
                        k++;
                        if(vertices[(j+750)%750] != null){
                            Vector3 p2_position = vertices[(j+750)%750].position;
                            float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);
                            if (distanceSqr <= maxDistanceSqr)
                            {
                                LineRenderer lr;
                                if (lrIndex == lineRendererCount)
                                {
                                    Vector2 vertices = new Vector2(i, j);
                                    lineIndexes.Add(vertices);
                                    lr = Instantiate(lineRendererTemplate, _transform, false);
                                    lineRenderers.Add(lr);
                                    lineRendererCount++;
                                }
                                lr = lineRenderers[lrIndex];
                                lr.enabled = true;
                                //lr.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World ? true : false;

/* 
                GameObject trail = Instantiate(trailRendererTemplate, p1_position, new Quaternion(0,0,0,0));
                trailList.Add(trail); */
                //if(Random.Range(0f,100f) < MainAudio._audioBandBuffer[5]/2f){
                if(Random.Range(0f,100f) < 0.01f *Mathf.Pow(30,MainAudio._audioBandBuffer[6]) && i < 300){
                  //rail newTrail = new Trail();
                  GameObject trailObj = (GameObject) Instantiate(_sampleCubePrefab);
                  Trail newTrail = trailObj.AddComponent(typeof(Trail)) as Trail;
                    
                  newTrail.startPos = p1_position;
                  newTrail.endPos = p2_position;
                  newTrail.TrailRenderer = trailRendererTemplate;
                  newTrail.upVector = vertices[i].transform.up;

                }
                
                //GameObject newTrail = trailRoot.AddComponent<Trail>();
                //newTrail.AddComponent<Trail>();

                                lr.SetPosition(0, p1_position);
                                lr.SetPosition(1, p2_position);

                                lrIndex++;
                                connections++;

                                if (connections == maxConnections || lrIndex == maxLineRenderers)
                                {
                                    break;
                                }
                            }
                        } 
                    }
                }
            }
        }

        for (int i = lrIndex; i < lineRendererCount; i++)
        {
            lineRenderers[i].enabled = false;
        }
    }
}
