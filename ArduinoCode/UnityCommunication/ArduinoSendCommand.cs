using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ArduinoSendCommand : MonoBehaviour {

	private static SerialPort _ardPort;

	public string comPort = "COM3";
	//Could probably make this as a PlayerPrefs so it gets stored in registry but allowing a super user
	//to manually override the com port location should system specificaitons change. Ideally, I'd just
	//use a system that doesn't originally use a COM port...but silly me I have _two_ in mine.


	// Use this for initialization
	void Start () {
		_ardPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
		_ardPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);
		_ardPort.ReadTimeout = 10;
		_ardPort.Open();
	}
	
	// Update is called once per frame
	void Update () {
		try{
			//TO DO: Currently MonoDevelop does not natively support the Serial Read event. I'd need to make a socket for this, later...
			if(_ardPort.BytesToRead > 0){
				string data = _ardPort.ReadLine();
				Debug.Log(data);
			}
		}
		catch{

		}

	}

	//OnGUI is also called once per frame
	void OnGUI(){
		//Testing my state machine
		if(GUI.Button(new Rect(0,0,100,50), "RED ON")){
			Debug.Log("SEND RED ON");
			//_ardPort.WriteLine("1/1/");
			_ardPort.Write("100,1");
		}
		if(GUI.Button(new Rect(0,50,100,50), "RED OFF")){
			Debug.Log("SEND RED OFF");
			_ardPort.Write("100,0");
		}
		if(GUI.Button(new Rect(0,100,100,50), "BLUE ON")){
			Debug.Log("SEND BLUE ON");
			_ardPort.Write("200,1");
		}
		if(GUI.Button(new Rect(0,150,100,50), "BLUE OFF")){
			Debug.Log("SEND BLUE OFF");
			_ardPort.Write("200,0");
		}
	}

	//Does nothing, it turns out, due to MonoDevelop not supporting this event
	void DataRecievedHandler(object sender, SerialDataReceivedEventArgs e){
		Debug.Log("Message incoming!");
		SerialPort sp = (SerialPort)sender;
		string indata = sp.ReadLine();
		Debug.Log("Data recieved: " + indata);
	}

	void OnDisable(){
		_ardPort.Close();
	}
}
