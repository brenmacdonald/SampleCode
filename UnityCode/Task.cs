using UnityEngine;
using System;
using System.Collections;

public class Task : MonoBehaviour {
	//Public variables
	public Texture[] textureArray 	= new Texture[9];
	public Texture textureBlank;
	public Texture textureCross;
	public Texture textureBlackScreen;
	public GUISkin levelGUI;
	public serialComm communicationScript;
	public labViewComm labViewScript;
	
	//Contains the numbers the player sees and the correct matches.
	private int[] iDataSet 			= new int[200];
	private int[] iCorrectAnswers 	= new int[200];
	
	//Player Input
	private int iIndex 				= 0;
	private int iCorrectIndex 		= 0;
	private int iTotalCorrect 		= 0;
	private int iTotalWrong 		= 0;
	private int iResponse 			= 0;
	private int iTimeStamp;
	private int iLogTime;
	
	//Card timing
	private float fNextTime 		= 0f;
	private float fStartDelay 		= 5.0f;
	private float fInvisDelay 		= 0.1f;
	private float fShowCardDelay 	= 1.5f;
	private int fReactionTime 		= 0; //CHANGE TO nReactionTime!!!!!! It gets *1000 so that in the log 0.123 seconds converts to 123ms with the int cast doing a quick and dirty flooring operation. Should tweak this...
	
	//Logging
	private string sLoggedData;
	private DateTime systemTime;
	//private Array logArray;
	
	//Flags
	private bool bStart 			= false;
	private bool bVisible 			= true;
	private bool bFirstLoopComplete = false;
	private bool bNewAnswer 		= true;
	private bool bAnswered 			= false;
	private bool bFixationCross 	= true;
	private bool bAfterInstructions	= false;
	
	//Instruction strings
	private string instructions;
	private string startTest = "Please locate the fixation + and press the ‘SPACEBAR’ to begin.";

	// Use this for initialization
	void Start () {
		Vector3 pos = transform.parent.position;
		pos.y = SettingsClass.fMeterVerticalPos;
		transform.parent.position = pos;
		Screen.showCursor = false;
		PopulateDataSet();
		GetInstructions();
		fNextTime = Time.time + fStartDelay;
		renderer.material.mainTexture = textureBlank;
		systemTime = DateTime.Now;
		iTimeStamp = systemTime.Hour*10000 + systemTime.Minute*100 + systemTime.Second;
		SetDataPath();
		communicationScript.SetSerial(SettingsClass.sComPort1, SettingsClass.iBaud1, SettingsClass.sParity1, SettingsClass.iDataBits1, SettingsClass.sStopBits1);
		labViewScript.SetUDP(SettingsClass.sIpAddress, SettingsClass.iPort);
		labViewScript.LabViewSync(2);
	}
	
	// Update is called once per frame
	void Update () {
		if(bStart){
		if(Time.time >= fNextTime && bVisible){		
			//renderer.enabled = true;		
			renderer.material.mainTexture = textureArray[iDataSet[iIndex]-1];
			communicationScript.SerialSync(); //Sends a synchonization pulse out to the ESU			
			//labViewScript.data[0] = 1;			
			labViewScript.LabViewSync(1);
			GetCurrentTime();
			bVisible = false;
			bFirstLoopComplete = true;
			//Debug.Log("Showing Number: " + iDataSet[iIndex]);
			bAnswered = false;
			fReactionTime = (int)(Time.time * 1000);// MAY WANT TO LOOK AT THIS ONE!!!
			fNextTime = Time.time + fShowCardDelay;
		}
		if(Time.time >= fNextTime && !bVisible){
			//renderer.enabled = false;
			renderer.material.mainTexture = textureBlank;
			//labViewScript.data[0]=0;
			labViewScript.LabViewSync(0);
			bVisible = true;
			if(iCorrectAnswers[iIndex]==1 && !bAnswered){
				//Debug.Log("++++++YOU MISSED ONE++++++++");
				//GetCurrentTime();
				//TO LOG  logger.logStringArray[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + iCorrectAnswers[iIndex] + "\t" + 0 + "\t" + 1500 + "\t" + "A" + "\t" + (iIndex+1);
				//Debug.Log(iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "A" + "\t" + (iIndex+1));
				//SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "A" + "\t" + (iIndex+1);
				SetLogWrongSkip();
				iTotalWrong++;			
			}
			else{
				if(!bAnswered){
					//GetCurrentTime();
					//SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "A" + "\t" + (iIndex+1);
					SetLogCorrectSkip();
				}
			}
			iIndex++;
			bNewAnswer = true;
			if(iIndex>=200){
				iIndex=0;
				//Debug.Log("Total Correct Answers: " + iTotalCorrect);
				//Debug.Log("Total Wrong Answers: " + iTotalWrong);
				//fNextTime = Time.time + 500;
				//Have this send the user back to the main menu as it would mean the test was over...
				Save.CreateTextFile(SettingsClass.sFileLocation);
				for(int iSaveIndex = 0;iSaveIndex<200;iSaveIndex++){
					Debug.Log(SettingsClass.iTaskALog[iSaveIndex]);
					//Save.SaveTextFile(SettingsClass.sFileLocation, SettingsClass.iTaskALog[iSaveIndex]);
					//Save.SaveTextFile(SettingsClass.iTaskALog[iSaveIndex]);
					SaveLog(iSaveIndex);			
				}
				Save.CloseTextFile();
				//SettingsClass.iTaskAIterations++;
				IncreaseTaskIteration();
				labViewScript.LabViewSync(8);
				//Application.LoadLevel("EEG_menu");
				//This is a cluster view function call. Needed for synchonizing multiple instances of Unity.
				if(getReal3D.Cluster.isMaster){
					getReal3D.ClusterView cv = gameObject.GetComponent<getReal3D.ClusterView>();
					if(cv != null){
						cv.RPC("LoadLevel", "EEG_menu");
					}
					else{
						LoadLevel("EEG_menu");
					}
				}
			}
			//logArray.Push		
			fNextTime = Time.time + fInvisDelay;
		}
		if(Input.GetKeyUp(KeyCode.Space)){
			if(iCorrectAnswers[iIndex]==1 && bNewAnswer){
				//Debug.Log("You are correct!");
				iTotalCorrect++;
				//GetCurrentTime();
				if(!bAnswered){
					fReactionTime = (int)(Time.time*1000 - fReactionTime); //CORRECT ME!
					//fReactionTime = fReactionTime*1000;
				}
				//Debug.Log(iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1));
				//SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1);
				SetLogCorrectEntry();
				iResponse = 1;
				bAnswered = true;
				bNewAnswer = false;
			}
			else{
				//Debug.Log("Wrong. You're so wrong.");
				iResponse = 1;
				//GetCurrentTime();
				if(!bAnswered){
					fReactionTime = (int)(Time.time*1000 - fReactionTime);//CORRECT ME
					//fReactionTime = fReactionTime*1000;
				}
				//Debug.Log(iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1));
				//SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1);
				SetLogWrongEntry();
				bAnswered = true;
				iTotalWrong++;
			}
		}
	}
	else{
		if(Input.GetKeyUp(KeyCode.Space) && bAfterInstructions){
			bStart = true;
			fNextTime = Time.time + fStartDelay;
			renderer.material.mainTexture = textureCross;
			//labViewScript.data[0]=4;
			labViewScript.LabViewSync(4);
		}
		if(Input.GetKeyUp(KeyCode.Space) && !bAfterInstructions){
			renderer.material.mainTexture = textureCross;
			bAfterInstructions=true;
		}
	}
	
	if(Input.GetKeyUp(KeyCode.Escape)){
		Save.CreateTextFile(SettingsClass.sFileLocation);
		for(int iQuitIndex = 0;iQuitIndex<iIndex;iQuitIndex++){
			//Debug.Log(SettingsClass.iTaskALog[iSaveIndex]);
			//Save.SaveTextFile(SettingsClass.sFileLocation, SettingsClass.iTaskALog[iSaveIndex]);
			//Save.SaveTextFile(SettingsClass.iTaskALog[iQuitIndex]);	
			SaveLog(iQuitIndex);				
		}
		Save.CloseTextFile();
		//SettingsClass.iTaskAIterations++;
		IncreaseTaskIteration();
		labViewScript.LabViewSync(8);
	
		//Application.LoadLevel("EEG_menu");
		if(getReal3D.Cluster.isMaster){
			getReal3D.ClusterView cv2 = gameObject.GetComponent<getReal3D.ClusterView>();
			if(cv2 != null){
				cv2.RPC("LoadLevel", "EEG_menu");
			}
			else{
				LoadLevel("EEG_menu");
			}
		}
	}
	}
	
	void PopulateDataSet(){
		//DataSets.GetValues();
		switch(DataSets.iTaskNumber){
		case 0:
			iDataSet = DataSets.iDataSetA;
			iCorrectAnswers = DataSets.iCorrectAnswersA;
			break;
		case 1:
			iDataSet = DataSets.iDataSetB;
			iCorrectAnswers = DataSets.iCorrectAnswersB;
			break;
		case 2:
			iDataSet = DataSets.iDataSetC;
			iCorrectAnswers = DataSets.iCorrectAnswersC;
			break;
		case 3:
			iDataSet = DataSets.iDataSetD;
			iCorrectAnswers = DataSets.iCorrectAnswersD;
			break;
		case 4:
			iDataSet = DataSets.iDataSetE;
			iCorrectAnswers = DataSets.iCorrectAnswersE;
			break;
		case 5:
			iDataSet = DataSets.iDataSetF;
			iCorrectAnswers = DataSets.iCorrectAnswersF;
			break;
		default:
			iDataSet = DataSets.iDataSetA;
			iCorrectAnswers = DataSets.iCorrectAnswersA;
			break;
		}

	}
	void SetDataPath(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskA_" + SettingsClass.iTaskAIterations + ".txt";
			break;
		case 1:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskB_" + SettingsClass.iTaskBIterations + ".txt";
			break;
		case 2:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskC_" + SettingsClass.iTaskCIterations + ".txt";
			break;
		case 3:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskD_" + SettingsClass.iTaskDIterations + ".txt";
			break;
		case 4:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskE_" + SettingsClass.iTaskEIterations + ".txt";
			break;
		case 5:
			SettingsClass.sFileLocation = Application.dataPath + "/../" + "Data/" + SettingsClass.sSubject + "/" + SettingsClass.sSubject + "_" + SettingsClass.sSession + "_TaskF_" + SettingsClass.iTaskFIterations + ".txt";
			break;
		default:
			break;
		}
	}
	
	void SetLogCorrectEntry(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1);
			break;
		case 1:
			SettingsClass.iTaskBLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "B" + "\t" + (iIndex+1);
			break;
		case 2:
			SettingsClass.iTaskCLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "C" + "\t" + (iIndex+1);
			break;
		case 3:
			SettingsClass.iTaskDLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "D" + "\t" + (iIndex+1);	
			break;
		case 4:
			SettingsClass.iTaskELog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "E" + "\t" + (iIndex+1);
			break;
		case 5:
			SettingsClass.iTaskFLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "F" + "\t" + (iIndex+1);
			break;
		default:
			break;
		}
	}
	
	void SetLogWrongEntry(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "A" + "\t" + (iIndex+1);
			break;
		case 1:
			SettingsClass.iTaskBLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "B" + "\t" + (iIndex+1);
			break;
		case 2:
			SettingsClass.iTaskCLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "C" + "\t" + (iIndex+1);
			break;
		case 3:
			SettingsClass.iTaskDLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "D" + "\t" + (iIndex+1);
			break;
		case 4:
			SettingsClass.iTaskELog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "E" + "\t" + (iIndex+1);
			break;
		case 5:
			SettingsClass.iTaskFLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Space" + "\t" + fReactionTime + "\t" + "F" + "\t" + (iIndex+1);
			break;
		default:
			break;
		}

	}

	void SetLogCorrectSkip(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "A" + "\t" + (iIndex+1);
			break;
		case 1:
			SettingsClass.iTaskBLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "B" + "\t" + (iIndex+1);
			break;
		case 2:
			SettingsClass.iTaskCLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "C" + "\t" + (iIndex+1);
			break;
		case 3:
			SettingsClass.iTaskDLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "D" + "\t" + (iIndex+1);
			break;
		case 4:
			SettingsClass.iTaskELog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "E" + "\t" + (iIndex+1);
			break;
		case 5:
			SettingsClass.iTaskFLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Nothing" + "\t" + "Nothing" + "\t" + 0 + "\t" + "F" + "\t" + (iIndex+1);
			break;
		default:
			break;
		}

	}

	void SetLogWrongSkip(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.iTaskALog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "A" + "\t" + (iIndex+1);
			break;
		case 1:
			SettingsClass.iTaskBLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "B" + "\t" + (iIndex+1);
			break;
		case 2:
			SettingsClass.iTaskCLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "C" + "\t" + (iIndex+1);
			break;
		case 3:
			SettingsClass.iTaskDLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "D" + "\t" + (iIndex+1);
			break;
		case 4:
			SettingsClass.iTaskELog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "E" + "\t" + (iIndex+1);
			break;
		case 5:
			SettingsClass.iTaskFLog[iIndex] = iTimeStamp + "\t" +iDataSet[iIndex] + "\t" + "Space" + "\t" + "Nothing" + "\t" + 0 + "\t" + "F" + "\t" + (iIndex+1);
			break;
		default:
			break;
		}
	}

void SaveLog(int SaveIndex ){
	switch(DataSets.iTaskNumber){
		case 0:
			Save.SaveTextFile(SettingsClass.iTaskALog[SaveIndex]);
			break;
		case 1:
			Save.SaveTextFile(SettingsClass.iTaskBLog[SaveIndex]);
			break;
		case 2:
			Save.SaveTextFile(SettingsClass.iTaskCLog[SaveIndex]);
			break;
		case 3:
			Save.SaveTextFile(SettingsClass.iTaskDLog[SaveIndex]);
			break;
		case 4:
			Save.SaveTextFile(SettingsClass.iTaskELog[SaveIndex]);
			break;
		case 5:
			Save.SaveTextFile(SettingsClass.iTaskFLog[SaveIndex]);
			break;
		default:
			break;
	}
}

	void IncreaseTaskIteration(){
		switch(DataSets.iTaskNumber){
		case 0:
			SettingsClass.iTaskAIterations++;
			break;
		case 1:
			SettingsClass.iTaskBIterations++;
			break;
		case 2:
			SettingsClass.iTaskCIterations++;
			break;
		case 3:
			SettingsClass.iTaskDIterations++;
			break;
		case 4:
			SettingsClass.iTaskEIterations++;
			break;
		case 5:
			SettingsClass.iTaskFIterations++;
			break;
		default:
			break;
		}
	}

	void GetInstructions(){
		switch(DataSets.iTaskNumber){
		case 0:
			instructions = DataSets.sInstructionsA;
			break;
		case 1:
			instructions = DataSets.sInstructionsB;
			break;
		case 2:
			instructions = DataSets.sInstructionsC;
			break;
		case 3:
			instructions = DataSets.sInstructionsD;
			break;
		case 4:
			instructions = DataSets.sInstructionsE;
			break;
		case 5:
			instructions = DataSets.sInstructionsF;
			break;
		default:
			break;
		}
	}

	void GetCurrentTime(){
		systemTime = DateTime.Now;
		iTimeStamp = systemTime.Hour*10000 + systemTime.Minute*100 + systemTime.Second;	
	}

	void OnGUI(){
		if(!bStart && !bAfterInstructions){
			GUI.skin = levelGUI;
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),textureBlackScreen);
			GUI.Label(new Rect((Screen.width/2)-400,(Screen.height/2)-300,800,600),instructions);
		}
		if(!bStart && bAfterInstructions){
			GUI.skin = levelGUI;
			GUI.Label(new Rect((Screen.width/2)-400,(Screen.height/2)-300,800,600),startTest);
		}
	}

	void LoadLevel(string levelName){
		Application.LoadLevel(levelName);
	}

}
