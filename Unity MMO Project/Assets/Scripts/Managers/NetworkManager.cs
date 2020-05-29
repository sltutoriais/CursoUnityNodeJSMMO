using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Text.RegularExpressions;

public class NetworkManager : MonoBehaviour
{

	public static NetworkManager instance;

	public SocketIOComponent socket;

	public GameObject playerPrefab;

	public Dictionary<string,PlayerManager> networkPlayers = new Dictionary<string, PlayerManager> ();

	
    // Start is called before the first frame update
    void Start()
    {

		if (instance == null) 
		{

			instance = this;

			socket = GetComponent<SocketIOComponent> ();

			socket.On ("PONG",OnReceivePong);

			socket.On ("JOIN_SUCCESS", OnJoinSuccess);

			socket.On ("SPAWN_PLAYER",OnSpawnPlayer);

			socket.On ("UPDATE_POS_ROT",OnUpdatePosAndRot);

			socket.On ("UPDATE_ANIMATOR",OnUpdateAnimator );

			socket.On ("USER_DISCONNECTED",OnUserDisconnected);

			
		}
		else
		{
			Destroy (this.gameObject);
		}
    }

	void OnReceivePong(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();


		Debug.Log ("mensagem do servidor: "+result["message"]);
	}

	//metodo responsável por enviar um "ping" ao servidor
	public void SendPingToServer()
	{
		Dictionary<string,string> pack = new Dictionary<string,string> ();

		pack["message"] = "ping!!!";

		socket.Emit ("PING", new JSONObject (pack));//envia ao servidor nodejs

	}


	public void EmitJoin()
	{
		Dictionary<string,string> data = new Dictionary<string,string> ();

		data ["name"] = CanvasManager.instance.inputLogin.text;

		socket.Emit("JOIN_ROOM",new JSONObject(data));


	}

	public void OnJoinSuccess(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();

		PlayerManager new_player = Instantiate (playerPrefab, new Vector3 (0, 0, 0), Quaternion.identity).GetComponent<PlayerManager> ();

		new_player.gameObject.name = result ["id"];

		new_player.gameObject.GetComponentInChildren<TextMesh> ().text = result ["name"];

		new_player.isLocalPlayer = true;

		CanvasManager.instance.OpenScreen (1);

	}

	void OnSpawnPlayer(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();

		if(!networkPlayers.ContainsKey(result ["id"]))
		{
			PlayerManager new_player = Instantiate (playerPrefab,new Vector3(0,0,0),Quaternion.identity).GetComponent<PlayerManager>();

			new_player.gameObject.name = result ["id"];

			new_player.gameObject.GetComponentInChildren<TextMesh>().text = result["name"];

			networkPlayers [result ["id"]] = new_player;
		}



	}

	public void EmitPosAndRot(Vector3 _newPos, Quaternion _newRot)
	{
		Dictionary<string,string> data = new Dictionary<string,string> ();

		data ["position"] = _newPos.x + ":" + _newPos.y + ":" + _newPos.z;

		data ["rotation"] = _newRot.x + ":" + _newRot.y + ":" + _newRot.z + ":" + _newRot.w;

		socket.Emit ("MOVE_AND_ROT",new JSONObject(data));


	}


	void OnUpdatePosAndRot(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();

		PlayerManager netPlayer = networkPlayers [result ["id"]];
		Vector3 _pos = JsonToVector3 (result ["position"]);
		Vector4 _rot = JsonToVector4 (result["rotation"]);

		netPlayer.UpdatePosAndRot (_pos, new Quaternion (_rot.x, _rot.y, _rot.z, _rot.w));

	}

	public void EmitAnimation(string _animation)
	{
		Dictionary<string,string> data = new Dictionary<string,string> ();

		data ["animation"] = _animation;

		socket.Emit ("ANIMATION",new JSONObject(data));

	}

	void OnUpdateAnimator(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();

		PlayerManager netPlayer = networkPlayers [result ["id"]];
		Debug.Log ("receive animation: " + result ["animation"]);
		netPlayer.UpdateAnimator (result["animation"]);


	}

	void OnUserDisconnected(SocketIOEvent pack)
	{
		Dictionary<string,string> result = pack.data.ToDictionary ();

		if (GameObject.Find (result ["id"]))
		{
			Destroy (GameObject.Find (result ["id"]));

			networkPlayers.Remove (result["id"]);
		}

	}

	Vector3 JsonToVector3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,":");
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]),float.Parse(newString[2]));

		return newVector;

	}

	Vector4 JsonToVector4(string target ){

		Vector4 newVector;
		string[] newString = Regex.Split(target,":");
		newVector = new Vector4( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]),float.Parse(newString[3]));

		return newVector;

	}


}
