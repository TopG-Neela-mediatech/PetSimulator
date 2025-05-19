using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System;

public class BlendTreeCreator : EditorWindow
{
    public AnimationClip[] animationClips = new AnimationClip[0];
    public FacialPresetEnum[] facialPreset = new FacialPresetEnum[0];
    Vector2 scrollPos;
    string layerName = "Facial Layer";
    string blendTreeName = "Facial Blend Tree";

    int tempArraySize = 0;

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Blend Tree Creator")]

    public static void ShowWindow()
    {
        GetWindow<BlendTreeCreator>("Blend Tree Creator");
    }

    private void OnGUI()
    {
        minSize = new Vector2(525, minSize.y);
        maxSize = new Vector2(525, maxSize.y);

        /*
        // Let user know that this tool needs some time to process.
        GUILayout.Label("Note: This tool might need a few seconds to process.");
        GUILayout.Label("");
        */

        GameObject selectedObject = Selection.activeGameObject;
        // If there's a selected object?
        if (selectedObject)
        {
            if (selectedObject.GetComponentInParent<Animator>() != null)
            {
                selectedObject = selectedObject.GetComponentInParent<Animator>().gameObject;
                Selection.activeGameObject = selectedObject;
            }

            // And it has Animator component?
            if (selectedObject.GetComponent<Animator>())
            {
                // Finally, it has Animator Controller?
                if (selectedObject.GetComponent<Animator>().runtimeAnimatorController)
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(522));

                    GUILayout.Label("Assign Animation Clips that you want to use.", EditorStyles.boldLabel);

                    GUILayout.BeginHorizontal();
                    // Making Animation Clip Array
                    {
                        ScriptableObject scriptableObj = this;
                        SerializedObject serialObj = new SerializedObject(scriptableObj);
                        SerializedProperty serialProp = serialObj.FindProperty("animationClips");

                        EditorGUILayout.PropertyField(serialProp, true, GUILayout.Width(400));
                        serialObj.ApplyModifiedProperties();
                    }

                    // Animation Clip array size cannot be 0.
                    if (animationClips.Length <= 0)
                    {
                        GUILayout.EndHorizontal();
                        EditorGUILayout.HelpBox("Animation Clips amount cannot be 0.", MessageType.Error);
                    }
                    else
                    {
                        if (tempArraySize != animationClips.Length)
                        {
                            FacialPresetEnum[] tempPresets = facialPreset;
                            facialPreset = new FacialPresetEnum[animationClips.Length];
                            for (int i = 0; i < animationClips.Length && i < tempPresets.Length; i++)
                            {
                                facialPreset[i] = tempPresets[i];
                            }
                            tempArraySize = animationClips.Length;
                        }

                        GUILayout.BeginVertical();
                        GUILayout.Label("");
                        GUILayout.Label("");

                        for (int i = 0; i < animationClips.Length; i++)
                        {
                            facialPreset[i] = (FacialPresetEnum)EditorGUILayout.EnumPopup(facialPreset[i], GUILayout.Width(100));
                        }

                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }

                    // Make sure that every array is filled.
                    bool oneClipIsNULL = false;
                    for (int i = 0; i < animationClips.Length; i++)
                    {
                        if (animationClips[i] == null)
                        {
                            oneClipIsNULL = true;
                        }
                    }
                    // One of array hasn't been filled yet.
                    if (oneClipIsNULL == true)
                    {
                        EditorGUILayout.HelpBox("One of Animation Clips is missing!", MessageType.Error);
                    }

                    GUILayout.BeginHorizontal();

                    if (Selection.activeGameObject.GetComponent<EMPComponentControl>() != null)
                    {
                        if (GUILayout.Button("Load EMP Component data"))
                        {
                            EMPComponentControl EMPCC = Selection.activeGameObject.GetComponent<EMPComponentControl>();
                            animationClips = new AnimationClip[EMPCC.everyAnimationSettings.Length];
                            facialPreset = new FacialPresetEnum[EMPCC.everyAnimationSettings.Length];
                            for (int i = 0; i < EMPCC.everyAnimationSettings.Length; i++)
                            {
                                animationClips[i] = EMPCC.everyAnimationSettings[i].animationClip;
                                facialPreset[i] = (FacialPresetEnum)EMPCC.everyAnimationSettings[i].presetIndex;
                                if ((int)facialPreset[i] >= Enum.GetValues(typeof(FacialPresetEnum)).Length)
                                {
                                    facialPreset[i] = FacialPresetEnum.None;
                                    EMPCC.everyAnimationSettings[i].presetIndex = (int)FacialPresetEnum.None;
                                }
                            }
                        }
                    }

                    // Try set presets by clip name
                    if (GUILayout.Button("Try setting Preset tags"))
                    {
                        for (int i = 0; i < animationClips.Length; i++)
                        {
                            string[] presetSection = animationClips[i].name.Split('_');
                            string presetNameFromClip = presetSection[presetSection.Length - 1];
                            for (int j = 0; j < Enum.GetValues(typeof(FacialPresetEnum)).Length; j++)
                            {
                                if (j >= 21) // A, I, U, E, O presets
                                {
                                    if (string.CompareOrdinal(presetNameFromClip, Enum.GetName(typeof(FacialPresetEnum), j)) == 0)
                                    {
                                        facialPreset[i] = (FacialPresetEnum)j;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (presetNameFromClip.Contains(Enum.GetName(typeof(FacialPresetEnum), j)))
                                    {
                                        facialPreset[i] = (FacialPresetEnum)j;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.Label("");

                    /*
                    // Target layer name for new Blend Tree
                    layerName = EditorGUILayout.TextField("Target Layer", layerName);
                    GUILayout.Label("Note: Leave this empty is same as Facial layer");

                    GUILayout.Label("");

                    // Target Bleend Tree name
                    blendTreeName = EditorGUILayout.TextField("Blend Tree Name", blendTreeName);

                    // Let user know that this task will overwrite component variables.
                    GUILayout.Label("Note: This task will overwrite Blend Tree, Parameter component and Scripts.");
                    */

                    //Everything has set and good to go.
                    if (animationClips.Length > 0)
                    {
                        // Press this Button to generate scripts.
                        if (GUILayout.Button("Generate scripts"))
                        {
                            string outputPath = "";
                            string outputContent = "";
                            string editedObjectName = selectedObject.name.Replace(" ", "_");    // Replace blank space into _ from object name

                            Directory.CreateDirectory(Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script");
                            Directory.CreateDirectory(Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script/Script for " + selectedObject.name);

                            SetBlendTree(selectedObject);

                            // Literally generate scripts from scratch...
                            for (int i = 0; i < animationClips.Length; i++)
                            {
                                // In case the animation clip has blank space on its name.
                                string editedAnimationName = animationClips[i].name.Replace(" ", "_");

                                outputPath = Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script/Script for " + selectedObject.name + "/EMPStudio_" + editedObjectName + "_" + editedAnimationName + ".cs";
                                outputContent =
                                    "using UnityEngine;" + "\n" +
                                    "using UnityEditor;" + "\n" +
                                    "[AddComponentMenu(\"\")]" + "\n" +
                                    "[ExecuteInEditMode]" + "\n" +
                                    "public class EMPStudio_" + editedObjectName + "_" + editedAnimationName + " : MonoBehaviour" + "\n" +
                                    "{" + "\n" +
                                    "    [Range(0f,1f)]" + "\n" +
                                    "    public float parameterValue = 0;" + "\n" +
                                    "    private string parameterName = \"" + animationClips[i].name + "\";" + "\n" +
                                    "    private int index = 0;" + "\n" +
                                    "    private EMPComponentControl empComponentControl;" + "\n" +
                                    "    private Animator animator;" + "\n" +
                                    "    void Start()" + "\n" +
                                    "    {" + "\n" +
                                    "        empComponentControl = GetComponent<EMPComponentControl>();" + "\n" +
                                    "        animator = GetComponent<Animator>();" + "\n" +
                                    "        for (int i = 0; i < empComponentControl.everyAnimationSettings.Length; i++)" + "\n" +
                                    "        {" + "\n" +
                                    "            if (string.CompareOrdinal(empComponentControl.everyAnimationSettings[i].animationClip.name, parameterName) == 0)" + "\n" +
                                    "            {" + "\n" +
                                    "                index = i;" + "\n" +
                                    "                break;" + "\n" +
                                    "            }" + "\n" +
                                    "        }" + "\n" +
                                    "    }" + "\n" +
                                    "    void Update()" + "\n" +
                                    "    {" + "\n" +
                                    "        if (empComponentControl.updateEMPValues == true)" + "\n" +
                                    "        {" + "\n" +
                                    "            parameterValue = empComponentControl.everyAnimationSettings[index].parameterValue;" + "\n" +
                                    "        }" + "\n" +
                                    "        empComponentControl.ChangeParameter(parameterName, parameterValue);" + "\n" +
                                    "        if (Application.isEditor)" + "\n" +
                                    "        {" + "\n" +
                                    "            if (empComponentControl.hideEMPScript == true)" + "\n" +
                                    "            {" + "\n" +
                                    "                if (empComponentControl.everyAnimationSettings[index].highlight)" + "\n" +
                                    "                    hideFlags = HideFlags.None;" + "\n" +
                                    "                else" + "\n" +
                                    "                    hideFlags = HideFlags.HideInInspector;" + "\n" +
                                    "            }" + "\n" +
                                    "            else" + "\n" +
                                    "            {" + "\n" +
                                    "                empComponentControl.everyAnimationSettings[index].highlight = false;" + "\n" +
                                    "                hideFlags = HideFlags.None;" + "\n" +
                                    "            }" + "\n" +
                                    "        }" + "\n" +
                                    "    }" + "\n" +
                                    "    void LateUpdate()" + "\n" +
                                    "    {" + "\n" +
                                    "        if (empComponentControl == null)" + "\n" +
                                    "        {" + "\n" +
                                    "            DestroyImmediate(this);" + "\n" +
                                    "        }" + "\n" +
                                    "        if (empComponentControl.updateEMPValues == true)" + "\n" +
                                    "        {" + "\n" +
                                    "            parameterValue = empComponentControl.everyAnimationSettings[index].parameterValue;" + "\n" +
                                    "            if (index == empComponentControl.everyAnimationSettings.Length - 1)" + "\n" +
                                    "            {" + "\n" +
                                    "                empComponentControl.updateEMPValues = false;" + "\n" +
                                    "            }" + "\n" +
                                    "        }" + "\n" +
                                    "        if (Application.isEditor && EditorApplication.isPlaying == false && empComponentControl.updateEMPValues == false)" + "\n" +
                                    "            parameterValue = animator.GetFloat(parameterName);" + "\n" +
                                    "    }" + "\n" +
                                    "}" + "\n"
                                    ;

                                //This will overwrite everything even there were files with same name.
                                File.WriteAllText(outputPath, outputContent);

                                AssetDatabase.Refresh();
                            }

                            if (animationClips.Length == 1)
                                Debug.Log("EMP Studio : Script is successfully generated to \"" + Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script\" folder.");
                            else
                                Debug.Log("EMP Studio : Scripts are successfully generated to \"" + Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script\" folder.");
                        }

                        // Press this Button after generate scripts to add them to selected object.
                        if (GUILayout.Button("Apply components"))
                        {
                            string editedAnimationName = "";
                            string editedObjectName = "";
                            bool doesScriptExist = true;

                            // Before we start, make sure that user had generated scripts.
                            for (int i = 0; i < animationClips.Length; i++)
                            {
                                editedAnimationName = animationClips[i].name.Replace(" ", "_");
                                editedObjectName = selectedObject.name.Replace(" ", "_");

                                if (System.IO.File.Exists(Application.dataPath + "/EMP Studio/Facial Animation Tool/Output Script/Script for " + selectedObject.name + "/EMPStudio_" + editedObjectName + "_" + editedAnimationName + ".cs") == false)
                                {
                                    doesScriptExist = false;
                                    break;
                                }
                            }

                            editedAnimationName = "";
                            editedObjectName = "";

                            if (doesScriptExist == false)
                            {
                                Debug.LogError("EMP Studio : Please generate scripts first.");
                            }
                            else
                            {
                                if (selectedObject.GetComponent<EMPComponentControl>() == null)
                                    selectedObject.AddComponent<EMPComponentControl>();

                                EMPComponentControl EMPCC = selectedObject.GetComponent<EMPComponentControl>();

                                EMPAnimationParameters[] tempParameters = new EMPAnimationParameters[animationClips.Length];

                                for (int i = 0; i < animationClips.Length; i++)
                                {
                                    // In case the animation clip has blank space on its name.
                                    editedAnimationName = animationClips[i].name.Replace(" ", "_");
                                    editedObjectName = selectedObject.name.Replace(" ", "_");
                                    string assetImportPath = "EMPStudio_" + editedObjectName + "_" + editedAnimationName;

                                    // AddComponent to selected object.
                                    System.Type freshComponent = System.Type.GetType(assetImportPath + ", Assembly-CSharp");

                                    if (selectedObject.GetComponent(freshComponent) == null)
                                        selectedObject.AddComponent(freshComponent);


                                    tempParameters[i].animationClip = animationClips[i];
                                    tempParameters[i].parameterValue = 0;
                                    tempParameters[i].highlight = false;
                                    tempParameters[i].presetIndex = (int)facialPreset[i];
                                }

                                EMPCC.everyAnimationSettings = tempParameters;

                                if (animationClips.Length == 1)
                                    Debug.Log("EMP Studio : Successfully applied component to selected object.");
                                else
                                    Debug.Log("EMP Studio : Successfully applied components to selected object.");
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                // It doesn't have Animator Controller.
                else
                {
                    EditorGUILayout.HelpBox("Selected object's Animator component doesn't have \"Animator Controller\".", MessageType.Error);
                }
            }
            // It doesn't have Animator component.
            else
            {
                EditorGUILayout.HelpBox("Selected object doesn't contain \"Animator\" component.", MessageType.Error);
            }
        }
        // User hasn't selected anything yet.
        else
        {
            EditorGUILayout.HelpBox("Please select the object with \"Animator\" component from the scene.", MessageType.Info);
        }
    }

    // Create Blend Tree function.
    void SetBlendTree(GameObject selectedObject)
    {
        AnimatorController controller = (UnityEditor.Animations.AnimatorController)selectedObject.GetComponent<Animator>().runtimeAnimatorController;

        // Edit "layer name" to Facial Layer if it's empty.
        if (layerName == "") layerName = "Facial Layer";

        // Look for matching layer.
        bool foundMatchingLayer = false;
        int layerIndex = 0;
        for (int i = 0; i < controller.layers.Length; i++)
        {
            if (controller.layers[i].name == layerName)
            {
                foundMatchingLayer = true;
                layerIndex = i;
                break;
            }
        }

        // If it couldn't find one then create new one. (This function only needs layer index.)
        if (foundMatchingLayer == false)
        {
            AnimatorControllerLayer targetLayer = new AnimatorControllerLayer();
            targetLayer.name = layerName;
            AnimatorStateMachine asm = new AnimatorStateMachine();
            asm.name = layerName;
            targetLayer.stateMachine = asm;
            targetLayer.defaultWeight = 1;
            targetLayer.avatarMask = SetAvatarMask(selectedObject);
            AssetDatabase.AddObjectToAsset(asm, controller);
            controller.AddLayer(targetLayer);

            layerIndex = controller.layers.Length - 1;
        }

        // Create new Blend Tree
        BlendTree BT = new BlendTree();
        var trackedStateMachine = controller.layers[layerIndex].stateMachine;
        for (int i = 0; i < trackedStateMachine.states.Length; i++)
        {
            var trackedBlendTree = (BlendTree)trackedStateMachine.states[i].state.motion;
            if (trackedBlendTree.name == blendTreeName)
            {
                //If there is a existed one, destroy it.
                trackedStateMachine.RemoveState(trackedStateMachine.states[i].state);
                break;
            }
        }

        controller.CreateBlendTreeInController(blendTreeName, out BT, layerIndex);

        BT.blendType = BlendTreeType.Direct;

        using (var so = new SerializedObject(BT))
        {
            so.FindProperty("m_NormalizedBlendValues").boolValue = true;
            so.ApplyModifiedProperties();
        }

        // Add every Animation Clips into Blend Tree as Child Motion.
        for (int i = 0; i < animationClips.Length; i++)
        {
            // Look for matching parameter from controller.
            bool foundMatchingParameter = false;
            for (int j = 0; j < controller.parameters.Length; j++)
            {
                if (controller.parameters[j].name == animationClips[i].name)
                {
                    foundMatchingParameter = true;

                    ChildMotion[] tempChildren = BT.children;
                    ChildMotion tempMotion = new ChildMotion();
                    tempMotion.timeScale = 1f;
                    tempMotion.directBlendParameter = controller.parameters[j].name;
                    tempMotion.motion = animationClips[i];
                    ArrayUtility.Add(ref tempChildren, tempMotion);
                    BT.children = tempChildren;

                    break;
                }
            }
            // We have to create new parameter.
            if (foundMatchingParameter == false)
            {
                controller.AddParameter(animationClips[i].name, AnimatorControllerParameterType.Float);
                ChildMotion[] tempChildren = BT.children;
                ChildMotion tempMotion = new ChildMotion();
                tempMotion.timeScale = 1f;
                tempMotion.directBlendParameter = animationClips[i].name;
                tempMotion.motion = animationClips[i];
                ArrayUtility.Add(ref tempChildren, tempMotion);
                BT.children = tempChildren;
            }
        }

        Debug.Log("EMP Studio : \"" + blendTreeName + "\" has been successfully created.");
        AssetDatabase.Refresh();
    }

    AvatarMask SetAvatarMask(GameObject selectedObject)
    {
        AvatarMask avatarMask = new AvatarMask();
        avatarMask.AddTransformPath(selectedObject.transform);

        string[] transformPath;
        string targetTransform = "";

        int targetParentIndex = 0;
        int targetParentDepth = 0;

        // Search for targetParent name
        for (int i = 0; i < selectedObject.GetComponentsInChildren<Transform>().Length; i++)
        {
            if (selectedObject.GetComponentsInChildren<Transform>()[i].name.Contains("Head") || selectedObject.GetComponentsInChildren<Transform>()[i].name.Contains("head"))
            {
                targetTransform = selectedObject.GetComponentsInChildren<Transform>()[i].name;
                break;
            }
        }

        // Search for targetParent index and depth
        for (int i = 0; i < avatarMask.transformCount; i++)
        {
            transformPath = avatarMask.GetTransformPath(i).Split('/');
            if (transformPath[transformPath.Length - 1] == targetTransform)
            {
                targetParentIndex = i;
                targetParentDepth = transformPath.Length - 1;
                break;
            }
        }

        // Disable every transform of AvatarMask
        for (int i = 0; i < avatarMask.transformCount; i++)
        {
            avatarMask.SetTransformActive(i, false);
        }

        // Enable target head only
        for (int i = targetParentIndex + 1; i < avatarMask.transformCount; i++)
        {
            string[] tempPath = avatarMask.GetTransformPath(i).Split('/');
            if (tempPath.Length - 1 <= targetParentDepth)
            {
                break;
            }

            avatarMask.SetTransformActive(i, true);
        }

        // Save AvatarMask as asset file
        string path = "Assets/EMP Studio/Facial Animation Tool/Output Script/Script for " + selectedObject.name + "/" + selectedObject.name + "AvatarMask.mask";
        AssetDatabase.CreateAsset(avatarMask, path);

        return avatarMask;
    }
}
