using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TeamAsset", menuName = "ScriptableObjects/TeamAsset")]
public class TeamAsset : ScriptableObject
{
    public string TeamName;
    public Color BodyColor;
    public Color MaskColor;
    public Color ThirdPersonOutlineColor;
    public Gradient ParticleGradient;
    public Gradient LaserGradient;

    [Header("Team Specific UI")]
    public Sprite AbilityBackground;
    public Sprite AbilityDecoy;
    public Sprite AbilityDash;
    public Sprite AbilityShoot;
    public Sprite AbilityChargeShoot;
    public Sprite Crosshair;
    public Sprite MinimapBorder;
}
