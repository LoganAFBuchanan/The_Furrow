using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FletcherKit : Artifact
{

    public int huntBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public FletcherKit()
    {
        name = "FletcherKit";
        title = "Fletcher's Kit";
        desc = "+2 Rations when Hunting";
        huntBoost = 2;
        cost = 9;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/FletcherKit"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.isHuntBoosted = true;
        player.huntBoost += 2;
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        player.huntBoost -= 2;
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new FletcherKit();
    }
}