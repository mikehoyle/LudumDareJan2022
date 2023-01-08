using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
  public class RequestsDisplay : MonoBehaviour {

    [SerializeField] private Texture wheatIcon;
    [SerializeField] private Texture grapeIcon;
    [SerializeField] private Texture tomatoIcon;
    [SerializeField] private Texture pumpkinIcon;
    [SerializeField] private Texture cornIcon;
    
    private GameController _gameController;
    private List<(GameObject, RawImage, Text)> _requestContainers;

    void Start() {
      _requestContainers = new();
      var displayPanels = GameObject.FindGameObjectsWithTag("RequestContainer");
      foreach (var container in displayPanels) {
        container.SetActive(false);
        var image = container.GetComponentInChildren<RawImage>();
        var text = container.GetComponentInChildren<Text>();
        _requestContainers.Add((container, image, text));
      }
      _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    void Update() {
      var currentContainer = 0;
      foreach (var request in _gameController.OutstandingRequests) {
        if (request != null) {
          _requestContainers[currentContainer].Item1.SetActive(true);
          _requestContainers[currentContainer].Item2.texture =
              GetSpriteForType(request.RequestedResource);
          var timeRemainingFmt =
              $"{(int)request.TimeRemainingSecs / 60}:{((int)request.TimeRemainingSecs % 60):00}";
          _requestContainers[currentContainer].Item3.text =
              timeRemainingFmt;
          currentContainer++;
        }
      }

      for (; currentContainer < _requestContainers.Count; currentContainer++) {
        _requestContainers[currentContainer].Item1.SetActive(false);
      }
    }

    private Texture GetSpriteForType(CropType cropType) {
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
