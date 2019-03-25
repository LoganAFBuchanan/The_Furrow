using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ConsortsScribe : Artifact
{

    public int bondMultiplier;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public ConsortsScribe()
    {
        name = "ConsortsScribe";
        title = "Consort's Scribe";
        desc = "Gain double Bond Points at Camp";
        bondMultiplier = 1;
        cost = 9;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/ConsortsScribe"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.isCampBondBoosted = true;
        player.campBondBoost += bondMultiplier;
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        player.campBondBoost -= bondMultiplier;
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new ConsortsScribe();
    }
}