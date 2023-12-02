using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiToolbox;
using System.Data;
using System.Security.Cryptography;
using UnityEditor.PackageManager;

namespace AiToolboxRuntimeSample
{

	/// <summary>
	/// This class sends a prompt to a Foundation Model and receives a response.
	/// </summary>
	public class PromptTextFoundationModel : MonoBehaviour
    {
		public Parameters parameters;

		[SerializeField]
	    MapCreator mapCreator;

        string basePrompt = @"
            1.) Create a Dictionary of format Dictionary<Point, Tile> tilesDict
		    2.) The Point class is defined as:

	        3.) The Tile class is defined as:

		    4.) Focus on the tile.pos and tile.height properties. The rest of the properties will be defaults.
        ";


	    // Start is called before the first frame update
	    void Start()
        {
			// Check if the API Key is set in the Inspector, just in case.
			if (parameters == null || string.IsNullOrEmpty(parameters.apiKey))
			{
				const string errorMessage = "Please set the <b>API Key</b> in the <b>ChatGPT Dialogue</b> Game Object.";
				Debug.Log(errorMessage);
				return;
			}
		}

		// This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
		// is not provided, the request will be completed in one step, and the `completeCallback` will be called only
		// once, with the full text of the answer.
	//	ChatGpt.Request(prompt, parameters, completeCallback: text => {
 //           // We've received the full text of the answer, so we can display it in the "You're chatting with" text.
 //           characterDescription.text = text;

 //           // Create a new Parameters object with the `role` parameter set to the character description we've received.
 //           // Now, all the requests made with this Parameters object will be made in the context of this character.
 //           _parametersWithCharacterRole = new Parameters(parameters) { role = text };

	//	// Ask AI to introduce itself. Note that the message does not contain the character description, because
	//	// the `role` parameter is already set in the `_parametersWithCharacterRole` object.
	//	_conversationSoFar.Add(new Message("Introduce yourself as the character.", Role.User));
 //           AddNpcAnswer();
	//}, failureCallback: (errorCode, errorMessage) => {
 //           // If the request fails, display the error message in the "You're chatting with" text.
 //           var errorType = (ChatGptErrorCodes)errorCode;
	//characterDescription.text = $"Error {errorCode}: {errorType} - {errorMessage}";
 //           characterDescription.color = Color.red;
 //       });

	    //ChatGpt.Request();

	    // This request provides only `completeCallback` and `failureCallback`
	    // parameters. Since the `updateCallback` is not provided, the request
	    // will be completed in one step, and the `completeCallback` will be
	    // called only once, with the full text of the answer.
	    //ChatGpt.Request(prompt, parameters,
					//            response => {
     //                               Debug.Log("Full response: " + response);
     //                           }, (errorCode, errorMessage) => {
	    //        var errorType = (ChatGptErrorCodes)errorCode;
	    //        Debug.LogError("Error: " + errorType + " - " +
					//            errorMessage);
     //   });
        /*

        // Update is called once per frame
        void Update()
        {
        
        }

        //connect to the foundation model
        void ConnectToFoundationModel()
        {
		    //connect to ChatGPT AI
            //connect to GPT-3
        
	    }

        //send a prompt
        void SendPrompt()
        { }


        //receive a response
        void ReceiveResponse()
        { }

        //display the response
        void DisplayResponse()
        { }
        */
    }
}
