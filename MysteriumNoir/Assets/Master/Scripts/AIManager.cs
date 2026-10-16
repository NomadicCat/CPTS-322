using System;
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
class ChatMessage
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
    // VARIABLES AND INSTANCE ==========================================================
    #region Variables and Instance
    public static AIManager Instance;

    [SerializeField] private string apiKey;
    [SerializeField] private string model;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion
    
    // GENERATE RESPONSE ==============================================================
    #region Generate Response
    public async Task<string> GenerateResponse(string input)
    {
        ChatMessage[] messages = {new ChatMessage("user", input)};
        ChatCompletionRequest requestBody = new ChatCompletionRequest(model, messages);

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