using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchListener : MonoBehaviour
{
    private TwitchListener()
    {

    }
    public int maxMessages = 100; //we start deleting UI elements when the count is larger than this var.
    private LinkedList<GameObject> messages =
        new LinkedList<GameObject>();
    public UnityEngine.UI.InputField inputField;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.RectTransform chatBox;
    public UnityEngine.UI.ScrollRect scrollRect;
    private TwitchIRC IRC;
    private static TwitchListener instance;
    private static List<CommandListener> commandListeners;
    public static TwitchListener Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TwitchListener();
                commandListeners = new List<CommandListener>();
            }
            return instance;
        }
    }
    public IDisposable AddListener(string cmd, int amountToTrigger, Action callback, bool loop = false)
    {
        CommandListener listener = new CommandListener(cmd, amountToTrigger, callback, loop);
        commandListeners.Add(listener);
        return listener;
    }
    void OnCommandRecieved(string cmd)
    {
        for(int i=0;i<commandListeners.Count;i++)
        {
            CommandListener listener = commandListeners[i];
            if (!listener.Listens)
            {
                commandListeners.Remove(listener);
                continue;
            }
            if (listener.Command == cmd)
            {
                listener.CurrentAmount++;
            }
        }
    }
    //when message is recieved from IRC-server or our own message.
    void OnChatMsgRecieved(string msg)
    {
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        //remove old messages for performance reasons.
        if (messages.Count > maxMessages)
        {
            Destroy(messages.First.Value);
            messages.RemoveFirst();
        }

        if (msgString[0]=='!')
        {
            OnCommandRecieved(msgString);
        }
        //add new message.
        //CreateUIMessage(user, msgString);
    }
    void CreateUIMessage(string userName, string msgString)
    {
        Color32 c = ColorFromUsername(userName);
        string nameColor = "#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        GameObject go = new GameObject("twitchMsg");
        var text = go.AddComponent<UnityEngine.UI.Text>();
        var layout = go.AddComponent<UnityEngine.UI.LayoutElement>();
        go.transform.SetParent(chatBox);
        messages.AddLast(go);

        layout.minHeight = 20f;
        text.text = "<color=" + nameColor + "><b>" + userName + "</b></color>" + ": " + msgString;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        scrollRect.velocity = new Vector2(0, 1000f);
    }
    //when Submit button is clicked or ENTER is pressed.
    public void OnSubmit()
    {
        if (inputField.text.Length > 0)
        {
            IRC.SendMsg(inputField.text); //send message.
            CreateUIMessage(IRC.nickName, inputField.text); //create ui element.
            inputField.text = "";
        }
    }
    Color ColorFromUsername(string username)
    {
        UnityEngine.Random.InitState(username.Length + username[0] + username[username.Length - 1]);
        return new Color(UnityEngine.Random.Range(0.25f, 0.55f), UnityEngine.Random.Range(0.20f, 0.55f), UnityEngine.Random.Range(0.25f, 0.55f));
    }
    // Use this for initialization
    void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }
}