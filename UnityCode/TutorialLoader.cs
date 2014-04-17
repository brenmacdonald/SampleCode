using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
/*
 *A class for loading a set of tutorial messages from an XML file. This way it can be changed without doing 
 *another build. Other perks include being able to easily do multiple languages.
 * 
*/
public class TutorialLoader : MonoBehaviour {

	public string xmlTutorial = "Tutorial.xml";
	private Dictionary<string,string> _dTutorial = new Dictionary<string, string>();
	private int _prevSection = -1;
	private int _sectionCount = 0;

	//Called before any other MonoBehavior
	void Awake(){
		LoadTutorialXML();
	}



	void LoadTutorialXML(){
		XmlDocument doc = new XmlDocument();
		XmlNodeList tutorialNode;
		int i=0;
		//The XML resides in the _Data folder, this is the local path to it.
		//Alternative path would be to the MyDocuments folder if we worry about admin privileges
		if(System.IO.File.Exists(Application.dataPath + "/" + xmlTutorial))
		{
			doc.Load(Application.dataPath + "/" + xmlTutorial);
			tutorialNode = doc.GetElementsByTagName("TutorialMessage");
			foreach(XmlNode itemNode in tutorialNode)
			{
				string title = itemNode.Attributes["title"].Value;
				string message = itemNode.InnerText;
				string stateInt = itemNode.Attributes["state"].Value;
				string sSection = itemNode.Attributes["section"].Value;

				int nSection = -1;
				if(int.TryParse(sSection, out nSection)){
					if(nSection != _prevSection){
						SetTutorialMessage(_sectionCount + "_sectionHead", i.ToString());
						_sectionCount++;
						_prevSection = nSection;
					}
				}
				SetTutorialMessage(i + "_title", title);
				SetTutorialMessage(i + "_message", message);
				SetTutorialMessage(i + "_state", stateInt);
				SetTutorialMessage(i + "_section", sSection);
				i++;
			}
			SetTutorialMessage("count", i.ToString());
			SetTutorialMessage("sectionCount", _sectionCount.ToString());

			TutorialManager.dTutorial = _dTutorial;
		}
		else{
			Debug.Log ("TUTORIAL NOT LOADED");	
		}
	}

	void SetTutorialMessage(string key, string message){
		_dTutorial.Add(key, message);
	}
}
