using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiToolbox;

/// <summary>
/// This class sends a prompt to a Foundation Model and receives a response.
/// </summary>
public class PromptTextFoundationModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
	//ChatGpt.Request();

	// This request provides only `completeCallback` and `failureCallback`
	// parameters. Since the `updateCallback` is not provided, the request
	// will be completed in one step, and the `completeCallback` will be
	// called only once, with the full text of the answer.
	ChatGpt.Request(prompt, parameters,
					        response => {
                                Debug.Log("Full response: " + response);
                            }, (errorCode, errorMessage) => {
	        var errorType = (ChatGptErrorCodes)errorCode;
	        Debug.LogError("Error: " + errorType + " - " +
					        errorMessage);
    });
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
