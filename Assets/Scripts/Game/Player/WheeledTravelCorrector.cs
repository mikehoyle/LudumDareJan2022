using UnityEngine;

namespace Game.Player {
  public static class WheeledTravelCorrector {
    private const float CorrectionScale = 1f;

    public static void CorrectDrift(Rigidbody2D rigidbody2D) {
      var currentVelocity = rigidbody2D.velocity;
      var angleOfTravel = Vector2.Angle(Vector2.zero, currentVelocity);
      
      // TODO: Correct to straight faster, the faster we're going
      //rigidbody2D.angularVelocity -=
      //(angleOfTravel - rigidbody2D.transform.eulerAngles.z) * CorrectionScale;

      if (angleOfTravel > rigidbody2D.transform.eulerAngles.z) {
        rigidbody2D.AddTorque(CorrectionScale);
      } else {
        rigidbody2D.AddTorque(CorrectionScale * -1);
      }
      
    }
  }
}
