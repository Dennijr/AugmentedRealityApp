using UnityEngine;
using System.Collections;
using System.IO;

using System;
using System.Net;
using AssemblyCSharpfirstpass;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;

public class PushService: MonoBehaviour, App42NativePushListener
{
	private string message="App42 Push Message";
	public string app42Response="";
	public const string ApiKey ="43eec6b7f0e821f7fc622589461deaffd65bd20d68462953d74a7b013a26bbfd";
	public const string SecretKey="88fa44334532667534310afbf8cf0972040b39860fa48814641aa6ba78f44d82";
	public const string GoogleProjectNo="635062038612";
	public const string UserId="rthirucs"; 
   

	public void Start (){
		DontDestroyOnLoad (transform.gameObject);
		App42API.Initialize(ApiKey,SecretKey);
		App42API.SetLoggedInUser(UserId);
		//Put Your Game Object Here
		App42Push.setApp42PushListener (this);
		#if UNITY_ANDROID
		App42Push.registerForPush (GoogleProjectNo);
		message=App42Push.getLastPushMessage();
		#endif 
	}

	public void onDeviceToken(String deviceToken){
		message="Device token from native: "+deviceToken;
		String deviceType = App42Push.getDeviceType ();
		if(deviceType!=null&&deviceToken!=null&& deviceToken.Length!=0)
			App42API.BuildPushNotificationService().StoreDeviceToken(App42API.GetLoggedInUser(),deviceToken,
                                                    deviceType,new Callback());
   }
	public void onMessage(String msg){
		message="Message From native: "+msg;
		
	}
		public void onError(String error){
		message="Error From native: "+error;
		
	}
	public void sendPushToUser(string userName,string msg){
		
		App42API.BuildPushNotificationService().SendPushMessageToUser(userName,msg,new Callback());
		
	}
	public void sendPushToAll(string msg){
		
		App42API.BuildPushNotificationService().SendPushMessageToAll(msg,new Callback());
		
	}
}


