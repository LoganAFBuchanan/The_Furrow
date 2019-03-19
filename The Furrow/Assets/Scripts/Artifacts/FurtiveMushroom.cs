using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FurtiveMushroom : Artifact
{

    public int hpBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public FurtiveMushroom()
    {
        name = "Furtive Mushroom";
        desc = "+5 Max HP for both heroes";
        hpBoost = 5;
        cost = 10;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/FurtiveMushroom"));
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
        return new FurtiveMushroom();
    }
}