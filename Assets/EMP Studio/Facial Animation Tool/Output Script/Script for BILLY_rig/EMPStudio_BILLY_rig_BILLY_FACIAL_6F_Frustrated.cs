using UnityEngine;
using UnityEditor;
[AddComponentMenu("")]
[ExecuteInEditMode]
public class EMPStudio_BILLY_rig_BILLY_FACIAL_6F_Frustrated : MonoBehaviour
{
    [Range(0f,1f)]
    public float parameterValue = 0;
    private string parameterName = "BILLY_FACIAL_6F_Frustrated";
    private int index = 0;
    private EMPComponentControl empComponentControl;
    private Animator animator;
    void Start()
    {
        empComponentControl = GetComponent<EMPComponentControl>();
        animator = GetComponent<Animator>();
        for (int i = 0; i < empComponentControl.everyAnimationSettings.Length; i++)
        {
            if (string.CompareOrdinal(empComponentControl.everyAnimationSettings[i].animationClip.name, parameterName) == 0)
            {
                index = i;
                break;
            }
        }
    }
    void Update()
    {
        if (empComponentControl.updateEMPValues == true)
        {
            parameterValue = empComponentControl.everyAnimationSettings[index].parameterValue;
        }
        empComponentControl.ChangeParameter(parameterName, parameterValue);
        if (Application.isEditor)
        {
            if (empComponentControl.hideEMPScript == true)
            {
                if (empComponentControl.everyAnimationSettings[index].highlight)
                    hideFlags = HideFlags.None;
                else
                    hideFlags = HideFlags.HideInInspector;
            }
            else
            {
                empComponentControl.everyAnimationSettings[index].highlight = false;
                hideFlags = HideFlags.None;
            }
        }
    }
    void LateUpdate()
    {
        if (empComponentControl == null)
        {
            DestroyImmediate(this);
        }
        if (empComponentControl.updateEMPValues == true)
        {
            parameterValue = empComponentControl.everyAnimationSettings[index].parameterValue;
            if (index == empComponentControl.everyAnimationSettings.Length - 1)
            {
                empComponentControl.updateEMPValues = false;
            }
        }
        if (Application.isEditor && EditorApplication.isPlaying == false && empComponentControl.updateEMPValues == false)
            parameterValue = animator.GetFloat(parameterName);
    }
}
