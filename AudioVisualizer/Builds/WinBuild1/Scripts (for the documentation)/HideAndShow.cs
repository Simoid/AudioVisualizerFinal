using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shows and Hides the UI
public class HideAndShow : MonoBehaviour
{
    public GameObject buttonToHide;
    public bool mouseIn = false;
    public int fadeOutTime = 300;


    // Start is called before the first frame update
    void Start()
    {
        //print(gameObject);

        GameObject.Find("Canvas").GetComponent<CanvasGroup>().alpha = 1f;
    }

    // Update is called once per frame
    //Fades out songlist when mouse is not inside
    void Update()
    {   

        if(fadeOutTime > 0 && !mouseIn){
        	GameObject.Find("Canvas").GetComponent<CanvasGroup>().alpha -= 0.01f;// * (float)Time.deltaTime;
        }
        fadeOutTime--;
    }

    //Shows songlist when entering object
    public void mouseEnter(){
        GameObject.Find("Canvas").GetComponent<CanvasGroup>().alpha = 0.7f;
        mouseIn = true;
        fadeOutTime = 300;
    }

    //Starts fading out songlist when mouse is moved out
    public void mouseExit(){
        mouseIn = false;
        fadeOutTime = 300;
    }

}
