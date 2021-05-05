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
    
    [Tooltip("probability of a person spawning being impfgegner")]
    public float probImpfgegner;

    [Tooltip("1: Easy, 2: Medium, 3: Hard, 4: Impossible")]
    public int difficultLevel;
   
    public int bossLives;

    public int impfDosenVonPolitikern;

    public int koBulletsVonPolitikern;

    public int raketenVonPolitiker;

    public float timeBetweenHusten=3f;
    public int getNumberOfImpfbare() {
        return (int) (((float) size * size) * personDensity);
    }
    public void changeSettingsAtStart(int schwierigkeit)
    {
        this.difficultLevel=schwierigkeit;
        if(schwierigkeit==1)
        {
            probInfected=0.1f;
            probImpfgegner=0.05f;
            bossLives=40;
            setBulletsVonPolitikern(20,20,20);  
            timeBetweenHusten=4f;
        }
        if(schwierigkeit==2)
        {
            probInfected=0.2f;
            probImpfgegner=0.1f;
            bossLives=60;
            setBulletsVonPolitikern(20,10,10);
            timeBetweenHusten=3f;
        }
        if(schwierigkeit==3)
        {
            probInfected=0.3f;
            probImpfgegner=0.2f;
            bossLives=100;
            setBulletsVonPolitikern(15,5,5);
            timeBetweenHusten=2f;
        }
        if(schwierigkeit==4)
        {
            probInfected=0.5f;
            probImpfgegner=0.5f;
            bossLives=200;
            setBulletsVonPolitikern(10,2,5);
            timeBetweenHusten=1f;
        }
    }
    private void setBulletsVonPolitikern(int impfDosen, int koBullets, int raketen)
    {
        impfDosenVonPolitikern=impfDosen;
        koBulletsVonPolitikern=koBullets;
        raketenVonPolitiker=raketen;
    }
}