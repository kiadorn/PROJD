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
    public Gradient SwirlyGradient;
    public Gradient LaserGradient;

    [Header("Team Specific UI")]
    public Sprite AbilityBackground;
    public Color AbilityCDBackground;
    public Color AbilityCDBackgroundInactive;
    [Header("Decoy")]
    public Sprite AbilityDecoy;
    [Header("Dash")]
    public Sprite AbilityDash;
    [Header("Shoot")]
    public Sprite AbilityShoot;
    public Sprite AbilityChargeShoot;
    public Sprite Crosshair;
    [Header("Minimap")]
    public Sprite MinimapBorder;
}
