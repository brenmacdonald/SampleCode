using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;

public class labViewComm : MonoBehaviour {
	
	int iLevelNumber=0;
	private bool portOpened=true;
	
	private int portID = 4000;
	private string ipAdress = "127.0.0.1";
	
	private UdpClient _udpClient = new UdpClient();
	

	void OnDisable(){
		if(portOpened){
			//_labViewPort.Close();
			
		}
		_udpClient.Close();
		//_labViewPort.Dispose();
		//_udpClient.Dispose(true);
		//_udpClient.Finalize();
	}
	
	public void LabViewSync(int dataOut){
		//data[0] = (byte)dataOut;
		//String converter = "";
		//converter += dataOut;
		byte[] converter = Encoding.ASCII.GetBytes(dataOut.ToString());
		_udpClient.Send(converter,converter.Length);
		Debug.Log("To port: " + dataOut);
		if(portOpened){
			//_labViewPort.Write(data,0,1);	
			//_labViewPort.Write(converter);
			
		}
	}
	
	int HexLevel(int iLevelNumber){
		switch(iLevelNumber){
		case 0:
			return 0x30;//title screen
		case 1:
			return 0x31;//during directions
		case 2:
			return 0x32;//during test
		case 3:
			return 0x33;//end of test screen
		default:
			return 0;
		}
	}
	
	public void SetUDP(String sIpAddress, int nPortID){
		ipAdress = sIpAddress;
		portID = nPortID;
		_udpClient.Connect(ipAdress,portID);
	}

}
