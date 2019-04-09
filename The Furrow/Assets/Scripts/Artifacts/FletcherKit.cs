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
    public string lore { get; set; }

    public FletcherKit()
    {
        name = "TravelersTrap";
        title = "Traveler’s Trap";
        desc = "+2 Rations when Hunting";
        lore = "A trap left behind by some wayward soul, still rattling with the bones of a rabbit.";
        huntBoost = 2;
        cost = 9;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/TravelersTrap"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.isHuntBoosted = true;
        player.huntBoost += huntBoost;
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        player.huntBoost -= huntBoost;
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new FletcherKit();
    }
}