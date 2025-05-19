using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class FacialPresetSetting : EditorWindow
{
    private string enumPath = "";
    string fromFile = "";
    bool fileRead = false;
    string[] enumList;
    int enumSize = 0;
    int tempSize = 0;

    int prePresetAmount = 25;  //exclude "None"

    Vector2 scrollPos;

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Facial Preset Setting")]
    static void ShowWindow()
    {
        GetWindow<FacialPresetSetting>("Facial Preset Setting");
    }

    private void OnEnable()
    {
        // Make sure FacialPresetEnum.cs is exist.
        fileRead = false;
        if (System.IO.File.Exists(Application.dataPath + "/EMP Studio/Facial Animation Tool/Facial Preset/FacialPresetEnum.cs"))
        {
            enumPath = Application.dataPath + "/EMP Studio/Facial Animation Tool/Facial Preset/FacialPresetEnum.cs";
        }
        else
        {
            Debug.LogError("EMP Studio : File is missing! Please Reimport the package!");
        }
    }
    private void OnGUI()
    {
        minSize = new Vector2(260, 145); // 5 (original size is 140) is the last value for safty size.

        // Read FacialPresetEnum.cs only once to get raw enumList values.
        if (fileRead == false)
        {
            StreamReader reader = new StreamReader(enumPath);
            fromFile = reader.ReadToEnd();
            reader.Close();

            enumList = fromFile.Split(',');
            enumSize = enumList.Length - 2 - prePresetAmount;
            tempSize = enumSize;

            fileRead = true;
        }

        /*
        EditorGUILayout.HelpBox("Note: This preset is universal.", MessageType.Info);
        EditorGUILayout.HelpBox("Only PNG files are accepted as preset icon.", MessageType.Info);
        */

        // Apply facial presets with pre-presets
        if (GUILayout.Button("Apply Facial Preset"))
        {
            bool detectedSamePresetName = false;
            bool detectedEmptyName = false;
            bool detectedStartWithUnvaliedLetter = false;
            bool detectedContainingSymbols = false;

            // But you cannot leave anything with blank or same names.
            for (int i = prePresetAmount + 1; i < enumList.Length - 1; i++)   // Exclude "None = 0", pre-presets and "};"
            {
                for (int j = prePresetAmount + 1; j < enumList.Length - 1; j++)   // Exclude "None = 0", pre-presets and "};"
                {
                    if (i == j) continue;
                    if (string.CompareOrdinal(enumList[i], enumList[j]) == 0)   // Do not allow same names.
                    {
                        detectedSamePresetName = true;
                        break;
                    }
                    if (enumList[i] == "")  // Do not allow empty space.
                    {
                        detectedEmptyName = true;
                        break;
                    }
                    if (!Regex.IsMatch(enumList[i], @"^[a-zA-Z]"))  // Only allow names start with alphabet.
                    {
                        detectedStartWithUnvaliedLetter = true;
                        break;
                    }
                    if (!enumList[i].All(c => Char.IsLetterOrDigit(c))) // Only allow names with letters and numbers.
                    {
                        detectedContainingSymbols = true;
                        break;
                    }
                }
                if (detectedSamePresetName == true || detectedEmptyName == true || detectedStartWithUnvaliedLetter == true || detectedContainingSymbols == true)
                    break;
            }

            if (detectedEmptyName == true)
            {
                Debug.LogError("EMP Studio : You cannot leave any presets' name empty.");
            }
            else if (detectedSamePresetName == true)
            {
                Debug.LogError("EMP Studio : You cannot assign presets with same name.");
            }
            else if (detectedStartWithUnvaliedLetter == true)
            {
                Debug.LogError("EMP Studio : First letter of preset name must be alphabet.");
            }
            else if (detectedContainingSymbols == true)
            {
                Debug.LogError("EMP Studio : Preset name cannot contain sysmbols include space");
            }
            // Everything is good to go. Write it into FacialPresetEnum.cs
            else
            {
                string outputPath = Application.dataPath + "/EMP Studio/Facial Animation Tool/Facial Preset/FacialPresetEnum.cs";
                string outputContent = "public enum FacialPresetEnum{None = 0,";
                outputContent = outputContent +
                    "Default,EyeClose,MouthOpen,Sleep,Tired," +
                    "Happy,Smile,Flush,Excited,Fresh," +
                    "Nervous,Frustrated,Despair,Sad,Cry," +
                    "Angry,Shout,Confounded,Unpleasant,Grimace," +
                    "A,I,U,E,O,";

                for (int i = 1 + prePresetAmount; i < enumList.Length - 1; i++)
                {
                    enumList[i].Replace(' ', '_');  // Remove space
                    outputContent = outputContent + enumList[i] + ",";
                }

                outputContent = outputContent + "};";


                File.WriteAllText(outputPath, outputContent);
                AssetDatabase.Refresh();

                Debug.Log("EMP Studio : Successfully updated Facial Preset.");
            }
        }

        // Set enum size. But exclude None and pre-presets.
        enumSize = EditorGUILayout.IntField("Facial Preset size", enumSize);
        if (enumSize < 0) enumSize = 0;
        if (enumSize != enumList.Length - 2 - prePresetAmount)
        {
            string[] tempEnum = enumList;
            enumList = new string[enumSize + 2 + prePresetAmount];

            for (int i = 0; i < tempEnum.Length && i < enumList.Length; i++)
            {
                enumList[i] = tempEnum[i];
                if (tempEnum[i] == "};")
                {
                    enumList[i] = "";
                }
            }

            enumSize = enumList.Length - 2 - prePresetAmount;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));

        for (int i = 1 + prePresetAmount; i < enumList.Length - 1; i++)
        {
            enumList[i] = EditorGUILayout.TextField(enumList[i]);
        }

        EditorGUILayout.EndScrollView();
    }
}
