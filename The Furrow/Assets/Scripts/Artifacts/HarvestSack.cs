using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HarvestSack : Artifact
{

    public int rationBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public HarvestSack()
    {
        name = "HarvestSack";
        title = "Harvest Sack";
        desc = "+4 Rations";
        rationBoost = 4;
        cost = 11;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/HarvestSack"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.SetRationCount(rationBoost);
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new HarvestSack();
    }
}