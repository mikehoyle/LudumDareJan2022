using UnityEngine;

namespace Game.Player {
  public interface ITrainComponent {
    Rigidbody2D Rigidbody { get; }
    Transform BottomAnchor { get; }
    
    Quaternion Rotation { get; }
  }
}
