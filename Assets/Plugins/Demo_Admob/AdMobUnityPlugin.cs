using UnityEngine;

public class AdMobUnityPlugin {

	private static string classname = "com.platoevolved.admobunity.UnityAndroidInterface";

	public static void StartAds(){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("StartAds");
	}
	public static void StopAds(){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("StopAds");		
	}
	public static void SetPosition(string xpos,string ypos){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("SetPosition",new string[] {xpos, ypos});			
	}
	public static void ShowAds(){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("ShowAds");			
	}
	public static void HideAds(){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("HideAds");			
	}
	public static void RefreshAd(){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("RefreshAd");			
	}
	public static void SetBanner(string bannertype){
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("SetBanner",bannertype);			
	}
	
}
