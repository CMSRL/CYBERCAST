using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.IO;
using UnityEngine;
using TMPro;
#if WINDOWS_UWP
    using Windows.Storage;
#endif

public class DebugScript : MonoBehaviour
{


    /// <summary>
    /// Event fired when we want to clear the log.
    /// </summary>
    public delegate void ClearLog();
    public event ClearLog OnClearLog;

    /// <summary>
    /// Event fired when message needs to be added to the UI.
    /// </summary>
    /// <param name="message">Message.</param>
    public delegate void Message(string message);
    public event Message OnMessage;

    //private TextMeshPro _dialogUI;
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        //_dialogUI = GetComponentInChildren<TextMeshPro>(true);
        Application.logMessageReceived += Application_logMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Capture logs from Unity and add them to the content.
    /// Only logs, errors and exceptions are added.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="stackTrace"></param>
    /// <param name="type"></param>
    private void Application_logMessageReceived(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Error || type == LogType.Exception)
        {
            string msg = message;
            if (type == LogType.Exception || type == LogType.Error)
            {
                msg += $"\n*** stack trace ***\n{stackTrace}\n*** end stack trace ***";
            }
            WriteMessage(msg);
        }
    }

    /// <summary>
    /// Add the message to the console or overwrite contents with it.
    /// </summary>
    /// <param name="message">Message.</param>
    public void WriteMessage(string message)
    {
        text.text += "\n" + message;
    }
}

