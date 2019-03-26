using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class AegisCharm : Artifact
{

    public int defMultiplier;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }

    public AegisCharm()
    {
        name = "AegisCharm";
        title = "Aegis Charm";
        desc = "Double the effectiveness of the Defend Action";
        defMultiplier = 1;
        cost = 14;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/AegisCharm"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.defenseStrength += defMultiplier;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.defenseStrength += defMultiplier;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new AegisCharm();
    }
}