using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//A class which controlls how the ui buttons control the music
public class MusicSelector : MonoBehaviour
{
	public MainAudio AudioHand;
	GameObject song0;
	GameObject song1;
	GameObject song2;
	GameObject hide;
	GameObject show;
	int selectedSong = 0;
    // Start is called before the first frame update
    //Finds All Ui elements and initialises them correctly
    void Start()
    {
    	song0 = GameObject.Find("Song0");
    	song1 = GameObject.Find("Song1");
    	song2 = GameObject.Find("Song2");
    	song0.GetComponentInChildren<Text>().text = AudioHand.a_clips[0].name;
    	song1.GetComponentInChildren<Text>().text = AudioHand.a_clips[1].name;
    	song2.GetComponentInChildren<Text>().text = AudioHand.a_clips[2].name;
    	hide = GameObject.Find("Hide Songs");
    	show = GameObject.Find("Show Songs");

        GameObject.Find("Song0").GetComponent<UnityEngine.UI.Button>().Select();
		/*
		song0.SetActive(false);
		song1.SetActive(false);
		song2.SetActive(false);
		
		//hide.SetActive(false);
		//show.SetActive(true);
		*/
        GameObject visRoot = GameObject.Find("VisualizationRoot");
        AudioHand = visRoot.GetComponent<MainAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSong1(){
    	AudioHand.switchSong(0);
    	selectedSong = 0;
		AudioHand.resetValues();
	}

    //Plays second Song
    public void playSong2(){
    	AudioHand.switchSong(1);
    	selectedSong = 1;
		AudioHand.resetValues();
    }

    //Plays third Song
    public void playSong3(){
		AudioHand.switchSong(2);
		selectedSong = 2;
		AudioHand.resetValues();
	}

	//Deprecated
	public void hideList(){
		song0.SetActive(false);
		song1.SetActive(false);
		song2.SetActive(false);
		hide.SetActive(false);
		show.SetActive(true);
	}

	//Deprecated
	public void showList(){
		song0.SetActive(true);
		song1.SetActive(true);
		song2.SetActive(true);
		hide.SetActive(true);
		show.SetActive(false);
		show.GetComponent<CanvasGroup>().alpha = 0f;

        GameObject.Find("Song" + selectedSong).GetComponent<UnityEngine.UI.Button>().Select();
	}             
}
