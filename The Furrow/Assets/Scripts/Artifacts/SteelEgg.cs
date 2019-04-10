using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SteelEgg : Artifact
{

    public int defBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public SteelEgg()
    {
        name = "TitanstoneShield";
        title = "Titanstone Shield";
        desc = "Start Combat with +3 Defense";
        lore = "A shield studded with blue stones, which glow like diluted stars.";
        defBoost = 3;
        cost = 7;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/TitanstoneShield"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.startingDefence += defBoost;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.startingDefence -= defBoost;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new SteelEgg();
    }
}