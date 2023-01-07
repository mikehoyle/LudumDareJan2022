using Game.Player;
using UnityEngine;

namespace Game {
  public class GameController : MonoBehaviour {
    [SerializeField] private GameObject boxcarPrefab;

    public BoxcarController CreateBoxcar(ITrainComponent parent) {
      var prefab = Instantiate(boxcarPrefab) as GameObject;
      var boxcar = prefab.GetComponent<BoxcarController>();
      boxcar.transform.position = parent.BottomAnchor;
      boxcar.transform.rotation = parent.Rotation;
      boxcar.Hinge.connectedBody = parent.Rigidbody;
      boxcar.Hinge.connectedAnchor = parent.BottomAnchor;
      boxcar.FrictionJoint.connectedBody = parent.Rigidbody;
      boxcar.FrictionJoint.connectedAnchor = parent.BottomAnchor;
      return boxcar;
    }
  }
}
