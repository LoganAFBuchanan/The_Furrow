using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ScarredJournal : Artifact
{

    public float bondBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public ScarredJournal()
    {
        name = "ScarredJournal";
        title = "Scarred Journal";
        desc = "+25% Bond from Combat";
        bondBoost = 0.25f;
        cost = 12;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/ScarredJournal"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.isBondBoosted = true;
        player.bondBoost += bondBoost;
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        
        player.bondBoost -= bondBoost;
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new ScarredJournal();
    }
}