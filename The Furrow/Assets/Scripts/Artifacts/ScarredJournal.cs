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
    public string lore { get; set; }

    public ScarredJournal()
    {
        name = "CompatriotsLink";
        title = "Compatriot’s Link";
        desc = "+25% Bond from Combat";
        lore = "An enchanted ear cuff for each traveller, which allows for in-combat communication.";
        bondBoost = 0.25f;
        cost = 12;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/CompatriotsLink"));
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