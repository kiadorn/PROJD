using UnityEngine;
using UnityEngine.UI;

public class TeamUISwap : MonoBehaviour {

    [SerializeField] private Image _abilityBackgroundIcon;
    [Header("Decoy")]
    [SerializeField] private Image _abilityDecoyIcon;
    [SerializeField] private Image _abilityDecoyCDIcon;
    [SerializeField] private Image _abilityDecoyInactiveIcon;
    [Header("Dash")]
    [SerializeField] private Image _abilityDashIcon;
    [SerializeField] private Image _abilityDashCDIcon;
    [SerializeField] private Image _abilityDashInactiveIcon;
    [Header("Shoot")]
    [SerializeField] private Image _abilityShootIcon;
    [SerializeField] private Image _abilityShootCDIcon;
    [SerializeField] private Image _abilityShootInactiveIcon;
    [SerializeField] private Image _abilityShootChargeIcon;
    [SerializeField] private Image _crosshair;
    [Header("Minimap")]
    [SerializeField] private Image _minimapBorder;
    

    public void SwapUIImages(TeamAsset asset)
    {
        _abilityBackgroundIcon.sprite = asset.AbilityBackground;
        _abilityDecoyIcon.sprite = asset.AbilityDecoy;  _abilityDecoyCDIcon.sprite = asset.AbilityDecoyCD;  _abilityDecoyCDIcon.color = asset.AbilityCDBackground;  _abilityDecoyInactiveIcon.sprite = asset.AbilityDecoyInactive;  _abilityDecoyInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityDashIcon.sprite = asset.AbilityDash;    _abilityDashCDIcon.sprite = asset.AbilityDashCD;    _abilityDashCDIcon.color = asset.AbilityCDBackground;   _abilityDashInactiveIcon.sprite = asset.AbilityDashInactive;    _abilityDashInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityShootIcon.sprite = asset.AbilityShoot;  _abilityShootCDIcon.sprite = asset.AbilityShootCD;  _abilityDecoyCDIcon.color = asset.AbilityCDBackground;  _abilityShootInactiveIcon.sprite = asset.AbilityShootInactive;  _abilityShootInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityShootChargeIcon.sprite = asset.AbilityChargeShoot;
        _crosshair.sprite = asset.Crosshair;
        _minimapBorder.sprite = asset.MinimapBorder;
    }
}
