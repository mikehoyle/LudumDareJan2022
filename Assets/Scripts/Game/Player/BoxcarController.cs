using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player {
  public class BoxcarController : MonoBehaviour, ITrainComponent {
    [SerializeField] private int fullLoadSize;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color partialColor;
    [SerializeField] private Color fullColor;
    
    private Transform _bottomAnchor;
    private HingeJoint2D _hinge;
    
    public Transform BottomAnchor => _bottomAnchor;
    public Quaternion Rotation => transform.rotation;
    public Rigidbody2D Rigidbody { get; private set; }

    public CropType Contents { get; private set; } = CropType.None;
    public uint ContentsCount { get; private set; } = 0;

    public Text ContentsIndicatorUI { get; set; }

    private void Awake() {
      _bottomAnchor = transform.Find("AnchorBottom");
      Rigidbody = GetComponent<Rigidbody2D>();
      _hinge = GetComponent<HingeJoint2D>();

      GetComponentInChildren<Canvas>().worldCamera = Camera.main;
      ContentsIndicatorUI = GetComponentInChildren<Text>();

      // This is at the very front, due to sprite pivot placement
      Rigidbody.centerOfMass = Vector2.zero;
    }

    private void Update() {
      ContentsIndicatorUI.text = $"{ContentsCount}/{fullLoadSize}";
      if (ContentsCount == 0) {
        ContentsIndicatorUI.color = emptyColor;
      } else if (ContentsCount < fullLoadSize) {
        ContentsIndicatorUI.color = partialColor;
      } else {
        ContentsIndicatorUI.color = fullColor;
      }
    }

    public void AttachTo(ITrainComponent parent) {
      Rigidbody.velocity = Vector2.zero;
      Rigidbody.angularVelocity = 0f;
      var targetPosition = parent.BottomAnchor.position;
      transform.position = targetPosition;
      Rigidbody.MovePosition(targetPosition);
      Rigidbody.rotation = parent.Rigidbody.rotation;

      _hinge.connectedBody = parent.Rigidbody;
      _hinge.connectedAnchor = parent.BottomAnchor.localPosition;
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
  }
}
