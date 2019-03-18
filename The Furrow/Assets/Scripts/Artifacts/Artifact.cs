using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/// <summary>
/// Class representing an artifact the player can find on the overworld
/// </summary>
public interface Artifact
{
    /// <summary>
    /// Determines the cost of an artifact if spawned in a store
    /// </summary>
    int cost { get; set; }

    /// <summary>
    /// Name of the artifact
    /// </summary>
    string name { get; set; }
    string desc { get; set; }
    Image image { get; set; }

    /// <summary>
    /// Describes how the player should be affected
    /// </summary>
    void Apply(OverworldPlayer player);

    /// <summary>
    /// Removes the effects of the artifact
    /// </summary>
    void Undo(OverworldPlayer player);

    /// <summary>
    /// Returns deep copy of the Buff object.
    /// </summary>
    Artifact Clone();
}
