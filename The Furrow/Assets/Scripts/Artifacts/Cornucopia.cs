using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Cornucopia : Artifact
{

    public int rationBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public Cornucopia()
    {
        name = "SpiritdCornucopia";
        title = "Spirit’d Cornucopia";
        desc = "+6 Rations";
        lore ="You find that whatever you put into this horn becomes many meals of food.";
        rationBoost = 6;
        cost = 15;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/SpiritdCornucopia"));
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
        return new Cornucopia();
    }
}