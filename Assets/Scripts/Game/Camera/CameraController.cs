using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Transform target;
  public float smoothing;
  
  void Start() {
    var mainCamera = GetComponent<Camera>();
    
    // Sort front-to-back for proper layering.
    mainCamera.transparencySortMode = TransparencySortMode.CustomAxis;
    mainCamera.transparencySortAxis = Vector3.up;
  }

  void LateUpdate() {
    if (transform.position != target.position)
    {
      Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

      transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
    }
  }
}
