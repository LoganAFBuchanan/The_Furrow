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
    public string lore { get; set; }

    public HarvestSack()
    {
        name = "SpiritdPlatter";
        title = "Spirit’d Platter";
        desc = "+4 Rations";
        lore = "You find that whatever you put onto this platter becomes a hearty meal.";
        rationBoost = 4;
        cost = 11;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/SpiritdPlatter"));
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