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
    public void LoadArtifactToGUI(string name)
    {
        GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = Instantiate(Resources.Load<GameObject>("Artifacts/" + name));

        RectTransform rectT = artifactUI.GetComponent<RectTransform>();
        rectT.SetParent(this.GetComponent<RectTransform>());
        rectT.localScale = new Vector3(1,1,1);
        rectT.localRotation = Quaternion.identity;

        artifacts.Add(artifactUI);
        UpdateCollection();
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