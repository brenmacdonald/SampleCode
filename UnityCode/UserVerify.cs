using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class UserVerify : MonoBehaviour {

	private static string _encryptedPass;
	// Use this for initialization
	void Start(){
		_encryptedPass = PlayerPrefs.GetString("password");
	}

	public static string EncryptedPassword{
		get{return _encryptedPass;}
	}

	public static bool Verify(string password, string userName){
		bool bVerify = false;
		string sPassToCheck = HashPass(password, userName);
		return string.Equals(sPassToCheck, _encryptedPass);
	}
	//One way hash, compare with saved hash in PlayerPrefs via Verify
	//Player's user name is the salt
	public static string HashPass(string passInput, string userName){
		SHA256 sha256 = new SHA256CryptoServiceProvider();
		sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(userName + passInput));
		
		byte[] result = sha256.Hash;		
		string sReturnVal = Convert.ToBase64String(result);
		
		return sReturnVal;
	}
}
