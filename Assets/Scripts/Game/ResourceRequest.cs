using UnityEngine;

namespace Game {
  /// <summary>
  /// Represents an outstanding request for a resource
  /// </summary>
  public class ResourceRequest {
    public int RequestingMarketStandIndex { get; }
    
    public float TimeRemainingSecs { get; private set; }

    public CropType RequestedResource { get; }

    public ResourceRequest(int requesterIndex, float duration, CropType crop) {
      RequestingMarketStandIndex = requesterIndex;
      TimeRemainingSecs = duration;
      RequestedResource = crop;
    }

    public void Update() {
      TimeRemainingSecs -= Time.deltaTime;
    }
  }
}
