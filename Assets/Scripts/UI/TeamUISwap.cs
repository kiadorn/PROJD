using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamUISwap : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI[] _texts;
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
    [SerializeField] private Image _abilityShootCharge2Icon;
    [SerializeField] private Image _crosshair;
    [Header("Minimap")]
    [SerializeField] private Image _minimapBorder;
    

    public void SwapUIImages(TeamAsset asset)
    {
        _abilityBackgroundIcon.sprite = asset.AbilityBackground;
        _abilityDecoyIcon.sprite = asset.AbilityDecoy;  _abilityDecoyCDIcon.sprite = asset.AbilityDecoy;    _abilityDecoyIcon.color = asset.AbilityCDBackground;  _abilityDecoyInactiveIcon.sprite = asset.AbilityDecoy;  _abilityDecoyInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityDashIcon.sprite = asset.AbilityDash;    _abilityDashCDIcon.sprite = asset.AbilityDash;      _abilityDashIcon.color = asset.AbilityCDBackground;   _abilityDashInactiveIcon.sprite = asset.AbilityDash;    _abilityDashInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityShootIcon.sprite = asset.AbilityShoot;  _abilityShootCDIcon.sprite = asset.AbilityShoot;    _abilityShootIcon.color = asset.AbilityCDBackground;  _abilityShootInactiveIcon.sprite = asset.AbilityShoot;  _abilityShootInactiveIcon.color = asset.AbilityCDBackgroundInactive;
        _abilityShootChargeIcon.sprite = asset.AbilityChargeShoot; _abilityShootCharge2Icon.sprite = asset.AbilityChargeShoot;
        _crosshair.sprite = asset.Crosshair;
        _minimapBorder.sprite = asset.MinimapBorder;
        foreach (TextMeshProUGUI text in _texts)
        {
            text.color = asset.BodyColor;
        }
    }
}
