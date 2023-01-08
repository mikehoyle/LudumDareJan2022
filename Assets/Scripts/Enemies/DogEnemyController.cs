using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogEnemyController : MonoBehaviour
{
  public Transform target;
  public float speed = 3f;
  public float rotateSpeed = 0.0025f;
  private Rigidbody2D rb;

  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();

  }

  // Update is called once per frame
  void Update()
  {
    if (!target)
    {
      GetTarget();
    } else
    {
      RotateTowardsTarget();
    }
  }

  void FixedUpdate()
  {
    rb.velocity = transform.up * speed;
  }

  private void RotateTowardsTarget()
  {
    Vector2 targetDirection = target.position - transform.position;
    float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
    Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
    transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
  }

  private void GetTarget()
  {
    target = GameObject.FindGameObjectWithTag("Player").transform;
  }
}
