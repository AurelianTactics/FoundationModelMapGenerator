using AiToolbox;
using UnityEngine;

namespace AiToolboxRuntimeSample
{
	public class PromptTextFoundationModel : MonoBehaviour
	{
		public Parameters parameters;
		public string prompt = "Generate a character description";

		[SerializeField]
		MapCreator mapCreator;

		string basePrompt = @"
            1.) Create a Dictionary of format Dictionary<Point, Tile> tilesDict
		    2.) The Point class is defined as:

	        3.) The Tile class is defined as:

		    4.) Focus on the tile.pos and tile.height properties. The rest of the properties will be defaults.
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
		}

		void OnDestroy()
		{
			ChatGpt.CancelAllRequests();
		}
	}
}
