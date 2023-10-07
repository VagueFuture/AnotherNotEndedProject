using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogSystem
{
    static TextAsset TA = Resources.Load<TextAsset>("DialogSystemsDialogs");
    public enum DialogType
    {
        Que_,
        No_,
        Yes_
    };

    public static Dictionary<string, string> inGameTexts = new Dictionary<string, string>();

    public static Dictionary<string, string> GetText(string key = "Tonnel_")
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        string allText = TA.text;
        string[] fields = allText.Split('\n');
        foreach (var v in fields)
        {
            string[] splitKeys = v.Split(';');
            if (splitKeys[0].Contains(key))
                result.Add(splitKeys[0], splitKeys[1]);
        }
        inGameTexts = result;
        return result;
    }

    public static string GetDialogFromKey(string key)
    {
        if (!inGameTexts.ContainsKey(key))
            return "Text_Not_Found";
        return inGameTexts[key];
    }
}
