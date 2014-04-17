#pragma strict

var skinCalibrate : GUISkin;
var fInterval = 0.05f;
var cameraContainer : GameObject;
private var pos : Vector3;
private var cv : getReal3D.ClusterView;
private var loadOnce : boolean = true;

function Start () {
	Debug.Log(transform.position);
	var rotationTarget = Quaternion.Euler(0,16.428,0);
	cameraContainer.transform.rotation = rotationTarget;
}

function Update () {
	cv = gameObject.GetComponent(getReal3D.ClusterView);
	pos = transform.position;
	if(getReal3D.Cluster.isMaster){
		if(Input.GetKeyUp(KeyCode.UpArrow) && (pos.y + fInterval) < 1.71f){//&& pos.y < 0.16
			settings.fMeterVerticalPos += fInterval;
			pos.y = settings.fMeterVerticalPos;	
			Debug.Log(pos.y);
		}
		if(Input.GetKeyUp(KeyCode.DownArrow) && (pos.y - fInterval) > -0.05f){// && pos.y > -1.6f
			settings.fMeterVerticalPos -= fInterval;
			pos.y = settings.fMeterVerticalPos;
			Debug.Log(pos.y);
		}
		if(Input.GetKeyUp(KeyCode.Escape)){		
			settings.fMeterVerticalPos = 1.1f;
			pos.y = settings.fMeterVerticalPos;	
			Debug.Log(pos.y);
		}
	
		if(cv != null){
			//transform.position = pos;
			cv.RPC("NewYPosition", pos);
		}
		else{
			NewYPosition(pos);
		}
	}
	
}

function OnGUI(){
	if (!getReal3D.GUI.BeginGUI()) return;
	GUI.skin = skinCalibrate;
	GUI.Label(Rect(0,0,200,50), "Calibration");
	GUI.Label(Rect(0,50,200,50), "Use the arrow keys to adjust the gas meter.");
	
	//TEST CODE
	//GUI.Label(Rect(Screen.width-200,0,200,50), "" + settings.fMeterVerticalPos);
	//GUI.Label(Rect(Screen.width-200,50,200,50), "" + Mathf.Round((settings.fMeterVerticalPos-1.1f)*100f)/100f);
	
	if((GUI.Button(Rect(0,100,150,50),"Save") || Input.GetKeyUp(KeyCode.Space)) && loadOnce){
		//Application.LoadLevel("EEG_menu");
		if(getReal3D.Cluster.isMaster){
				//var cv : getReal3D.ClusterView = gameObject.GetComponent(getReal3D.ClusterView);
				if(cv != null){
					cv.RPC("LoadLevel", "EEG_menu");
				}
				else{
					LoadLevel("EEG_menu");
				}
				loadOnce = false;
		}
	}
	if(GUI.Button(Rect(0,150,150,50),"Reset") || Input.GetKeyUp(KeyCode.R)){
		settings.fMeterVerticalPos = 1.1f;
		pos.y = settings.fMeterVerticalPos;
		//transform.position = pos;
		if(getReal3D.Cluster.isMaster){
			if(cv != null){
			//transform.position = pos;
			cv.RPC("NewYPosition", pos);
			}
			else{
				NewYPosition(pos);
			}
		}
	}
}

function LoadLevel(levelName : String){
	Application.LoadLevel(levelName);
}

function NewYPosition(vector : Vector3){
	transform.position = vector;
	settings.fMeterVerticalPos = vector.y;
}