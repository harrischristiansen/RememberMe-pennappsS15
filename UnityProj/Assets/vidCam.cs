using UnityEngine;
using System.Collections;

public class vidCam : MonoBehaviour {
	
	public MeshRenderer[] UseWebcamTexture;
	private WebCamTexture webcamTexture;

	int takeLeftIndent=5, takeTopIndent, takeWidth, takeHeight=45;
	
	void Start() {
		webcamTexture = new WebCamTexture();
		foreach(MeshRenderer r in UseWebcamTexture) {
			r.material.mainTexture = webcamTexture;
		}
		renderer.material.mainTexture = webcamTexture;
		webcamTexture.Play();

		takeTopIndent = Screen.height - takeHeight - 5;
		takeWidth = Screen.width - 10;
	}
	
	void OnGUI() {
		if (webcamTexture.isPlaying) {
			if (GUI.Button(new Rect(takeLeftIndent, takeTopIndent, takeWidth, takeHeight),"Take")) {
				takePicture ();
				webcamTexture.Pause();
			}
		}
		/*
		else {
			if (GUI.Button(new Rect(disconnectLeftIndent, disconnectTopIndent, disconnectWidth, disconnectHeight),"Play")) {
				webcamTexture.Play();
			}
		}
		*/
	}

	void takePicture() {
		// Create Texture2D
		Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height);
		snap.SetPixels (webcamTexture.GetPixels());
		snap.Apply ();

		// Upload File
		StartCoroutine(uploadPicture("testPic.png", snap));

		// Save File
		//System.IO.File.WriteAllBytes("testasdf1234.png", snap.EncodeToPNG());
	}
	IEnumerator uploadPicture(string fileName, Texture2D picture) {
		
		WWWForm postForm = new WWWForm();
		postForm.AddBinaryData("theFile",picture.EncodeToPNG(),fileName,"text/plain");
		
		WWW upload = new WWW("http://www.purduecs.com/PennApps/upload.php",postForm);
		yield return upload;

		if (upload.error == null) {
			Debug.Log("Upload Success:" + upload.text);
		} else {
			Debug.Log("Upload ERROR: " + upload.error);
		}
	}
}
