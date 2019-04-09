using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class StrappedBundle : Artifact
{

    public int rationBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public StrappedBundle()
    {
        name = "SpiritdDish";
        title = "Spirit’d Dish";
        desc = "+2 Rations";
        lore = "You find that whatever you put onto this dish becomes a small meal.";
        rationBoost = 2;
        cost = 8;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/SpiritdDish"));
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
        return new StrappedBundle();
    }
}