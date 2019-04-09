using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class MercuryDraught : Artifact
{

    public int apBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public MercuryDraught()
    {
        name = "DraughtofGoodHaste";
        title = "Draught of Good Haste";
        desc = "+3 AP on the first turn of each combat";
        lore ="A tall bottle of fast-swirling amber liquid, the glass marked by inscrutable symbols.";
        apBoost = 3;
        cost = 11;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/DraughtofGoodHaste"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.startingAPBoost += apBoost;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.startingAPBoost -= apBoost;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new MercuryDraught();
    }
}