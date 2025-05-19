using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct EMPAnimationParameters
{
    public int presetIndex;
    public AnimationClip animationClip;
    public float parameterValue;
    public bool highlight;
}

[ExecuteInEditMode]
[AddComponentMenu("")]
public class EMPComponentControl : MonoBehaviour
{
    [Tooltip("Make sure you've selected the object.")]
    public bool hideEMPScript = false;

    [HideInInspector]
    public EMPAnimationParameters[] everyAnimationSettings;
    [HideInInspector]
    public bool updateEMPValues = false;

    private GameObject savedRoot;
    private Animator animator;

    private bool valueChanged = false;

    private void Start()
    {
        // Make backup of head from bone tree
        animator = GetComponent<Animator>();

        if (Application.isEditor)
        {
            if (this.gameObject.transform.Find("Head_Backup") == null)
            {
                for (int i = 0; i < this.GetComponentsInChildren<Transform>().Length; i++)
                {
                    if (this.GetComponentsInChildren<Transform>()[i].name.Contains("Head") || this.GetComponentsInChildren<Transform>()[i].name.Contains("head"))
                    {
                        GameObject tempObject = this.GetComponentsInChildren<Transform>()[i].gameObject;
                        savedRoot = Instantiate(tempObject);
                        savedRoot.name = "Head_Backup";
                        savedRoot.transform.parent = this.transform;
                        break;
                    }
                }
            }
            else
            {
                savedRoot = this.gameObject.transform.Find("Head_Backup").gameObject;
            }
        }
    }

    public void AnimatorUpdate()
    {
        // Operate this Animator.Update function only on Editor mode
        if (Application.isEditor && EditorApplication.isPlaying == false)
        {
            if (valueChanged)
            {
                // Animation Clip preview mode
                if (animator.enabled == false)
                {
                    ResetTransform();
                    animator.enabled = true;
                    for (int i = 0; i < everyAnimationSettings.Length; i++)
                    {
                        animator.SetFloat(everyAnimationSettings[i].animationClip.name, everyAnimationSettings[i].parameterValue);
                    }
                    animator.Update(0);
                    animator.enabled = false;

                    valueChanged = false;
                }
                // Normal preview mode
                else
                {
                    ResetTransform();
                    for (int i = 0; i < everyAnimationSettings.Length; i++)
                    {
                        animator.SetFloat(everyAnimationSettings[i].animationClip.name, everyAnimationSettings[i].parameterValue);
                    }
                    animator.Update(0);

                    valueChanged = false;
                }
            }
        }
    }

    private void Update()
    {
        AnimatorUpdate();
    }

    private void LateUpdate()
    {
        if (Application.isEditor && EditorApplication.isPlaying == false) { }
        else
        {
            // Apply paremeter values to Animator.
            for (int i = 0; i < everyAnimationSettings.Length; i++)
            {
                animator.SetFloat(everyAnimationSettings[i].animationClip.name, everyAnimationSettings[i].parameterValue);
            }
        }
    }

    public void ChangeParameter(string animationName, float changedValue)
    {
        // Parameter component had been changed. This function will update parameter array.
        for (int i = 0; i < everyAnimationSettings.Length; i++)
        {
            if (string.CompareOrdinal(animationName, everyAnimationSettings[i].animationClip.name) == 0)
            {
                if (everyAnimationSettings[i].parameterValue != changedValue)
                {
                    everyAnimationSettings[i].parameterValue = changedValue;
                    valueChanged = true;
                    break;
                }
            }
        }
    }

    public void ResetTransform()
    {
        // Animator.Update() has serious animation baking problem. To fix, just overwrite the transform values.
        if (this.gameObject.transform.Find("Head_Backup") != null)
        {
            GameObject tempObject = null;
            for (int i = 0; i < this.GetComponentsInChildren<Transform>().Length; i++)
            {
                if (this.GetComponentsInChildren<Transform>()[i].name.Contains("Head") || this.GetComponentsInChildren<Transform>()[i].name.Contains("head"))
                {
                    tempObject = this.GetComponentsInChildren<Transform>()[i].gameObject;
                    break;
                }
            }
            if (tempObject != null)
            {
                for (int i = 0; i < tempObject.GetComponentsInChildren<Transform>().Length; i++)
                {
                    for (int j = 0; j < savedRoot.GetComponentsInChildren<Transform>().Length; j++)
                    {
                        if (string.CompareOrdinal(tempObject.GetComponentsInChildren<Transform>()[i].name, savedRoot.GetComponentsInChildren<Transform>()[j].name) == 0)
                        {
                            if (tempObject.GetComponentsInChildren<Transform>()[i].localPosition != savedRoot.GetComponentsInChildren<Transform>()[j].localPosition)
                                tempObject.GetComponentsInChildren<Transform>()[i].localPosition = savedRoot.GetComponentsInChildren<Transform>()[j].localPosition;
                            if (tempObject.GetComponentsInChildren<Transform>()[i].localRotation != savedRoot.GetComponentsInChildren<Transform>()[j].localRotation)
                                tempObject.GetComponentsInChildren<Transform>()[i].localRotation = savedRoot.GetComponentsInChildren<Transform>()[j].localRotation;
                            if (tempObject.GetComponentsInChildren<Transform>()[i].localScale != savedRoot.GetComponentsInChildren<Transform>()[j].localScale)
                                tempObject.GetComponentsInChildren<Transform>()[i].localScale = savedRoot.GetComponentsInChildren<Transform>()[j].localScale;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("EMP Studio : Unable to Recover the Transform data.");
        }
    }

    public void HightlightComponent(int index)
    {
        // Hightlight certain component
        hideEMPScript = true;
        everyAnimationSettings[index].highlight = true;
        EditorUtility.SetDirty(this);
    }
    public void DisengageHightlightComponent()
    {
        // Turn off highlight of paremeter component.
        for (int i = 0; i < everyAnimationSettings.Length; i++)
            everyAnimationSettings[i].highlight = false;

        EditorUtility.SetDirty(this);
    }
    public bool GetHighlightedComponent(int index)
    {
        return everyAnimationSettings[index].highlight;
    }
    public void ToggleThisParameter(int index)
    {
        if (everyAnimationSettings[index].parameterValue < 1)
        {
            everyAnimationSettings[index].parameterValue = 1;
        }
        else
        {
            everyAnimationSettings[index].parameterValue = 0;
        }
        updateEMPValues = true;

        AnimatorUpdate();
        EditorUtility.SetDirty(this);
    }
}
