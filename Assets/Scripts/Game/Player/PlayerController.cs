using System;
using UnityEngine;

namespace Game {
  public class PlayerController : MonoBehaviour {
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnRadius;
    [SerializeField] private float acceleration = 2f;
    
    private Rigidbody2D _rigidBody;

    void Start() {
      _rigidBody = GetComponent<Rigidbody2D>();
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
        _rigidBody.angularVelocity =
            Math.Max(maxTurnRadius, turnSpeed * Time.fixedDeltaTime) * direction;
      } else {
        _rigidBody.angularVelocity = 0;
      }
      
      if (verticalAxis is > 0.1f or < -0.1f) {
        // Acceleration
        var direction = verticalAxis > 0 ? 1 : -1;
        var speed = direction * acceleration * Time.fixedDeltaTime;
        _rigidBody.AddRelativeForce(Vector2.up * speed);
      }
    }
  }
}
