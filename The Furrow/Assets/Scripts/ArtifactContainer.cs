using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ArtifactContainer : MonoBehaviour
{

    public List<GameObject> artifacts;

    private int gutter = 3;
    private int width = 50;

    //Loads the artifact resource and adds it to the GUI
    public void LoadArtifactToGUI(Artifact arti)
    {
        GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + arti.name);
        artifactUI = Instantiate(Resources.Load<GameObject>("Artifacts/" + arti.name));

        Text artifactName = artifactUI.transform.GetChild(0).Find("Name").GetComponent<Text>();
        Text artifactDesc = artifactUI.transform.GetChild(0).Find("Desc").GetComponent<Text>();

        artifactName.text = arti.title;
        artifactDesc.text = arti.desc;

        RectTransform rectT = artifactUI.GetComponent<RectTransform>();
        rectT.SetParent(this.GetComponent<RectTransform>());
        rectT.localScale = new Vector3(1,1,1);
        rectT.localRotation = Quaternion.identity;

        artifacts.Add(artifactUI);
        UpdateCollection();
    }

    //Eliminate a specified Artifact from the displayed list
    public void RemoveArtifactFromGUI(Artifact arti)
    {
        //Debug.Log("Container: Removing " + name);
        string clonedName = arti.name + "(Clone)";
        //Debug.Log("Cloned Name: " + clonedName);
        foreach(GameObject art in artifacts)
        {
            //Debug.Log(art.name);
            if(art.name == clonedName)
            {
                artifacts.Remove(art);
                Destroy(art);
                UpdateCollection();
                return;
            }
        }
    }


    //Position the artifacts in a row
    public void UpdateCollection()
    {
        for(int i = 0; i < artifacts.Count; i++)
        {
            artifacts[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(((i * gutter) + gutter) + (width * i),-gutter, 0);
        }
    }

}