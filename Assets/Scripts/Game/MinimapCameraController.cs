using UnityEngine;

namespace Game {
  public class MinimapCameraController : MonoBehaviour {
    private GameObject _player;

    private void Start() {
      _player = GameObject.FindWithTag("Player");
    }

    void Update() {
      transform.position = 
          new Vector3(
              _player.transform.position.x,
              _player.transform.position.y,
              -10);
    }
  }
}
