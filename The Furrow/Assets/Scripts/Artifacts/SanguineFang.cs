using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SanguineFang : Artifact
{

    public int killHeal;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public SanguineFang()
    {
        name = "SanguineFang";
        title = "Sanguine Fang";
        desc = "Heal 1 HP upon killing an enemy";
        lore = "A beast's wretched fang laced with crimson.";
        killHeal = 1;
        cost = 13;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/SanguineFang"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.isKillHeal = true;
            hero.killHeal += killHeal;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.isKillHeal = false;
            hero.killHeal -= killHeal;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new FurtiveMushroom();
    }
}