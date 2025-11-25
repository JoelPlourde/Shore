using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public static class I18N {

    private static readonly Dictionary<LanguageKey, Dictionary<string, string>> _translations = new Dictionary<LanguageKey, Dictionary<string, string>>();

    static I18N () {
        LoadLanguageFiles();
    }

    private static void LoadLanguageFiles() {
        // Load language files
        // For each values of the LanguageKey enum:
        foreach (LanguageKey language in (LanguageKey[])Enum.GetValues(typeof(LanguageKey))) {
            TextAsset languageFile = Resources.Load<TextAsset>("Translations/" + language.ToString());
            if (languageFile != null) {
                _translations[language] = FlattenJson(languageFile.text);
            }
        }
    }

    public static string GetValue(string key) {
        key = key.ToLower();

        if (_translations[LanguageKey.EN].ContainsKey(key)) {
            return _translations[LanguageKey.EN][key];
        } else {
            return key;
        }
    }

    public static string GetValue(string key, params object[] args) {
        key = key.ToLower();

        if (_translations[LanguageKey.EN].ContainsKey(key)) {
            return string.Format(_translations[LanguageKey.EN][key], args);
        } else {
            return key;
        }
    }

    public static Dictionary<string, string> FlattenJson(string json)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        JToken token = JToken.Parse(json);
        FillDictionaryFromJToken(dict, token, "");
        return dict;
    }

    private static void FillDictionaryFromJToken(Dictionary<string, string> dict, JToken token, string prefix)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                }
                break;

            case JTokenType.Array:
                int index = 0;
                foreach (JToken value in token.Children())
                {
                    FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                    index++;
                }
                break;

            default:
                dict.Add(prefix, (string)((JValue)token).Value);
                break;
        }
    }

    private static string Join(string prefix, string name) {
        return string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
    }
}