using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicCrushingObstacles : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private Transform blockA;
    [SerializeField] private Transform blockB;
 
    [Header("Movement")]
    [SerializeField] private float openDistance = 3f;    // how far each block sits from center when open
    [SerializeField] private float closeSpeed = 12f;     // fast slam
    [SerializeField] private float openSpeed = 2f;       // slow reopen
 
    [Header("Timing")]
    [SerializeField] private float openWaitTime = 1.5f;  // time the gap stays open
    [SerializeField] private float closedWaitTime = 0.5f;
 
    private float currentOffset;      // distance of each block from center
    private bool isDangerous;         // true while the blocks are closing/closed
 
    private void Start()
    {
        currentOffset = openDistance;
        ApplyPositions();
        StartCoroutine(CrushCycle());
    }
 
    private IEnumerator CrushCycle()
    {
        while (true)
        {
            // Gap is open: player's chance to run through
            isDangerous = false;
            yield return new WaitForSeconds(openWaitTime);
 
            // Slam shut
            isDangerous = true;
            while (currentOffset > 0f)
            {
                currentOffset = Mathf.MoveTowards(currentOffset, 0f, closeSpeed * Time.deltaTime);
                ApplyPositions();
                yield return null;
            }
 
            // Hold closed
            yield return new WaitForSeconds(closedWaitTime);
 
            // Slowly open
            while (currentOffset < openDistance)
            {
                currentOffset = Mathf.MoveTowards(currentOffset, openDistance, openSpeed * Time.deltaTime);
                ApplyPositions();
                yield return null;
            }
        }
    }
 
    private void ApplyPositions()
    {
        blockA.localPosition = new Vector3(-currentOffset, blockA.localPosition.y, blockA.localPosition.z);
        blockB.localPosition = new Vector3(currentOffset, blockB.localPosition.y, blockB.localPosition.z);
    }
 
    private void OnTriggerStay(Collider other)
    {
        // Only crush when the blocks are closing/closed and nearly touching
        if (isDangerous && other.CompareTag("Player") && currentOffset < 0.6f)
        {
            Crush();
        }
    }
 
    private void Crush()
    {
        Debug.Log("Player crushed!");
    }
}
