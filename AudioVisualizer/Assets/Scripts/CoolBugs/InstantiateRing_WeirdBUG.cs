using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRing_WeirdBUG : MonoBehaviour
{   
    public GameObject _sampleCubePrefab;
    GameObject[] _sampleCube = new GameObject[512];
    public float _maxScale;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 512; i++){
            GameObject _instanceSampleCube = (GameObject) Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SampleCube" + "_" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 50;
            _sampleCube[i] = _instanceSampleCube;
        }
        this.transform.eulerAngles = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _sampleCube.Length; i++){
            if(_sampleCube != null){
                _sampleCube[i].transform.localScale = new Vector3(2, (MainAudio._samples[i] * _maxScale) + 2 ,(MainAudio._samples[i] * _maxScale));
                _sampleCube[i].transform.localPosition = new Vector3(
                    _sampleCube[i].transform.localPosition.x, 
                    (MainAudio._samples[i] * _maxScale)/2,
                    _sampleCube[i].transform.localPosition.z + (MainAudio._samples[i] * _maxScale)/2
                    );
                _sampleCube[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(1f,1f,1f));
            }
        }
    }
}
