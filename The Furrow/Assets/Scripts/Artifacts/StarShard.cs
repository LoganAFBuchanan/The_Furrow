using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class StarShard : Artifact
{

    public int apBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public StarShard()
    {
        name = "StarShard";
        title = "Shard of the Star";
        desc = "+1 Max AP";
        lore = "A gently pulsing shard of light, which can only have come from a star.";
        apBoost = 5;
        cost = 10;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/StarShard"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalActionPoints += apBoost;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalActionPoints -= apBoost;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new StarShard();
    }
}