using UnityEngine;

namespace Game.Player {
  public interface ITrainComponent {
    Rigidbody2D Rigidbody { get; }
    Vector2 BottomAnchor { get; }
    
    Quaternion Rotation { get; }
  }
}
