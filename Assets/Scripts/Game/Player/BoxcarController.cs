using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player {
  public class BoxcarController : MonoBehaviour, ITrainComponent {
    [SerializeField] private int fullLoadSize;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color partialColor;
    [SerializeField] private Color fullColor;
    
    // Sprites
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite cornSprite;
    [SerializeField] private Sprite wheatSprite;
    [SerializeField] private Sprite pumpkinSprite;
    [SerializeField] private Sprite tomatoSprite;
    [SerializeField] private Sprite grapesSprite;

    private HingeJoint2D _hinge;
    private SpriteRenderer _spriteRenderer;
    private DistanceJoint2D _distanceJoint;

    public Transform BottomAnchor { get; private set; }

    public Quaternion Rotation => transform.rotation;
    public Rigidbody2D Rigidbody { get; private set; }

    public CropType Contents { get; set; } = CropType.None;
    public uint ContentsCount { get; set; } = 0;

    public Text ContentsIndicatorUI { get; set; }

    public bool IsUnattached => _hinge.enabled == false;
    public bool IsFull => ContentsCount == fullLoadSize;

    private void Awake() {
      BottomAnchor = transform.Find("AnchorBottom");
      Rigidbody = GetComponent<Rigidbody2D>();
      _hinge = GetComponent<HingeJoint2D>();
      _distanceJoint = GetComponent<DistanceJoint2D>();
      _spriteRenderer = GetComponent<SpriteRenderer>();

      GetComponentInChildren<Canvas>().worldCamera = Camera.main;
      ContentsIndicatorUI = GetComponentInChildren<Text>();

      // This is at the very front, due to sprite pivot placement
      Rigidbody.centerOfMass = Vector2.zero;
    }

    private void Update() {
      if (IsUnattached) {
        ContentsIndicatorUI.text = "[E] PICK\nME UP";
        ContentsIndicatorUI.color = partialColor;
        Rigidbody.bodyType = RigidbodyType2D.Static;
        return;
      }
      
      ContentsIndicatorUI.text = $"{ContentsCount}/{fullLoadSize}";
      if (ContentsCount == 0) {
        ContentsIndicatorUI.color = emptyColor;
      } else if (ContentsCount < fullLoadSize) {
        ContentsIndicatorUI.color = partialColor;
      } else {
        ContentsIndicatorUI.color = fullColor;
      }
      
      UpdateSprite();
    }

    private void UpdateSprite() {
      var sprite = Contents switch {
          CropType.Corn => cornSprite,
          CropType.Pumpkins => pumpkinSprite,
          CropType.Tomato => tomatoSprite,
          CropType.Wheat => wheatSprite,
          CropType.Grape => grapesSprite,
          _ => emptySprite
      };

      _spriteRenderer.sprite = sprite;
    }

    public void AttachTo(ITrainComponent parent) {
      Rigidbody.bodyType = RigidbodyType2D.Dynamic;
      Rigidbody.velocity = Vector2.zero;
      Rigidbody.angularVelocity = 0f;
      var targetPosition = parent.BottomAnchor.position;
      transform.position = targetPosition;
      Rigidbody.MovePosition(targetPosition);
      Rigidbody.rotation = parent.Rigidbody.rotation;
      Rigidbody.MoveRotation(parent.Rigidbody.rotation);

      _hinge.enabled = true;
      _distanceJoint.enabled = true;
      _hinge.connectedBody = parent.Rigidbody;
      _hinge.connectedAnchor = parent.BottomAnchor.localPosition;
      _distanceJoint.connectedBody = parent.Rigidbody;
      _distanceJoint.connectedAnchor = parent.BottomAnchor.localPosition;
      _distanceJoint.distance = 0;
      Physics2D.IgnoreCollision(parent.Rigidbody.GetComponent<Collider2D>(), GetComponent<Collider2D>());
      Physics2D.SyncTransforms();
    }

    /// <summary>
    /// Returns whether the crop was successfully collected.
    /// </summary>
    public bool TryCollectCrop(CropType type) {
      if (Contents == CropType.None) {
        Contents = type;
        ContentsCount = 1;
        return true;
      }

      if (Contents != type || ContentsCount >= fullLoadSize) {
        return false;
      }

      ContentsCount++;
      return true;
    }

    public void Empty() {
      Contents = CropType.None;
      ContentsCount = 0;
    }
  }
}
