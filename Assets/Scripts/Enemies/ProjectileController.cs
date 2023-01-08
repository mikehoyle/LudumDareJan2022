using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
  public float speed;

  private Transform player;
  private Vector2 target;

  // Start is called before the first frame update
  void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player").transform;

    target = new Vector2(player.position.x, player.position.y);
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

    if(transform.position.x == target.x && transform.position.y == target.y)
    {
      DestroyProjectile();
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      DestroyProjectile();
    }
  }

  void DestroyProjectile()
  {
    Destroy(gameObject);
  }
}
