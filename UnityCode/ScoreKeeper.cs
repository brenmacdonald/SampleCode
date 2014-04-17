using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {
	
	public enum Log{
		Install,
		Tester,
		Checklist,
		Generic		
	};

	public static int checklistSuccessVal = 1;
	public static int checklistDemeritVal = 1;
	public static int installSuccessVal = 10;
	public static int installDemeritVal = 5;
	public static int genericSuccessVal = 5;
	public static int genericDemeritVal = 5;
	public static int testerSuccessVal = 10;
	public static int testerDemeritVal = 5;
	
	private static int _checklistSuccessCount=0;
	private static int _checklistDemeritCount=0;
	private static int _installSuccessCount=0;
	private static int _installDemeritCount=0;
	private static int _testerSuccessCount=0;
	private static int _testerDemeritCount=0;
	private static int _genericSuccessCount=0;
	private static int _genericDemeritCount=0;
	private static string _installLog = null;
	private static string _testerLog = null;
	private static string _checklistLog = null;
	private static string _genericLog = null;
	
	public static int nScore = 0;
	public static int nTotalPositive = 0;
	public static int nTotalNegative = 0;
	
	
	
	public static int Score{
		get{ return nScore; }
		set{ nScore = value; }
	}
	
	public static int InstallPositive{
		get{ return _installSuccessCount; }
		set{ _installSuccessCount = value; }
	}
	public static int InstallDemerit{
		get{ return _installDemeritCount; }
		set{ _installDemeritCount = value; }
	}
	
	public static int ChecklistPositive{
		get{ return _checklistSuccessCount; }
		set{ _checklistSuccessCount = value; }
	}
	public static int ChecklistDemerit{
		get{ return _checklistDemeritCount; }
		set{ _checklistDemeritCount = value; }
	}
	
	public static int TesterPositive{
		get{ return _testerSuccessCount; }
		set{ _testerSuccessCount = value; }
	}
	public static int TesterDemerit{
		get{ return _testerDemeritCount; }
		set{ _testerDemeritCount = value; }
	}
	
	public static int GenericPositive{
		get{ return _genericSuccessCount; }
		set{ _genericSuccessCount = value; }
	}
	public static int GenericDemerit{
		get{ return _genericDemeritCount; }
		set{ _genericDemeritCount = value; }
	}
	
	public static string InstallLog{
		get{ return _installLog;}
		set{ _installLog = value;}
	}
	public static string TesterLog{
		get{ return _testerLog;}
		set{ _testerLog = value;}
	}
	public static string ChecklistLog{
		get{ return _checklistLog;}
		set{ _checklistLog = value;}
	}
	public static string GenericLog{
		get{ return _genericLog;}
		set{ _genericLog = value;}
	}

	public static int CalculateScore(){
		nScore = ((_installSuccessCount*installSuccessVal)-(_installDemeritCount*installDemeritVal)) + ((_checklistSuccessCount*checklistSuccessVal)-(_checklistDemeritCount*checklistDemeritVal)) + ((_testerSuccessCount*testerSuccessVal)-(_testerDemeritCount*testerDemeritVal)) + ((_genericSuccessCount*genericSuccessVal)-(_genericDemeritCount*genericDemeritVal));
		//nScore = ((_installSuccessCount*installSuccessVal)-(_installDemeritCount*installDemeritVal)) + ((_testerSuccessCount*testerSuccessVal)-(_testerDemeritCount*testerDemeritVal)) + ((_genericSuccessCount*genericSuccessVal)-(_genericDemeritCount*genericDemeritVal));
		return nScore;
	}
	public static void IncreaseScore(int nVal){
		nScore += nVal;
	}
	//resets only the score
	public static void ResetScore(){
		InstallPositive = 0;
		InstallDemerit = 0;
		TesterPositive = 0;
		TesterDemerit = 0;
		ChecklistPositive = 0;
		ChecklistDemerit = 0;
		GenericPositive = 0;
		GenericDemerit = 0;
	}
	//resets everything
	public static void ResetScoreKeeper(){
		InstallPositive = 0;
		InstallDemerit = 0;
		InstallLog = "";
		TesterPositive = 0;
		TesterDemerit = 0;
		TesterLog = "";
		ChecklistPositive = 0;
		ChecklistDemerit = 0;
		ChecklistLog = "";
		GenericPositive = 0;
		GenericDemerit = 0;
		GenericLog = "";
	}

	/// <summary>
	/// Adds a new line to the appropriate log with current timestamp from TimeKeeper.
	/// </summary>
	/// <param name='line'>
	/// Line to be inserted into the log.
	/// </param>
	/// <param name='type'>
	/// The log type.
	/// </param>
	public static void LogAddLine(string line, Log type){
		switch(type){
		case(Log.Install):
			InstallLog = InstallLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			break;
		case(Log.Tester):
			TesterLog = TesterLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			break;
		case(Log.Checklist):
			ChecklistLog = ChecklistLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			break;
		case(Log.Generic):
			GenericLog = GenericLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			break;
		default:
			break;
		}		
	}
		
	/// <summary>
	/// Adds a new line to the appropriate log with optional timestamp from TimeKeeper.
	/// </summary>
	/// <param name='line'>
	/// Line to be inserted into the log.
	/// </param>
	/// <param name='type'>
	/// The log type.
	/// </param>
	/// <param name='useTimeStamp'>
	/// Use time stamp.
	/// </param>
	public static void LogAddLine(string line, Log type, bool useTimeStamp){
		string time = null;
		if(useTimeStamp){
			time = TimeKeeper.HMSTimeDisplay() + ":  ";	
		}
		switch(type){
		case(Log.Install):
			InstallLog = InstallLog + "\n" + time + line;
			break;
		case(Log.Tester):
			TesterLog = TesterLog + "\n" + time + line;
			break;
		case(Log.Checklist):
			ChecklistLog = ChecklistLog + "\n" + time + line;
			break;
		case(Log.Generic):
			GenericLog = GenericLog + "\n" + time + line;
			break;
		default:
			break;
		}
	}
	
	/// <summary>
	/// Adds a new line to the appropriate log with current timestamp from TimeKeeper. Also adds to scores/demerits based on an int value passed. DEPRICATED
	/// </summary>
	/// <param name='line'>
	/// Line to be inserted into the log.
	/// </param>
	/// <param name='type'>
	/// The log type.
	/// <param name='val'>
	/// The score value to be added (or subtracted for demerits).
	/// </param>
	public static void LogAddLine(string line, Log type, int val){
		switch(type){
		case(Log.Install):
			InstallLog = InstallLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0){			
				_installSuccessCount = _installSuccessCount + val;
				InstallLog = InstallLog + " " + val*installSuccessVal + " points added.";
			}			
			if(val < 0){
				_installDemeritCount = _installDemeritCount - val;
				InstallLog = InstallLog + " " + val*installDemeritVal + " points removed.";
			}					
			break;
		case(Log.Tester):
			TesterLog = TesterLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0){			
				_testerSuccessCount = _testerSuccessCount + val;
				TesterLog = TesterLog + " " + val*testerSuccessVal + " points added.";
			}			
			if(val < 0){
				_testerDemeritCount = _testerDemeritCount - val;
				TesterLog = TesterLog + " " + val*testerDemeritVal + " points removed.";
			}
			break;
		case(Log.Checklist):
			ChecklistLog = ChecklistLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0){			
				_checklistSuccessCount = _checklistSuccessCount + val;
				ChecklistLog = ChecklistLog + " " + val*checklistSuccessVal + " points added.";
			}			
			if(val < 0){
				_checklistDemeritCount = _checklistDemeritCount - val;
				ChecklistLog = ChecklistLog + " " + val*checklistDemeritVal + " points removed.";
			}
			break;
		case(Log.Generic):
			GenericLog = GenericLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0){			
				_genericSuccessCount = _genericSuccessCount + val;
				GenericLog = GenericLog + " " + val*genericSuccessVal + " points added.";
			}			
			if(val < 0){
				_genericDemeritCount = _genericDemeritCount - val;
				GenericLog = GenericLog + " " + val*genericDemeritVal + " points removed.";
			}
			break;
		default:
			break;
		}		
	}
	
	
	/// <summary>
	/// Adds a new line to the appropriate log with current timestamp from TimeKeeper. Also adds to scores/demerits based on an int value passed. Optional: Boolean "undo" check should you want to remove positives or demerits.
	/// </summary>
	/// <param name='line'>
	/// Line to be inserted into the log.
	/// </param>
	/// <param name='type'>
	/// The log type.
	/// <param name='val'>
	/// The score value to be added (or subtracted for demerits).
	/// </param>
	/// <param name='undo'>
	/// If set to "true" then it will effectively remove points from positives or demerits. So a value of +1,true will remove 1 count from a positive. -1, true will remove one demerit.
	/// </param>
	public static void LogAddLine(string line, Log type, int val, bool undo){
		switch(type){
		case(Log.Install):
			InstallLog = InstallLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0 && undo){			
				_installSuccessCount = _installSuccessCount - val;
				InstallLog = InstallLog + " " + val*installSuccessVal + " points removed.";
			}
			if(val > 0 && !undo){			
				_installSuccessCount = _installSuccessCount + val;
				InstallLog = InstallLog + " " + val*installSuccessVal + " points added.";
			}	
			if(val < 0 && undo){
				_installDemeritCount = _installDemeritCount + val;
				InstallLog = InstallLog + " " + val*installDemeritVal + " points added.";
			}
			if(val < 0 && !undo){
				_installDemeritCount = _installDemeritCount - val;
				InstallLog = InstallLog + " " + val*installDemeritVal + " points removed.";
			}
			break;
		case(Log.Tester):
			TesterLog = TesterLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0 && undo){			
				_testerSuccessCount = _testerSuccessCount - val;
				TesterLog = TesterLog + " " + val*testerSuccessVal + " points removed.";
			}
			if(val > 0 && undo){			
				_testerSuccessCount = _testerSuccessCount + val;
				TesterLog = TesterLog + " " + val*testerSuccessVal + " points added.";
			}			
			if(val < 0 && undo){
				_testerDemeritCount = _testerDemeritCount - val;
				TesterLog = TesterLog + " " + val*testerDemeritVal + " points removed.";
			}
			if(val < 0 && !undo){
				_testerDemeritCount = _testerDemeritCount + val;
				TesterLog = TesterLog + " " + val*testerDemeritVal + " points added.";
			}
			break;
		case(Log.Checklist):
			ChecklistLog = ChecklistLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0 && undo){			
				_checklistSuccessCount = _checklistSuccessCount - val;
				ChecklistLog = ChecklistLog + " " + val*checklistSuccessVal + " points removed.";//Granted, it makes zero sense to have this condition for Checklists as you only should check this at the very end...
			}
			if(val > 0 && !undo){			
				_checklistSuccessCount = _checklistSuccessCount + val;
				ChecklistLog = ChecklistLog + " " + val*checklistSuccessVal + " points added.";
			}
			if(val < 0 && undo){
				_checklistDemeritCount = _checklistDemeritCount - val;
				ChecklistLog = ChecklistLog + " " + val*checklistDemeritVal + " points removed.";
			}
			if(val < 0 && !undo){
				_checklistDemeritCount = _checklistDemeritCount + val;
				ChecklistLog = ChecklistLog + " " + val*checklistDemeritVal + " points added.";
			}
			break;
		case(Log.Generic):
			GenericLog = GenericLog + "\n" + TimeKeeper.HMSTimeDisplay() + ":  " + line;
			if(val > 0 && undo){			
				_genericSuccessCount = _genericSuccessCount - val;
				GenericLog = GenericLog + " " + val*genericSuccessVal + " points removed.";
			}
			if(val > 0 && !undo){			
				_genericSuccessCount = _genericSuccessCount + val;
				GenericLog = GenericLog + " " + val*genericSuccessVal + " points added.";
			}
			if(val < 0 && undo){
				_genericDemeritCount = _genericDemeritCount + val;
				GenericLog = GenericLog + " " + val*genericDemeritVal + " points added.";
			}
			if(val < 0 && !undo){
				_genericDemeritCount = _genericDemeritCount - val;
				GenericLog = GenericLog + " " + val*genericDemeritVal + " points removed.";
			}
			break;
		default:
			break;
		}		
	}
}
