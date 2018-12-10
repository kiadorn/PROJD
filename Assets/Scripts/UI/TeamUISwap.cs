using UnityEngine;
using UnityEngine.UI;

public class TeamUISwap : MonoBehaviour {

    [SerializeField] private Image _abilityBackgroundIcon;
    [SerializeField] private Image _abilityDecoyIcon;
    [SerializeField] private GameObject _abilityDecoyCDIcon;
    [SerializeField] private GameObject _abilityDecoyInactiveIcon;
    [SerializeField] private Image _abilityDashIcon;
    [SerializeField] private GameObject _abilityDashCDIcon;
    [SerializeField] private GameObject _abilityDashInactiveIcon;
    [SerializeField] private Image _abilityShootIcon;
    [SerializeField] private GameObject _abilityShootCDIcon;
    [SerializeField] private GameObject _abilityShootInactiveIcon;
    [SerializeField] private Image _abilityShootChargeIcon;
    [SerializeField] private Image _crosshair;
    [SerializeField] private Image _minimapBorder;
    

    public void SwapUIImages(TeamAsset asset)
    {
        _abilityBackgroundIcon.sprite = asset.AbilityBackground;
        _abilityDecoyIcon.sprite = asset.AbilityDecoy;  _abilityDecoyCDIcon = asset.AbilityDecoyCD;   _abilityDecoyInactiveIcon = asset.AbilityDecoyInactive;
        _abilityDashIcon.sprite = asset.AbilityDash;    _abilityDashCDIcon = asset.AbilityDashCD;     _abilityDashInactiveIcon = asset.AbilityDashInactive;
        _abilityShootIcon.sprite = asset.AbilityShoot;  _abilityShootCDIcon = asset.AbilityShootCD;   _abilityShootInactiveIcon = asset.AbilityShootInactive;
        _abilityShootChargeIcon.sprite = asset.AbilityChargeShoot;
        _crosshair.sprite = asset.Crosshair;
        _minimapBorder.sprite = asset.MinimapBorder;
    }
}
