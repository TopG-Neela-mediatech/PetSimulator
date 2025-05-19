using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FacialAnimationToolLayout : EditorWindow
{
    static string layoutPath = Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Layout/";
    [MenuItem("Window/EMP Studio/Facial Animation Tool/Layout/2 by 3")]
    static void Layout2by3()
    {
        EditorUtility.LoadWindowLayout(layoutPath + "2 by 3 with EMP Facial Animation Tool.wlt");
    }

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Layout/4 Split")]
    static void Layout4Split()
    {
        EditorUtility.LoadWindowLayout(layoutPath + "4 Split with EMP Facial Animation Tool.wlt");
    }

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Layout/Default")]
    static void LayoutDefault()
    {
        EditorUtility.LoadWindowLayout(layoutPath + "Default with EMP Facial Animation Tool.wlt");
    }

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Layout/Tall")]
    static void LayoutTall()
    {
        EditorUtility.LoadWindowLayout(layoutPath + "Tall with EMP Facial Animation Tool.wlt");
    }

    [MenuItem("Window/EMP Studio/Facial Animation Tool/Layout/Wide")]
    static void LayoutWide()
    {
        EditorUtility.LoadWindowLayout(layoutPath + "Wide with EMP Facial Animation Tool.wlt");
    }
}
