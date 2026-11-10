using UnityEngine;

public class BasicObstacleBehavior : MonoBehaviour
{
   [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 4f;
 
    [Header("Boundaries (distance from start position)")]
    [SerializeField] private float maxRangeX = 6f;   // left / right limit
    [SerializeField] private float maxRangeZ = 6f;   // forward / backward limit
 
    [Header("Timing")]
    [SerializeField] private float minPauseTime = 0.2f;
    [SerializeField] private float maxPauseTime = 1f;
 
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float pauseTimer;
 
    // The four possible directions in world space
    private static readonly Vector3[] Directions =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };
 
    private void Start()
    {
        startPosition = transform.position;
        PickNewTarget();
    }
 
    private void Update()
    {
        // Wait at the target for a short random time
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }
 
        // Move toward the target
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, speed * Time.deltaTime);
 
        // Arrived: pause, then choose a new random direction
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            pauseTimer = Random.Range(minPauseTime, maxPauseTime);
            PickNewTarget();
        }
    }
 
    private void PickNewTarget()
    {
        Vector3 direction = Directions[Random.Range(0, Directions.Length)];
        float distance = Random.Range(minDistance, maxDistance);
 
        Vector3 newTarget = transform.position + direction * distance;
 
        // Keep the platform inside its allowed area
        newTarget.x = Mathf.Clamp(newTarget.x, startPosition.x - maxRangeX, startPosition.x + maxRangeX);
        newTarget.z = Mathf.Clamp(newTarget.z, startPosition.z - maxRangeZ, startPosition.z + maxRangeZ);
        newTarget.y = startPosition.y; // stay at the same height
 
        targetPosition = newTarget;
    }
 
    // Draws the movement area in the Scene view when the platform is selected
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(maxRangeX * 2f, 0.1f, maxRangeZ * 2f));
    }
}
