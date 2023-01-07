using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Player {
  public class BoxcarController : MonoBehaviour, ITrainComponent {
    private Transform _bottomAnchor;
    
    public Vector2 BottomAnchor => _bottomAnchor.position;
    public Quaternion Rotation => transform.rotation;
    public Rigidbody2D Rigidbody { get; private set; }
    public HingeJoint2D Hinge { get; private set; }

    private void Awake() {
      _bottomAnchor = transform.Find("AnchorBottom");
      Rigidbody = GetComponent<Rigidbody2D>();
      Hinge = GetComponent<HingeJoint2D>();
    }
  }
}
