using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using UnityEditor.Experimental;
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
    private CircleCollider2D _interactionCollider;
    private ParticleSystem _collectionParticles;

    public Rigidbody2D Rigidbody { get; private set; }
    public Transform BottomAnchor => _bottomAnchor;
    public Quaternion Rotation => transform.rotation;

    void Start() {
      Rigidbody = GetComponent<Rigidbody2D>();
      _bottomAnchor = transform.Find("AnchorBottom");
      _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
      _interactionCollider = GetComponentInChildren<CircleCollider2D>();
      _collectionParticles = GetComponentInChildren<ParticleSystem>();
      
      // Start with one cart
      CreateBoxcar();
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
            _collectionParticles.Play();
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
        var speed = verticalAxis switch {
            > 0 => acceleration,
            _ => reverseAcceleration * -1,
        };
        
        Rigidbody.AddRelativeForce(Vector2.up * speed);
      }

      if (Input.GetKeyDown(KeyCode.E)) {
        // TODO determine if there's a boxcar nearby to pickup.
        var overlappingItems = Physics2D.OverlapCircleAll(
            _interactionCollider.transform.position, _interactionCollider.radius);
        foreach (var collider in overlappingItems) {
          if (collider.CompareTag("Boxcar")
              && collider.gameObject.GetComponent<BoxcarController>().IsUnattached) {
            Destroy(collider.gameObject);
            CreateBoxcar();
          }

          if (collider.CompareTag("MarketStand")) {
            TryDeliverGoods(collider.gameObject);
          }
        }
      }

      if (Input.GetKeyDown(KeyCode.Q)) {
        // Dump items
        (uint count, CropType type) currentContents = (0, CropType.None);
        foreach (var boxcar in _childCars) {
          var nextContents = boxcar.ContentsCount;
          var nextType = boxcar.Contents;
          boxcar.ContentsCount = currentContents.count;
          boxcar.Contents = currentContents.type;
          currentContents = (nextContents, nextType);
        }
      }
    }

    private void TryDeliverGoods(GameObject targetMarket) {
      var request = _gameController.GetRequestForMarketStand(targetMarket);
      if (request != null) {
        foreach (var boxcar in _childCars) {
          if (boxcar.IsFull && boxcar.Contents == request.RequestedResource) {
            _gameController.FulfillRequest(request);
            boxcar.Empty();
            return;
          }
        }
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
