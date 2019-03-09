using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactGUI : MonoBehaviour
{

    public GameObject DescPanel;

    public void OnMouseEnter()
    {
        DescPanel.SetActive(true);
    }
    public void OnMouseExit()
    {
        DescPanel.SetActive(false);
    }

}