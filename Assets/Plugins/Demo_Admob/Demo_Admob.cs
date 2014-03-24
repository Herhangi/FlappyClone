using UnityEngine;
using System.Collections;

public class Demo_Admob : MonoBehaviour {
	
    public GUISkin skin;
	private int selGridInty = 0;
	private int selGridIntx = 0;
	private int selGridIntvis = 0;
	private int selGridIntbannertype = 0;
	
	private string[] selStringsy = new string[] {"top", "bottom"};
	private string[] selStringsx = new string[] {"left", "middle","right"};
	private string[] selStringsvis = new string[] {"visible", "hidden"};
	private string[] selStringsbannertype = new string[] {"BANNER", "IAB_MRECT", "IAB_BANNER", "IAB_LEADERBOARD", "SMART_BANNER"};
	
	private string currenty = "";
	private string currentx = "";
	private string currentvis = "";
	private string currentbannertype = "";
	
	
	void OnGUI() {
        if (skin != null)
        {
            GUI.skin = skin;
        }

        BeginPage(Screen.width/1.1f, Screen.height / 1.1f);
		
		selGridInty = GUILayout.SelectionGrid(selGridInty, selStringsy, 2);
		
		if(!selStringsy[selGridInty].Equals(currenty))
		{
			currenty = selStringsy[selGridInty];
			AdMobUnityPlugin.SetPosition(currentx,currenty);
		}
		
		selGridIntx = GUILayout.SelectionGrid(selGridIntx, selStringsx, 3);

		if(!selStringsx[selGridIntx].Equals(currentx))
		{
			currentx = selStringsx[selGridIntx];
			AdMobUnityPlugin.SetPosition(currentx,currenty);
		}
		

		selGridIntvis = GUILayout.SelectionGrid(selGridIntvis, selStringsvis, 2);

		if(!selStringsvis[selGridIntvis].Equals(currentvis))
		{
			currentvis = selStringsvis[selGridIntvis];
			if(currentvis.Equals("visible"))
				AdMobUnityPlugin.ShowAds();
			else
				AdMobUnityPlugin.HideAds();
			}
		
		selGridIntbannertype = GUILayout.SelectionGrid(selGridIntbannertype, selStringsbannertype, 5);

		if(!selStringsbannertype[selGridIntbannertype].Equals(currentbannertype))
		{
			Debug.Log(currentbannertype);
			currentbannertype = selStringsbannertype[selGridIntbannertype];
			AdMobUnityPlugin.SetBanner(currentbannertype);
		}
		
		EndPage();
    }	
	
    void BeginPage(float width, float height)
    {
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
    }

    void EndPage()
    {
        GUILayout.EndArea();
    }
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		    Application.Quit();		
	}
	
}
