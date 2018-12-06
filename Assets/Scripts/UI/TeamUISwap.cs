using UnityEngine;
using UnityEngine.UI;

public class TeamUISwap : MonoBehaviour {

    [SerializeField] private Image _abilityBackgroundIcon;
    [SerializeField] private Image _abilityDecoyIcon;
    [SerializeField] private Image _abilityDashIcon;
    [SerializeField] private Image _abilityShootIcon;
    [SerializeField] private Image _abilityShootChargeIcon;
    [SerializeField] private Image _crosshair;
    [SerializeField] private Image _minimapBorder;

    public void SwapUIImages(TeamAsset asset)
    {
        _abilityBackgroundIcon.sprite = asset.AbilityBackground;
        _abilityDecoyIcon.sprite = asset.AbilityDecoy;
        _abilityDashIcon.sprite = asset.AbilityDash;
        _abilityShootIcon.sprite = asset.AbilityShoot;
        _abilityShootChargeIcon.sprite = asset.AbilityChargeShoot;
        _crosshair.sprite = asset.Crosshair;
        _minimapBorder.sprite = asset.MinimapBorder;
    }
}
