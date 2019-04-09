using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FurrowMorel : Artifact
{

    public int hpBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public FurrowMorel()
    {
        name = "CurativeCuff";
        title = "Curative Cuff";
        desc = "+7 Max HP for both heroes";
        lore = "A metal cuff ringed with blue glowing stones, which cleanse the body’s blood.";
        hpBoost = 7;
        cost = 16;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/CurativeCuff"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalHitPoints += hpBoost;
            hero.HitPoints += hpBoost;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalHitPoints -= hpBoost;
            hero.HitPoints -= hpBoost;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new FurrowMorel();
    }
}