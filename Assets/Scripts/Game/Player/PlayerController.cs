using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game {
  public class PlayerController : MonoBehaviour, ITrainComponent {
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnRadius;
    [SerializeField] private float acceleration;
    [SerializeField] private float reverseAcceleration;
    [SerializeField] private GameObject boxcarPrefab;
    
    private Transform _bottomAnchor;
    private GameController _gameController;
    private List<BoxcarController> _childCars = new();

    public Rigidbody2D Rigidbody { get; private set; }
    public Transform BottomAnchor => _bottomAnchor;
    public Quaternion Rotation => transform.rotation;

    void Start() {
      Rigidbody = GetComponent<Rigidbody2D>();
      _bottomAnchor = transform.Find("AnchorBottom");
      _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    void Update() {
      HandleInput();
      MaybeGatherResource();
    }

    private void MaybeGatherResource() {
      var cropType = _gameController.GetCropAtWorldPosition(transform.position);
      if (cropType != CropType.None) {
        // Attempt to collect crop
        foreach (var car in _childCars) {
          if (car.TryCollectCrop(cropType)) {
            _gameController.CollectCropAtWorldPosition(transform.position, cropType);
            return;
          }
        }
      }
    }

    private void HandleInput() {
      var horizontalAxis = Input.GetAxis("Horizontal");
      var verticalAxis = Input.GetAxis("Vertical");
      if (horizontalAxis is > 0.1f or < -0.1f) {
        // Turning
        var direction = horizontalAxis > 0 ? -1 : 1;
        Rigidbody.angularVelocity =
            Math.Max(maxTurnRadius, turnSpeed * Time.deltaTime) * direction;
      } else {
        Rigidbody.angularVelocity = 0;
      }
      
      if (verticalAxis is > 0.1f or < -0.1f) {
        // Acceleration
        var direction = verticalAxis > 0 ? 1 : -1;
        var speed = direction * acceleration * Time.deltaTime;
        Rigidbody.AddRelativeForce(Vector2.up * speed);
      }

      if (Input.GetKeyDown(KeyCode.E)) {
        // TODO determine if there's a boxcar nearby to pickup.
        CreateBoxcar();
      }
    }

    private void CreateBoxcar() {
      var prefab = Instantiate(boxcarPrefab);
      var boxcar = prefab.GetComponent<BoxcarController>();
      if (_childCars.Count == 0) {
        boxcar.AttachTo(this);        
      } else {
        boxcar.AttachTo(_childCars.Last());
      }
      _childCars.Add(boxcar);
    }
  }
}
