using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HSController : MonoBehaviour
{




	private string secretKey = "756757527"; // Edit this value and make sure it's the same as the one stored on the server
	string addScoreURL = "test.bplaced.net/addscore.php?"; //be sure to add a ? to your url
	string highscoreURL = "test.bplaced.net/display.php";

	//for testing
	 string uniqueID;
	 string enterName;
	System.Int64 score;

	public InputField userName, scoreValue;

	public Text Status;

	public Text[] userNames,scoreValues;

	void Start(){

		LoadScores ();
	}

	public void LoadScores()
	{
		StartCoroutine(GetScores());
	}

	public void SaveScores()
	{	
		// uniqueID,enterName and score will get the actual value before posting score
		uniqueID = "A"+Random.Range(1,30000000).ToString(); 
		if (enterName.Contains (":") || enterName.Contains (";") || enterName.Contains (",") || enterName.Contains ("'") || enterName.Contains ("?"))
			enterName = "Unknown";
		else
			enterName = userName.text;

		score = System.Int64.Parse( scoreValue.text);

		StartCoroutine(SendScores());
	}

	public  string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
	
	// remember to use StartCoroutine when calling this function!
	IEnumerator SendScores()
	{
		
		Status.text = "Saving...";

		string hash = Md5Sum(enterName + score + secretKey);

		string post_url = addScoreURL + "uniqueID=" + uniqueID+ "&name=" + WWW.EscapeURL (enterName) + "&score=" + score+ "&hash=" + hash;

		WWW hs_post = new WWW("http://"+post_url);

		yield return hs_post;
		
		if (hs_post.error != null)
		{
			Status.text = "Error : "+ hs_post.error.ToString();
		}
		else
			Status.text = "Saved Successful";
	}

	 string[] onlineHighscore;

	IEnumerator GetScores()
	{

		Status.text = "Loading ...";


		WWW hs_get = new WWW("http://"+highscoreURL);

		yield return hs_get;
		
		if (hs_get.error != null)
		{
			Status.text = "Error : "+ hs_get.error.ToString();

		}
		else
		{

			//Change .text into string to use Substring and Split
			string help = hs_get.text;

			//help= help.Substring(5, hs_get.text.Length-5);
			//200 is maximum length of highscore - 100 Positions (name+score)

			onlineHighscore  = help.Split(";"[0]);
			Debug.Log (hs_get.text);


			List<string> listName = new List<string>();
			List<string> listScore = new List<string>();

			for (int a = 0; a < onlineHighscore.Length; a+=2) {

				listName.Add(onlineHighscore [a]);

			}
			for (int a = 1; a < onlineHighscore.Length; a += 2) {

				listScore.Add(onlineHighscore [a]);

			}

			for (int i = 0; i < listName.Count; i++)
			{
				if(i<=userNames.Length)
					userNames [i].text = listName[i];
			}
			for (int i = 0; i < listScore.Count; i++)
			{
				if(i<=scoreValues.Length)
					scoreValues [i].text = listScore[i];
			}
			Status.text = "Loaded Successful";
		}

	}
	
}
