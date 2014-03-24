Welcome to the Plato Evolved AdMob Android Unity Plugin v1.01

Setup

1 Download the asset from the Asset store (or import the package)
2 Amend the AndroidManifest.xml file with your AdMob Publisher ID
3 Deploy your game to an android device to check that you see the ad
4 Done!

In Detail

The AndroidManifest.xml file is in the Plugins/Android folder

The line you need to amend in the AndroidManifest.xml file is:

    <meta-data android:name="admob_pub_id" android:value="Enter AdMob Publisher ID here"/>


Change it to look something like (i.e. put in your Publisher ID that you get from signing up with AdMob):

    <meta-data android:name="admob_pub_id" android:value="a14xxxxxxxxxxx"/>



Scripting

Without doing any scripting, the default position of the ad banner can be set in the AndroidManifest.xml file:

    <meta-data android:name="adposition_x" android:value="middle"/>
    <meta-data android:name="adposition_y" android:value="bottom"/>

The possible values of adposition_x are: left,middle or right	
The possible values of adposition_y are: top or bottom	

If you want to change the position (or hide/show) the ad banner during your game check out the included DemoScene but here is a quick example:

AdMobUnityPlugin.ShowAds(); //Shows Ads
AdMobUnityPlugin.HideAds(); //Hides Ads
AdMobUnityPlugin.SetPosition("middle","top");
	
Common Errors

You MUST deploy to an android device to see ads (test or real)
You must be properly signed up with some AdMob and added a live AdMob Publisher ID 
How To Find Your AdMob Publisher ID: (http://www.admob.com/my_sites)
1) Click "Sites & Apps" Tab 2) on your app click "Manage settings" 3) Below your app's app's name, and URL, you'll see "Publisher ID"

New in v1.01

New meta-data tag, "start_hidden", values can be "true" or "false",if "true", ad banner is initially hidden (defaults to "false"):

<meta-data android:name="start_hidden" android:value="true"/>

New Method, SetBanner(String bannertype), bannertypes are: BANNER,IAB_MRECT,IAB_BANNER,IAB_LEADERBOARD,SMART_BANNER. (defaults to "BANNER")

Is now compatible with other plugins that need to override the main activity, ie "com.platoevolved.admobunity.AdMobUnityActivity" doesn't need to be specified in the AndroidManifest any more. However at least one of your installed plugins need to do override the main activity for touches to work.

Version 6.1.0 of the Google Admob SDK now used