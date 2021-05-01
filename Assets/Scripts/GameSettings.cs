using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game's settings.
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    /// <summary>
    /// Loads the game settings from the Resources directory.
    /// </summary>
    /// <returns></returns>
    public static GameSettings Load() => Resources.Load<GameSettings>("Settings");

    [Header("Labyrinth")]
    [Tooltip("The labyrinth's side length")]
    public int size;
    [Tooltip("The labyrinth's cells' side length")]
    public int cellSize;
    [Tooltip("Length of the path to the exit of the labyrinth relative to it's side length")]
    public int pathLengthRelativeToSize;
    [Tooltip("Thickness of the labyrinth's walls")]
    public float wallThickness;
    
    [Header("Gameplay")]
    [Tooltip("The number of persons spawned per cell")]
    public float personDensity;
    [Tooltip("probability of a person spawning infected")]
    public float probInfected;
    [Tooltip("number of Impfgegner spawning per attack")]
    public float numberOfImpfgegnerPerAttack;

    public int getNumberOfImpfbare() {
        return (int) (((float) size * size) * personDensity);
    }
}