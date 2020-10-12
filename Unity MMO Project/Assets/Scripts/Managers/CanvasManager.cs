using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static CanvasManager  instance;

	public Canvas panelLobby;

	public InputField inputLogin;

	public int currentScreen;

	// Use this for initialization
	void Start () {

		// if don't exist an instance of this class
		if (instance == null) {

       
			//it doesn't destroy the object, if other scene be loaded
			DontDestroyOnLoad (this.gameObject);
			instance = this;// define the class as a static variable


		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

	}
	

	public void  OpenScreen(int _screenId)
	{
		switch (_screenId)
		{

		case 0:

			currentScreen = _screenId;
			panelLobby.enabled = true;



			break;

		case 1:

			currentScreen = _screenId;
			panelLobby.enabled = false;


			break;


		}

	}
}
