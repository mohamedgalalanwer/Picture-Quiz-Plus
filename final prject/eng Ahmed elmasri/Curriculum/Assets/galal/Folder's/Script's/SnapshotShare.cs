using UnityEngine;
using System.Collections;
using System.IO;

public class SnapshotShare : MonoBehaviour
{
	private AndroidUltimatePluginController androidUltimatePluginController;
	Camera mainCamera;
	RenderTexture renderTex;
	Texture2D screenshot;
	Texture2D LoadScreenshot;
	int width = Screen.width;   // for Taking Picture
	int height = Screen.height; // for Taking Picture
	string fileName;
	string screenShotName = "PictureShare.png";

	void Start ()
	{
		androidUltimatePluginController = AndroidUltimatePluginController.GetInstance ();
	}

	public void Snapshot ()
	{
		StartCoroutine (CaptureScreen ());
	}
	
	public IEnumerator CaptureScreen ()
	{
		yield return null; // Wait till the last possible moment before screen rendering to hide the UI

		GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;
		yield return new WaitForEndOfFrame (); // Wait for screen rendering to complete
		if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			mainCamera = Camera.main.GetComponent<Camera> (); // for Taking Picture
			renderTex = new RenderTexture (width, height, 24);
			mainCamera.targetTexture = renderTex;
			RenderTexture.active = renderTex;
			mainCamera.Render ();
			screenshot = new Texture2D (width, height, TextureFormat.RGB24, false);
			screenshot.ReadPixels (new Rect (0, 0, width, height), 0, 0);
			screenshot.Apply (); //false
			RenderTexture.active = null;
			mainCamera.targetTexture = null;
		}
		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
			mainCamera = Camera.main.GetComponent<Camera> (); // for Taking Picture
			renderTex = new RenderTexture (height, width, 24);
			mainCamera.targetTexture = renderTex;
			RenderTexture.active = renderTex;
			mainCamera.Render ();
			screenshot = new Texture2D (height, width, TextureFormat.RGB24, false);
			screenshot.ReadPixels (new Rect (0, 0, height, width), 0, 0);
			screenshot.Apply (); //false
			RenderTexture.active = null;
			mainCamera.targetTexture = null;
		}
		// on Win7 - C:/Users/Username/AppData/LocalLow/CompanyName/GameName
		// on Android - /Data/Data/com.companyname.gamename/Files
		File.WriteAllBytes (Application.persistentDataPath + "/" + screenShotName, screenshot.EncodeToPNG ());  

		// on Win7 - it's in project files (Asset folder)
		//File.WriteAllBytes (Application.dataPath + "/" + screenShotName, screenshot.EncodeToPNG ());  
		//File.WriteAllBytes ("picture1.png", screenshot.EncodeToPNG ());
		//File.WriteAllBytes (Application.dataPath + "/../../picture3.png", screenshot.EncodeToPNG ());
		//Application.CaptureScreenshot ("picture2.png");
		GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true; // Show UI after we're done
	}

	public void ShareImage ()
	{
		string path = Application.persistentDataPath + "/" + screenShotName;
		androidUltimatePluginController.ShareImage ("subject", "subjectContent", path);
	}

	/*public void LoadImage ()
	{
		string path = Application.persistentDataPath + "/" + screenShotName;
		byte[] bytes;
		bytes = System.IO.File.ReadAllBytes(path);
		LoadScreenshot = new Texture2D(1,1);
		LoadScreenshot.LoadImage(bytes);
		GameObject.FindGameObjectWithTag ("Picture").GetComponent<Renderer> ().material.mainTexture = screenshot;
	}
*/
	public void close ()
	{
		Application.Quit ();
	}
}
