using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// CHAT OBJECTS =======================================================================
#region Chat Objects
[Serializable]
class ChatCompletionRequest
{
    public string model;
    public ChatMessage[] messages;

    public ChatCompletionRequest(string model, ChatMessage[] messages)
    {
        this.model = model;
        this.messages = messages;
    }
}

[Serializable]
public class ChatMessage
{
    public string role;
    public string content;

    public ChatMessage(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}

[Serializable]
class ChatCompletionResponse
{
    public ChatChoice[] choices;
}

[Serializable]
class ChatChoice
{
    public ChatMessage message;
}
#endregion

public class AIManager : MonoBehaviour
{
    // VARIABLES ==========================================================
    #region Variables and Instance
    private static AIManager instance;
    //Finds the instance in the scene, or creates one if none exists, so AI
    //calls work even when no AIManager object has been placed in the scene.
    public static AIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AIManager>();
                if (instance == null)
                {
                    instance = new GameObject("AIManager").AddComponent<AIManager>();
                }
            }
            return instance;
        }
        private set { instance = value; }
    }

    [SerializeField] private string model = "gpt-4o-mini";

    private string apiKey;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadLocalAPIKey();
    }
    #endregion

    // LOAD API KEY ==========================================================
    #region Load API Key

    private class APIKeyClass
    {
        public string apiKey;
    }

    private void LoadLocalAPIKey()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(),"api-key.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"Local API key file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        APIKeyClass config = JsonUtility.FromJson<APIKeyClass>(json);

        if (config == null ||string.IsNullOrWhiteSpace(config.apiKey))
        {
            Debug.LogError("Local API key is missing.");
            return;
        }

        apiKey = config.apiKey;
    }
    #endregion

    
    // GENERATE RESPONSE ==============================================================
    #region Generate Response
    public async Task<string> GenerateResponse(string input)
    {
        return await GenerateResponse(new List<ChatMessage> {new ChatMessage("user", input)});
    }

    public async Task<string> GenerateResponse(List<ChatMessage> messages)
    {
        ChatCompletionRequest requestBody = new ChatCompletionRequest(model, messages.ToArray());

        string json = JsonUtility.ToJson(requestBody);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(
            "https://api.openai.com/v1/chat/completions", 
            UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 30;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                string details = string.IsNullOrEmpty(request.downloadHandler.text)
                    ? request.error
                    : request.downloadHandler.text;

                Debug.Log("OpenAI request failed (HTTP " + request.responseCode + "): " + details);
                return "";
            }

            ChatCompletionResponse response =
                JsonUtility.FromJson<ChatCompletionResponse>(request.downloadHandler.text);

            if (response == null || response.choices == null || response.choices.Length == 0 ||
                response.choices[0].message == null ||
                string.IsNullOrEmpty(response.choices[0].message.content))
            {
                Debug.Log("OpenAI response is empty or invalid.");
                return "";
            }

            return response.choices[0].message.content.Trim();
        }
    }
    #endregion
}