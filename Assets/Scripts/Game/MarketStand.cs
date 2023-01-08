using UnityEngine;
using UnityEngine.UI;

namespace Game {
  public class MarketStand : MonoBehaviour {
    private Text _text;
    private SpriteRenderer _icon;
    
    [SerializeField] private Sprite wheatIcon;
    [SerializeField] private Sprite grapeIcon;
    [SerializeField] private Sprite tomatoIcon;
    [SerializeField] private Sprite pumpkinIcon;
    [SerializeField] private Sprite cornIcon;

    private void Start() {
      _text = GetComponentInChildren<Text>();
      _icon = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    public void UpdateVisuals(CropType requestedCrop, float remainingTime) {
      if (requestedCrop == CropType.None) {
        _text.text = "";
        _icon.enabled = false;
        return;
      }

      _icon.enabled = true;
      var timeRemainingFmt =
          $"{(int)remainingTime / 60}:{((int)remainingTime % 60):00}";
      _text.text = timeRemainingFmt;
      _icon.sprite = GetSpriteForType(requestedCrop);
    }

    private Sprite GetSpriteForType(CropType cropType) {
      return cropType switch {
          CropType.Wheat => wheatIcon,
          CropType.Grape => grapeIcon,
          CropType.Tomato => tomatoIcon,
          CropType.Pumpkins => pumpkinIcon,
          CropType.Corn => cornIcon,
          _ => cornIcon,
      };
    }
  }
}
