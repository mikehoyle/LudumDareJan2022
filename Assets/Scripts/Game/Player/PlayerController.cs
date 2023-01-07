using System;
using Game.Player;
using UnityEngine;

namespace Game {
  public class PlayerController : MonoBehaviour, ITrainComponent {
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnRadius;
    [SerializeField] private float acceleration = 2f;
    private Transform _bottomAnchor;
    private GameController _gameController;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 BottomAnchor => _bottomAnchor.position;
    public Quaternion Rotation => transform.rotation;

    void Start() {
      Rigidbody = GetComponent<Rigidbody2D>();
      _bottomAnchor = transform.Find("AnchorBottom");
      _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

      // DO NOT SUBMIT for testing only
      var boxcar = _gameController.CreateBoxcar(this);
      _gameController.CreateBoxcar(boxcar);
    }

    void FixedUpdate() {
      HandleInput();
    }

    private void HandleInput() {
      var horizontalAxis = Input.GetAxis("Horizontal");
      var verticalAxis = Input.GetAxis("Vertical");
      if (horizontalAxis is > 0.1f or < -0.1f) {
        // Turning
        var direction = horizontalAxis > 0 ? -1 : 1;
        Rigidbody.angularVelocity =
            Math.Max(maxTurnRadius, turnSpeed * Time.fixedDeltaTime) * direction;
      } else {
        Rigidbody.angularVelocity = 0;
      }
      
      if (verticalAxis is > 0.1f or < -0.1f) {
        // Acceleration
        var direction = verticalAxis > 0 ? 1 : -1;
        var speed = direction * acceleration * Time.fixedDeltaTime;
        Rigidbody.AddRelativeForce(Vector2.up * speed);
      }
    }
  }
}
