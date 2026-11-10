using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _collider.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropVelocity)
    {
        transform.SetParent(null);

        _rb.isKinematic = false;
        _rb.useGravity = true;
        _collider.enabled = true;
        _rb.linearVelocity = dropVelocity;
    }

}
