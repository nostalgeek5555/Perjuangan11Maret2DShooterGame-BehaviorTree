using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject actorFollowedByCamera;
    public GameObject levelMapFloor;

    [SerializeField]
    private float timeOffset;

    [SerializeField]
    private Vector2 posOffset;

    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private float topLimit;
    [SerializeField] private float bottomLimit;
    private Vector3 cameraVelocity;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D mapFloorCollider = levelMapFloor.GetComponent<BoxCollider2D>();
        rightLimit = mapFloorCollider.size.x - 20;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startPos = transform.position;

        Vector3 endPos = actorFollowedByCamera.transform.position;

        endPos.x += posOffset.x;
        endPos.y += posOffset.y;
        endPos.z = -10;

        transform.position = Vector3.Lerp(startPos, endPos, timeOffset * Time.deltaTime);

        transform.position = new Vector3(
           Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
           Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
           transform.position.z
        );
    }
}
