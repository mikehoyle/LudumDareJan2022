using UnityEditor;
using UnityEngine;

namespace Game {
  public class RequestsDisplay : MonoBehaviour {
    private const string RequestContainer = "Assets/Prefabs/RequestContainer.prefab";
    private const float _firstIndicatorX = 200;
    private const float _indicatorInterval = 70;
    
    private GameObject _farmerPrefab;
    
    void Start() {
      _farmerPrefab = PrefabUtility.LoadPrefabContents(RequestContainer);
    }

    void Update() {
      // TODO this is only partially done, pick up here
    }
  }
}
