using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class vidCam : MonoBehaviour {
	
	private Texture savedTexture;
	private WebCamTexture webcamTexture;

	int takeLeftIndent=5, takeTopIndent, takeWidth, takeHeight=70;
	
	// Entry Screen
	public GameObject entryScreen;
	private Text statusTxt;

	// Picture Info
	string picFileName;
	
	void Start() {
		// Find Back Cam
		WebCamDevice[] devices = WebCamTexture.devices;
		string backCamName="";
		for( int i = 0 ; i < devices.Length ; i++ ) {
			if (!devices[i].isFrontFacing) {
				backCamName = devices[i].name;
			}
		}
		
		
		webcamTexture = new WebCamTexture(backCamName,720,1280,30);
		savedTexture = renderer.material.mainTexture;
		renderer.material.mainTexture = webcamTexture;
		webcamTexture.requestedHeight = 1280; // 960
		webcamTexture.requestedWidth = 720; // 640
		webcamTexture.Play();
		if(webcamTexture.videoRotationAngle != 0) {
			GameObject.Find ("PreviewImg").GetComponent<RectTransform>().Rotate(Vector3.forward, -90);
			GameObject.Find ("PreviewImg").GetComponent<RectTransform>().localScale = new Vector3(1,0.63f,1);
			transform.Rotate(Vector3.up, webcamTexture.videoRotationAngle);
			transform.localScale = new Vector3(1,1,0.63f);
		}

		// Locate Take Picture Button
		takeTopIndent = Screen.height - takeHeight - 5;
		takeWidth = Screen.width - 10;

		// Setup UI
		GameObject.Find ("PreviewImg").GetComponent<RawImage>().texture = webcamTexture;
		statusTxt = GameObject.Find ("statusTxt").GetComponent<Text>();
		entryScreen.SetActive(false); // Hide Entry Canvas
	}
	
	void OnGUI() {
		if (webcamTexture.isPlaying) {
			if (GUI.Button(new Rect(takeLeftIndent, takeTopIndent, takeWidth, takeHeight),"Take")) {
				takePicture ();
			}
		}
	}

	void takePicture() {
		// Pause Camera
		webcamTexture.Pause();

		// Create Texture2D
		Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height);
		snap.SetPixels (webcamTexture.GetPixels());
		snap.Apply ();

		// Upload File
		StartCoroutine(uploadPicture(snap));
		// Show Entry Canvas
		StartCoroutine(showEntryField());

		// Save File
		//System.IO.File.WriteAllBytes("testasdf1234.png", snap.EncodeToPNG());
	}
	IEnumerator uploadPicture(Texture2D picture) {
		statusTxt.text = "Uploading Image";
		
		picFileName = System.DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		
		// Create Form
		WWWForm postForm = new WWWForm();
		postForm.AddBinaryData("theFile",picture.EncodeToPNG(),picFileName,"text/plain");
		
		// Submit Form
		WWW upload = new WWW("http://www.purduecs.com/PennApps/upload.php",postForm);
		yield return upload;
		
		// Show Result Message
		if (upload.error == null) {
			Debug.Log("Upload Success: " + upload.text);
			statusTxt.text = "Upload Success";
			if(!upload.text.Equals("Uploaded")) {
				setFormFields(upload.text,"","");
			}
		} else {
			Debug.Log("Upload ERROR: " + upload.error);
			statusTxt.text = "Upload ERROR: " + upload.error;
		}
	}
	IEnumerator showEntryField() {
		yield return new WaitForSeconds(0.1f);
		renderer.material.mainTexture = savedTexture; // Set BG To White
		entryScreen.SetActive(true); // Show Entry Canvas
		setFormFields("","",""); // Clear Form
	}
	
	void setFormFields(string personName, string personProject, string personNotes) {
		GameObject.Find("NameInput").GetComponent<InputField>().value = personName;
		GameObject.Find("ProjectInput").GetComponent<InputField>().value = personProject;
		GameObject.Find("NotesInput").GetComponent<InputField>().value = personNotes;
	}

	public void submitFormFunc() {
		StartCoroutine(submitForm());
	}

	IEnumerator submitForm() {
		// Set Status To Saving
		statusTxt.text = "Saving Form";

		// Get Fields
		string personName = GameObject.Find("NameInput").GetComponent<InputField>().value; // Save Person Name
		string personProject = GameObject.Find("ProjectInput").GetComponent<InputField>().value; // Save Person Project
		string personNotes = GameObject.Find("NotesInput").GetComponent<InputField>().value; // Save Person Notes

		// Create Form
		WWWForm postForm = new WWWForm();
		postForm.AddField ("personName", personName);
		postForm.AddField ("imgName", picFileName);
		postForm.AddField ("personProject", personProject);
		postForm.AddField ("personNotes", personNotes);

		// Submit Form
		WWW upload = new WWW("http://www.purduecs.com/PennApps/savePerson.php",postForm);
		yield return upload;

		// Show Result Message
		if (upload.error == null) {
			Debug.Log("Save Success: " + upload.text);
			statusTxt.text = "Save Success: " + upload.text;
		} else {
			Debug.Log("Save ERROR: " + upload.error);
			statusTxt.text = "Save ERROR: " + upload.error;
		}

		yield return new WaitForSeconds(1);
		
		returnToCamera();
	}

	public void returnToCamera() {
		webcamTexture.Play();
		entryScreen.SetActive(false); // Hide Entry Canvas
		renderer.material.mainTexture = webcamTexture;
	}
}
