using UnityEditor;
using UnityEngine;
using System.IO;

public class FBXClipRename : AssetPostprocessor
{
    string lastCheck;
    bool applyAll = false;
    void OnPreprocessAnimation()
    {
        if (System.IO.File.Exists(Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Remove this to ask renaming later importing AnimationClips.txt"))
        {
            StreamReader reader = new StreamReader(Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Remove this to ask renaming later importing AnimationClips.txt");
            string content = reader.ReadToEnd();
            reader.Close();

            string[] contents = content.Split(' ');

            lastCheck = contents[contents.Length - 4];

            if (int.Parse(contents[contents.Length - 1]) == 1)
            {
                applyAll = true;
            }
        }

        ModelImporter modelImporter = assetImporter as ModelImporter;

        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

        //Modify/Rename animation clips?
        if (clipAnimations.Length == 1)
        {
            string path = modelImporter.assetPath;
            string[] splittedString = path.Split('/');
            string trueName = "";
            trueName = splittedString[splittedString.Length - 1].Replace(" ", "_");   // replace every blank space into _
            trueName = splittedString[splittedString.Length - 1].Replace(".fbx", "");   // lower case filename extension
            trueName = splittedString[splittedString.Length - 1].Replace(".FBX", "");   // upper case filename extension


            if (trueName == lastCheck) return;

            if (applyAll == true)
            {
                clipAnimations[0].name = trueName;
                File.WriteAllText(Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Remove this to ask renaming later importing AnimationClips.txt", "LastCheck = " + trueName + " \nDoNotAsk = 1");
            }
            else
            {
                int option = EditorUtility.DisplayDialogComplex("Rename animation clip", "Do you want to rename the animation clip as file name?\n\n\"" + trueName + "\"", "Yes", "No", "Yes and do not ask again");
                switch (option)
                {
                    case 0:
                        clipAnimations[0].name = trueName;
                        File.WriteAllText(Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Remove this to ask renaming later importing AnimationClips.txt", "LastCheck = " + trueName + " \nDoNotAsk = 0");
                        break;
                    case 1: return;
                    case 2:
                        clipAnimations[0].name = trueName;
                        File.WriteAllText(Application.dataPath + "/EMP Studio/Facial Animation Tool/Resource/Remove this to ask renaming later importing AnimationClips.txt", "LastCheck = " + trueName + " \nDoNotAsk = 1");
                        break;
                }
            }

            //Assign modiffied clip names back to modelImporter
            modelImporter.clipAnimations = clipAnimations;

            //Save
            modelImporter.SaveAndReimport();
        }
    }
}