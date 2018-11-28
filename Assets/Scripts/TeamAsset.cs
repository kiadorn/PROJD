using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamAsset", menuName = "ScriptableObjects/TeamAsset")]
public class TeamAsset : ScriptableObject
{
    public string teamName;
    public Color bodyColor;
    public Color maskColor;
    public Color thirdPersonOutlineColor;
    public float colorLimit;
    public Color invisibilityParticleColor;
    public Gradient particleGradient;
    public Gradient laserGradient;

}
