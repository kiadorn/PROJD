using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamAsset", menuName = "ScriptableObjects/TeamAsset")]
public class TeamAsset : ScriptableObject
{
    public Color bodyColor;
    public Color maskColor;
    public float colorLimit;
    public Color invisibilityParticleColor;
    public Gradient particleGradient;
    public Gradient laserGradient;
}
