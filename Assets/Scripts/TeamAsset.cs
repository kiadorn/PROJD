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
    public Color AbilityCDBackground;
    public Color AbilityCDBackgroundInactive;
    [Header("Decoy")]
    public Sprite AbilityDecoy;
    public Sprite AbilityDecoyCD;
    public Sprite AbilityDecoyInactive;
    [Header("Dash")]
    public Sprite AbilityDash;
    public Sprite AbilityDashCD;
    public Sprite AbilityDashInactive;
    [Header("Shoot")]
    public Sprite AbilityShoot;
    public Sprite AbilityShootCD;
    public Sprite AbilityShootInactive;
    public Sprite AbilityChargeShoot;
    public Sprite Crosshair;
    [Header("Minimap")]
    public Sprite MinimapBorder;
}
