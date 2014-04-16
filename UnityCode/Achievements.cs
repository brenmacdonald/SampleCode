using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * An achievement system I created for a Global Game Jam game I worked on back in January.
 * It's quick, it's simple, it was made for the purposes of having a playable and working
 * prototype in a project who's lifetime was under 48 hours from being given the task to turning
 * in a deliverable demo.
 * 
 * It's available for download over at the Global Game Jam site, "Bunnies and Robots"
 * 
 * Brendan Macdonald
*/


public class Achievements : MonoBehaviour {

	public GameObject achievementPanel;
	public UILabel labelAchievement;
	public UILabel labelSubTitle;



	private bool _HumanMassacreShown = false;
	private bool _RobotMassacreShown = false;

	private bool _isShowing = false;
	private float _timeDelay = 2.0f;
	private float _wait = 0.0f;

	public delegate bool CheckAchievement();

	public class Achievement{
		public string title;
		public string subTitle;
		public bool achieved;
		public CheckAchievement check;
	}
	public List<Achievement> _achievements;
	// Use this for initialization
	void Start () {
		if(achievementPanel != null){
			NGUITools.SetActive(achievementPanel, false);
		}
		_achievements = new List<Achievement>();

		//copy
		Achievement a = new Achievement();
		a.achieved = false;
		a.title = "Git-r-Done!";
		a.subTitle = "50 Human Kills";
		a.check = () => {return Master.GameStats.GetKills(typeof(HumanUnit)) >= 50;};
		_achievements.Add(a);

		//Copy from here on
		a = new Achievement();
		a.achieved = false;
		a.title = "EXTERMINATE!";
		a.subTitle = "50 Robot Kills";
		a.check = () => {return Master.GameStats.GetKills(typeof(RobotUnit)) >= 50;};
		_achievements.Add(a);
		//end copy
		a = new Achievement();
		a.achieved = false;
		a.title = "Eat Your Vegetables!";
		a.subTitle = "50 Carrots Eaten";
		a.check = () => {return Master.GameStats.GetDeaths(typeof(GrassStatic)) >= 50;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "That Rabbit's Dynamite!";
		a.subTitle = "50 Rabbit Kills";
		a.check = () => {return Master.GameStats.GetKills(typeof(BunnyUnit)) >= 50;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Braaaaaaiiiinnnss";
		a.subTitle = "Spawn a Zombie";
		a.check = () => {return Master.GameStats.GetBirths(typeof(ZombieUnit)) >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "SYSTEMS ONLINE";
		a.subTitle = "Spawn a Robot";
		a.check = () => {return Master.GameStats.GetBirths(typeof(RobotUnit)) >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Afternoon of the Dead";
		a.subTitle = "50 Zombie Kills";
		a.check = () => {return Master.GameStats.GetKills(typeof(ZombieUnit)) >= 50;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "What Have We Done?!";
		a.subTitle = "Build a Factory";
		a.check = () => {return Master.GameStats.GetBirths(typeof(FactoryStatic)) >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Skynet Open Business";
		a.subTitle = "Build 10 Factories";
		a.check = () => {return Master.GameStats.GetBirths(typeof(FactoryStatic)) >= 10;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Movin' on up";
		a.subTitle = "Build a House";
		a.check = () => {return Master.GameStats.GetBirths(typeof(HouseStatic)) >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Git off my lawn";
		a.subTitle = "Build 10 Houses";
		a.check = () => {return Master.GameStats.GetBirths(typeof(HouseStatic)) >= 10;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "You Be Assimilated";
		a.subTitle = "Spawn 200 Robots";
		a.check = () => {return Master.GameStats.GetBirths(typeof(RobotUnit)) >= 200;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Like Rabbits...";
		a.subTitle = "Spawn 200 Bunnies";
		a.check = () => {return Master.GameStats.GetBirths(typeof(BunnyUnit)) >= 200;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Like Rabbits...";
		a.subTitle = "Spawn 200 Humans";
		a.check = () => {return Master.GameStats.GetBirths(typeof(HumanUnit)) >= 200;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "The Swaggering Dead";
		a.subTitle = "Spawn 200 Zombies";
		a.check = () => {return Master.GameStats.GetBirths(typeof(ZombieUnit)) >= 200;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Lumberjack";
		a.subTitle = "Destroyed all trees";
		a.check = () => {return Master.GameStats.AllTreesDestroyed;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Addicted to Rock";
		a.subTitle = "Destroy all rocks";
		a.check = () => {return Master.GameStats.AllRocksDestroyed;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Kill All Humans";
		a.subTitle = "What he said";
		a.check = () => {return Master.GameStats.GetLiving(typeof(HumanUnit)) <= 0;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "What Did They Ever Do To You?";
		a.subTitle = "Kill all bunnies";
		a.check = () => {return Master.GameStats.GetLiving(typeof(BunnyUnit)) <= 0;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Del *.*";
		a.subTitle = "Kill all robots";
		a.check = () => {return Master.GameStats.GetBirths(typeof(RobotUnit)) >= 1 && Master.GameStats.GetLiving(typeof(RobotUnit)) <= 0;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Rule Two: The Double Tap";
		a.subTitle = "Kill all zombies";
		a.check = () => {return Master.GameStats.GetBirths(typeof(ZombieUnit)) >= 1 && Master.GameStats.GetLiving(typeof(ZombieUnit)) <= 0;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Quit Eating Yourself";
		a.subTitle = "Humans resort to cannibalism";
		a.check = () => {return Master.GameStats.HumanCannibalism >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "I don't want to set the world on fire";
		a.subTitle = "Death Bot has come online";
		a.check = () => {return Master.GameStats.GetBirths(typeof(DeathBot)) >= 1;};
		_achievements.Add(a);

		a = new Achievement();
		a.achieved = false;
		a.title = "Achievement System Unlocked!";
		a.subTitle = "You started the game...";
		a.check = () => {return Time.time > 1.0f;};
		_achievements.Add(a);
	}
	
	// Update is called once per frame
	void Update () {
		if(!_isShowing){

			foreach(Achievement a in _achievements){
				if(!a.achieved && a.check()){
					a.achieved = true;
					ShowAchievement(a.title, a.subTitle);
				}
			}

		}
		else{
			//Debug.Log(Time.time + " : until : " + _wait);
			if(Time.time >= _wait){
				NGUITools.SetActive(achievementPanel, false);
				_isShowing = false;
			}
		}
	}//EAT YOUR VEGETABLES

	void ShowAchievement(string title, string subTitle){
		NGUITools.SetActive(achievementPanel, true);
		labelAchievement.text = title;
		labelSubTitle.text = subTitle;
		_wait = Time.time + _timeDelay;
		_isShowing = true;
	}
}
