using AiToolbox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

namespace AiToolboxRuntimeSample
{
	public class PromptTextFoundationModel : MonoBehaviour
	{
		public Parameters parameters;

		[SerializeField]
		MapCreator mapCreator;

		public TMP_InputField inputField;
		public TMP_Text chatDisplay;

		bool isNewChat = true;

		//string basePrompt = @"
		//	1.) Do not include any explanations, only provide a  RFC8259 compliant JSON response.

		//	2.) The JSON response will be a list of the coordinates of a 3D grid of tiles

		//	3.) The JSON attributes are 
		//		'x': the x coordinate of the tile
		//		'y': the height of the tile. This must be at least 1.
		//		'z': the z coordinate of the tile.

		//	4.) The tiles you create should look like:
  //      ";

		string basePrompt = @"
			1.) Do not include any explanations, only provide a  RFC8259 compliant JSON response.

			2.) The JSON response will be a list of the coordinates of a 3D grid of tiles

			3.) The JSON attributes are 
				'x': the x coordinate of the tile
				'y': the height of the tile. This must be at least 1.
				'z': the z coordinate of the tile.

			4.) Each tile must have a unique x and z tuple.

			5.) Be creative.

			6.) The tiles you create should look like:
		";
		void Start()
		{
			//// Check if the API Key is set in the Inspector, just in case.
			//if (parameters == null || string.IsNullOrEmpty(parameters.apiKey))
			//{
			//	const string errorMessage = "Please set the <b>API Key</b> in the <b>ChatGPT Dialogue</b> Game Object.";
			//	Debug.LogError(errorMessage);
			//	return;
			//}

			//// This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
			//// is not provided, the request will be completed in one step, and the `completeCallback` will be called only
			//// once, with the full text of the answer.
			//ChatGpt.Request(prompt, parameters, response => { Debug.Log("Full response: " + response); },
			//				(errorCode, errorMessage) => {
			//					var errorType = (ChatGptErrorCodes)errorCode;
			//					Debug.LogError("Error: " + errorType + " - " + errorMessage);
			//				});

			//// This request provides all three callbacks: `completeCallback`, `updateCallback`, and `failureCallback`.
			//// Since the `updateCallback` is provided, the request will be completed in multiple steps, and the
			//// `completeCallback` will be called only once, with the full text of the answer.
			//ChatGpt.Request(prompt, parameters, response => { Debug.Log("Full response: " + response); },
			//				(errorCode, errorMessage) => {
			//					var errorType = (ChatGptErrorCodes)errorCode;
			//					Debug.LogError("Error: " + errorType + " - " + errorMessage);
			//				}, chunk => { Debug.Log("Next part of response: " + chunk); });

			isNewChat = true;

			
		}

		void OnDestroy()
		{
			ChatGpt.CancelAllRequests();
		}

		// convert string in JSON format to SerializableVector3 list
		public List<SerializableVector3> ConvertJsonToSv3List(string jsonString)
		{
			List<SerializableVector3> vectors = JsonConvert.DeserializeObject<List<SerializableVector3>>(jsonString);


			return vectors;
		}

		void SendReponseToMapCreator(List<SerializableVector3> sv3List)
		{
			mapCreator.SetLevelFromSerializableVector3List(sv3List);
		}

		public void SubmitText()
		{
			string submitPrompt;

			if( isNewChat)
				submitPrompt = basePrompt + inputField.text;
			else
				submitPrompt = inputField.text;

			Debug.Log("submitPrompt: " + submitPrompt);
			// This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
			// is not provided, the request will be completed in one step, and the `completeCallback` will be called only
			// once, with the full text of the answer.
			ChatGpt.Request(submitPrompt, parameters, response =>
			{
				Debug.Log("Full response: " + response);

				var sv3List = ConvertJsonToSv3List(response);
				SendReponseToMapCreator(sv3List);

			},
				(errorCode, errorMessage) =>
				{
					var errorType = (ChatGptErrorCodes)errorCode;
					Debug.LogError("Error: " + errorType + " - " + errorMessage);
				});

			//Debug.Log("testing JSON parser");

			//// dummy example works
			////string jsonString = @"[{""x"":0,""y"":1,""z"":0},{""x"":1,""y"":1,""z"":0},{""x"":0,""y"":1,""z"":1},{""x"":1,""y"":1,""z"":1}]";
			//string jsonString = @"
				
			//";
			//Debug.Log("jsonString: " + jsonString);
			//var sv3List = ConvertJsonToSv3List(jsonString);
			//Debug.Log("sv3List: " + sv3List);
			//foreach (SerializableVector3 vec in sv3List)
			//{
			//	Debug.Log(vec.ToString());
			//}

			//SendReponseToMapCreator(sv3List);

			isNewChat = false;
		}

		public void StartNewConversation()
		{
			isNewChat = true;
		}

		public void ClearText()
		{
			inputField.text = "";
		}
	}
}
