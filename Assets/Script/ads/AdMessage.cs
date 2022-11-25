using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AdMessage
{
    const string TYPE = "type";
    const string EVENT = "event";
    const string MSG = "msg";

    public string type = "";
    public string adEvent = "";
    public string msg = "";

    public Dictionary<string, string> allParams;

    private AdMessage() { }

    public static AdMessage parse(string msg)
    {
        if (msg == null)
        {
            return null;
        }

        string[] kvString = Regex.Split(msg, ",,");

        Dictionary<string, string> d = new Dictionary<string, string>();

        foreach (string kv in kvString)
        {
            string[] item = Regex.Split(kv, "::");
            if (item.Length != 2)
            {
                return null;
            }

            d.Add(item[0], item[1]);
        }

        AdMessage message = new AdMessage();
        message.allParams = d;
        message.type = d[TYPE];
        message.adEvent = d[EVENT] != null ? d[EVENT] : "";
        if (d.ContainsKey(MSG))
        {
            message.msg = d[MSG];
        }

        return message;
    }
}