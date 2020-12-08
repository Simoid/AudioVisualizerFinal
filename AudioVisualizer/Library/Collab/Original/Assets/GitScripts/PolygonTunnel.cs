using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public LineRenderer lineRendererTemplate;
  public GameObject trailRendererTemplate;
    public int maxConnections = 5;
    public int maxLineRenderers = 2000;
    public int outerTunnelCount = 10;
    public int outerTunnelRadius = 40;
    List<LineRenderer> lineRenderers = new List<LineRenderer>();
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


    List<GameObject> trailList = new List<GameObject>();

    void Start()
    {
      spawnNewBezierCurve();
      initVertices();
      spawnNewBezierCurve();
      initVertices();
      spawnNewBezierCurve();
      initVertices();
      drawLines();
      currentColor1 = originalColor1;
      currentColor2 = originalColor2;
      v1 = 2 * BezierCurves[curveN%3].p0 - 4 * BezierCurves[curveN%3].p1 + 2 * BezierCurves[curveN%3].p2; 
      v2 = -2*BezierCurves[curveN%3].p0 + 2 * BezierCurves[curveN%3].p1;

     
    }

    // Update is called once per frame
    void Update()
    { 
      oldColor1 = currentColor1;

/*       currentColor1 = MainAudio._audioBandBuffer[0] * new Color(1f,0f,0f,1f)
                      + MainAudio._audioBandBuffer[3] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[7] * new Color(0f,0f,1f,1f); */
  /*     currentColor2 = oldColor2 *0.8f + 0.2f* (MainAudio._audioBandBuffer[1] * new Color(1f,0f,0f,1f)
      + MainAudio._audioBandBuffer[4] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[6] * new Color(0f,0f,1f,1f)); */

      //WOrks but is not bound to FPS
      /*  currentColor1 = oldColor1 * 0.9f * Time.deltaTime + Time.deltaTime * 0.1f * (MainAudio._audioBandBuffer[1] * new Color(1f,0f,0f,1f)
      + MainAudio._audioBandBuffer[4] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[6] * new Color(0f,0f,1f,1f)); */
      
      currentColor1 = Color.Lerp(oldColor1, (MainAudio._audioBandBuffer[1] * new Color(1f,0f,0f,1f)
      + MainAudio._audioBandBuffer[4] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[6] * new Color(0f,0f,1f,1f)),Time.deltaTime);
     /*  currentColor1 = oldColor1 * 54f * Time.deltaTime + Time.deltaTime * 0.f * (MainAudio._audioBandBuffer[1] * new Color(1f,0f,0f,1f)
      + MainAudio._audioBandBuffer[4] * new Color(0f,1f,0f,1f) + MainAudio._audioBandBuffer[6] * new Color(0f,0f,1f,1f)); */

      /* Texture2D test = new Texture2D(1,1);
      test.SetPixel(1,1,currentColor1);
      test.wrapMode = TextureWrapMode.Repeat;
      test.Apply(); */

      /*
      spawnNewVertex();
      spawnNewVertex();
      spawnNewVertex();
      moveVertices();
      rotateVertices();
      drawLines();
      */
      moveCamera();
      //rotateSegments();
      drawLines();
      generateVertices();

      for(int i = 0; i < lineRenderers.Count; i++){
        //lineRenderers[i].startColor = new Color(192f/255f,10f/255f,6f/255f,1f) * (MainAudio._audioBandBuffer[0] + 0.3f) + new Color(1f,1f,1f,1f) * MainAudio._audioBandBuffer[0];
        //lineRenderers[i].endColor = new Color(255f/255f,123f/255f,81f/255f,1f) *(MainAudio._audioBandBuffer[0] + 0.3f) + new Color(1f,1f,1f,1f) * MainAudio._audioBandBuffer[0];
        lineRenderers[i].startColor = currentColor1;
        lineRenderers[i].endColor = currentColor1;
      }

      //print(currentColor1);
    }

    void initVertices(){
      for(int i = 0; i < SegmentsPerCurve; i++){
        generateVertices();
      }
    }

    void generateVertices(){
      if(generatedSegmentVertices == SegmentsPerCurve){
        return;
      }
      BezierCurves[(curveN-1)%3].segments[generatedSegmentVertices].generateVertices(_sampleCubePrefab, tunnelRadius);
      for(int i = 0; i < vertexDensity; i++){
        vertices[(((curveN-1)%3)*250 + generatedSegmentVertices*5 + i)] = BezierCurves[(curveN-1)%3].segments[generatedSegmentVertices].vertices[i];
      }
      generatedSegmentVertices++;
    }

    void printshit(){
      for(int i = 0; i < vertices.Length; i++){
        print(vertices[i].position.x + " " + vertices[i].position.y + " " + vertices[i].position.z );
      }
    }

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

    void rotateSegments(){
      for(int i = 0; i < segments.Length; i++){
        segments[i].rotateVertices(rotationSpeed);
        for(int j = 0; j < 5; j++){
            vertices[i*5 + j] = segments[i].vertices[j];
          }
      }
    }

    void spawnNewBezierCurve(){
		//print(lastPos.x + " " + lastPos.y + " " + lastPos.z);
		//print(lastDir.x + " " + lastDir.y + " " + lastDir.z);

		float forwardL = Random.Range(-100f, 100f) + 300f;
		//float sideL = Random.Range(-50f, 50f) + 200f;
		float sideL = forwardL;
		float degrees = Random.Range(1f, 360f);
		Vector3 p0Pos = lastPos;
		Vector3 p1Pos = lastPos + (forwardL * lastDir);

		//print("a: " + forwardL + " b: " + sideL + " c: " + degrees);

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
    	//newCurve.generateOuterTunnels(_sampleCubePrefab,10,20f);
		/*
		for(int i = 0; i < 51; i++){
		
		CurveSegment cs = newCurve.segments[i];
		Vector3 position = cs.position;
		Vector3 tangent = cs.tangent;
		Vector3 perpt = cs.perpt;
		segments[i] = cs; 
		print("Segment "+i+"! pos = (" + position.x + ", " + position.y + ", " + position.z + ") Tangent = (" + tangent.x + ", " + tangent.y + ", " + tangent.z + ") Perpt = (" + perpt.x + ", " + perpt.y + ", " + perpt.z + ")!");
		
		for(int j = 0; j < 5; j++){
			//if(curveN >= 3)
			//print(vertices[((curveN%3)*500 + i*5 + j)].position.x + ", " + vertices[((curveN%3)*500 + i*5 + j)].position.y + ", " + vertices[((curveN%3)*500 + i*5 + j)].position.z  + " -> ");
			vertices[((curveN%3)*250 + i*5 + j)] = newCurve.segments[i].vertices[j];
			//print(vertices[((curveN%3)*500 + i*5 + j)].position.x + ", " + vertices[((curveN%3)*500 + i*5 + j)].position.y + ", " + vertices[((curveN%3)*500 + i*5 + j)].position.z);
		}
		}
		
	*/
		lastPos = newCurve.segments[SegmentsPerCurve].position;
		lastDir = newCurve.segments[SegmentsPerCurve].tangent;
		if(curveN >= 3)
			Destroy(this.transform.GetChild(0).gameObject);
		BezierCurves[curveN%3] = newCurve;
		curveN++;
		generatedSegmentVertices = 0;
    }

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

    void moveVertices()
    {
      for(int i = 0; i < vertices.Length; i++){
        if(vertices[i] != null){
          vertices[i].position += new Vector3(0, 0, speed);
        }
      }
    }

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

    void drawLines(){
        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;

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
