using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class OpenAIAPIManager : MonoBehaviour
{
    public string model = "gpt-3.5-turbo";
    public string API_KEY = "";

    [System.Serializable]
    public class ChatCompletionResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    
    private const string API_URL = "https://api.openai.com/v1/chat/completions";

    public void GenerateCompletion(string prompt, Action<string> callback)
    {
        StartCoroutine(SendRequest(prompt, callback));
    }

private IEnumerator SendRequest(string prompt, Action<string> callback)
{
    // Update jsonBody to use the model variable
    string jsonBody = "{\"model\": \"" + model + "\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}]}";

    UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError("Error: " + request.error);
        callback(null);
    }
    else
    {
        string response = request.downloadHandler.text;
        string completion = ParseResponse(response);
        callback(completion);
    }
}


    private string ParseResponse(string response)
    {
        ChatCompletionResponse chatCompletionResponse = JsonUtility.FromJson<ChatCompletionResponse>(response);
        if (chatCompletionResponse.choices.Length > 0)
        {
            return chatCompletionResponse.choices[0].message.content;
        }
        return "";
    }
}
