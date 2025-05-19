using UnityEngine;
using UnityEditor;
using System;

public class FacialPreset : EditorWindow
{
    private int windowGab = 5;
    private int buttonSize = 150;
    private int buttonGab = 3;
    private Vector2 scrollPos;
    private Texture[] button_tex;
    private GUIContent[] button_tex_con;

    private EMPComponentControl EMPCC;

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Facial Icon Preset Panel")]
    static void ShowWindow()
    {
        GetWindow<FacialPreset>("Facial Icon Preset");
    }

    private void OnEnable()
    {
        button_tex = new Texture[Enum.GetValues(typeof(FacialPresetEnum)).Length];
        button_tex_con = new GUIContent[Enum.GetValues(typeof(FacialPresetEnum)).Length];

        for (int i = 0; i < button_tex.Length; i++)
        {
            //note: used emoji from
            //https://openmoji.org/
            //resize to 256*256px
            //Primer font is used for A, I, U, E, O presets

            // Get icon textures by comparing FacialPresetEnum values
            string iconNumber = i.ToString();
            if (System.IO.File.Exists(Application.dataPath + "/EMP Studio/Facial Animation Tool/Facial Preset/Icon/" + Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + ".png"))
            {
                button_tex[i] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EMP Studio/Facial Animation Tool/Facial Preset/Icon/" + Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + ".png", typeof(Texture));
            }
            else if (System.IO.File.Exists(Application.dataPath + "/EMP Studio/Facial Animation Tool/Facial Preset/Icon/" + Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + ".PNG"))
            {
                button_tex[i] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EMP Studio/Facial Animation Tool/Facial Preset/Icon/" + Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + ".PNG", typeof(Texture));
            }
            else
            {
                button_tex[i] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EMP Studio/Facial Animation Tool/Resource/Icon/MissingIcon.png", typeof(Texture));
            }

            button_tex_con[i] = new GUIContent(Enum.GetNames(typeof(FacialPresetEnum))[i].ToString(), button_tex[i], Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + " preset");
        }
    }

    private void OnGUI()
    {
        minSize = new Vector2((buttonSize * 2) + windowGab + buttonGab + 18, minSize.y); // 18 is the last value for safty size

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null && selectedObject.GetComponentInParent<EMPComponentControl>() != null)
        {
            selectedObject = selectedObject.GetComponentInParent<EMPComponentControl>().gameObject;
            Selection.activeGameObject = selectedObject;
        }

        // Make sure that use has selected GameObject with EMPComponentControl component.
        if (selectedObject != null && selectedObject.GetComponent<EMPComponentControl>() != null)
        {
            if (EMPCC == null || EMPCC.gameObject != selectedObject)
                EMPCC = selectedObject.GetComponent<EMPComponentControl>();

            buttonSize = EditorGUILayout.IntSlider("Icon Size", buttonSize, 50, (int)((position.width / 2) - windowGab - buttonGab - 5));

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 22));

            int buttonRowIndex = 1;
            bool startHorizontal = false;
            GUILayout.BeginVertical();
            for (int i = 1; i < Enum.GetValues(typeof(FacialPresetEnum)).Length; i++)
            {
                bool indexOccupied = false;
                bool[] foundIndex = new bool[EMPCC.everyAnimationSettings.Length]; for (int k = 0; k < foundIndex.Length; k++) foundIndex[k] = false;

                for (int j = 0; j < EMPCC.everyAnimationSettings.Length; j++)
                {
                    // Has this index been occupied?
                    if (i == EMPCC.everyAnimationSettings[j].presetIndex)
                    {
                        indexOccupied = true;
                        foundIndex[j] = true;
                    }
                }

                if (startHorizontal == false)
                {
                    GUILayout.BeginHorizontal();
                    startHorizontal = true;
                }

                if (indexOccupied == true) GUI.enabled = true; else GUI.enabled = false;

                // Set percentage before write it on buttons. 
                for (int k = 0; k < foundIndex.Length; k++)
                {
                    if (foundIndex[k] == true)
                    {
                        button_tex_con[i].text = Enum.GetNames(typeof(FacialPresetEnum))[i].ToString() + "\n(" + EMPCC.everyAnimationSettings[k].parameterValue * 100 + "%)";
                    }
                }
                // Make preset button with icon
                if (GUILayout.Button(button_tex_con[i], GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    for (int k = 0; k < foundIndex.Length; k++)
                    {
                        if (foundIndex[k] == true)
                        {
                            EMPCC.DisengageHightlightComponent();
                            EMPCC.HightlightComponent(k);
                        }
                    }
                }

                // If it cannot put another button in this row, put it on next row. Or End the row if it's the last enum value
                if ((buttonRowIndex * (buttonSize + buttonGab)) + windowGab >= position.width - buttonSize - 13 || i == Enum.GetValues(typeof(FacialPresetEnum)).Length - 1)
                {
                    GUILayout.EndHorizontal();
                    startHorizontal = false;
                    buttonRowIndex = 1;
                }
                else
                    buttonRowIndex++;
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("Please select the object with \"EMP Component Control\" component from the scene.", MessageType.Info);
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}
