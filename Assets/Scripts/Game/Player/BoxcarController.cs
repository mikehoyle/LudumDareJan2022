using UnityEngine;

namespace Game.Player {
  public class BoxcarController : MonoBehaviour, ITrainComponent {
    private Transform _bottomAnchor;
    private HingeJoint2D _hinge;
    
    public Transform BottomAnchor => _bottomAnchor;
    public Quaternion Rotation => transform.rotation;
    public Rigidbody2D Rigidbody { get; private set; }

    private void Awake() {
      _bottomAnchor = transform.Find("AnchorBottom");
      Rigidbody = GetComponent<Rigidbody2D>();
      _hinge = GetComponent<HingeJoint2D>();

      // This is at the very front, due to sprite pivot placement
      Rigidbody.centerOfMass = Vector2.zero;
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
  }
}
