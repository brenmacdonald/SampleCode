using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Threading;

public class serialComm : MonoBehaviour {
	
	//private int offset=0;
	private String comPort = "COM1";
	private int baudRate = 57600;
	private System.IO.Ports.Parity parity = System.IO.Ports.Parity.None;
	private int dataBits = 8;
	private System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One;
	private int count;
	private int PACKET_SIZE = 4;
	private int EPRIME_SERIAL_PACKET = 11;
	private int nCurrentTime;
	private SerialPort _serialPort;
	//private array<Byte>^ buffer = gcnew array<Byte>(100);
	private int[] arrData = new int[100];
	private byte[] data = new byte[100];
	private int iChecksum = 0;
	
	// Use this for initialization
	void Start () {
		//move serialport initialization to a different function like OpenSerialPort(), this way I can call it externally and don't have to worry about it starting up before I can set the variables for it
		//_serialPort = new SerialPort("COM1", 57600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
		
		arrData[0] = 0x56;
		arrData[1] = 0x5A;
		arrData[2] = 0;
		arrData[3] = PACKET_SIZE;
		arrData[4] = EPRIME_SERIAL_PACKET;	
		
		count = 4 + PACKET_SIZE + 2;
		
		//_serialPort.Open();
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetKeyUp(KeyCode.Space)){
		//	Debug.Log("Spacebar pressed.");			
		//	Debug.Log(DateTime.Now.Hour*10000 + DateTime.Now.Minute*100 + DateTime.Now.Second);			
			
		//	SerialSync();
		//}		
	}
	
	//Cleanup
	void OnDisable(){
		_serialPort.Close();
		_serialPort.Dispose();
	}
	
	//Logic to actually send required data to the serial port
	public void SerialSync(){		
		nCurrentTime = DateTime.Now.Hour*10000 + DateTime.Now.Minute*100 + DateTime.Now.Second;		
		/*
		for(int i=PACKET_SIZE;i>0;i--){
			arrData[4+i] = nCurrentTime % 256;
			nCurrentTime = nCurrentTime / 256;
		}
		*/
		byte[] byteCurrentTime = new byte[4];
		byteCurrentTime = IntToBE(nCurrentTime);
		for(int i = 0;i<PACKET_SIZE;i++){
			arrData[5+i]= byteCurrentTime[i];
			//Debug.Log(arrData[5+i]);				
		}
		
		
		for(int i=2;i<=4+PACKET_SIZE;i++){
			iChecksum += arrData[i];	
		}
		iChecksum = 255 - (iChecksum % 256);
		arrData[4+PACKET_SIZE+1] = iChecksum;
		
		for(int i=0; i<100;i++){
			data[i]=(byte)arrData[i];
		}
		
		_serialPort.Write(data,0,count);	

	}
	//Convert my integer value timestamp to a bigendian 4-byte array
	byte[] IntToBE(int data)
  	{
     	byte[] b = new byte[4];
     	b[3] = (byte)data;
    	b[2] = (byte)(((uint)data >> 8) & 0xFF);
     	b[1] = (byte)(((uint)data >> 16) & 0xFF);
     	b[0] = (byte)(((uint)data >> 24) & 0xFF);
     	return b;
  	}
	
	public void SetSerial(String com, int baud, String par, int data, String stop){
		comPort = com;
		baudRate = baud;
		switch(par){
			case ("None"):
				parity = System.IO.Ports.Parity.None;
				break;
			case ("Odd"):
				parity = System.IO.Ports.Parity.Odd;
				break;
			case ("Even"):
				parity = System.IO.Ports.Parity.Even;
				break;
			case ("Mark"):
				parity = System.IO.Ports.Parity.Mark;
				break;
			case ("Space"):
				parity = System.IO.Ports.Parity.Space;
				break;
			default:
				parity = System.IO.Ports.Parity.None;
				break;
		}
		dataBits = data;
		switch(stop){
		case ("One"):
			stopBits = System.IO.Ports.StopBits.One;
			break;
		case ("Two"):
			stopBits = System.IO.Ports.StopBits.Two;
			break;
		case ("OnePointFive"):
			stopBits = System.IO.Ports.StopBits.OnePointFive;
			break;
		default:
			stopBits = System.IO.Ports.StopBits.One;
			break;
		}
		
		_serialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
		_serialPort.Open();
	}

	
}
