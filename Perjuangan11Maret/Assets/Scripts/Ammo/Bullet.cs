using UnityEngine;
using Lean.Pool;
using System.Linq;

public class Bullet : MonoBehaviour
{
    public Transform agentTransform;
    public BulletState bulletState;
    [SerializeField] private float direction = 1;
    public float traverseSpeed = 100f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float lifeTimer = 0f;
    public float bulletLifetime { get => lifetime; set => lifetime = value; }
    public CircleCollider2D bulletCollider;

    private void Awake()
    {
        Physics2D.IgnoreCollision(bulletCollider, agentTransform.GetComponent<Collider2D>(), true);
    }

    private void Start()
    {
        lifeTimer = bulletLifetime;
    }
    // Update is called once per frame
    void Update()
    {
        Traverse(direction, traverseSpeed);
    }

    public void SetDirection(float agentDirection)
    {
        direction = agentDirection;
    }

    public void Traverse(float direction, float speed)
    {
        bulletState = BulletState.Alive;
        transform.Translate(Vector3.right * Time.deltaTime * direction * traverseSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.name == "FireRange")
            {
                LeanPool.Despawn(gameObject);
            }
        }
    }

    public enum BulletState
    {
        Alive, Dead
    }
}
