using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField]
    private PlayerInputManager _inputManager;
    [SerializeField]
    private Animator _animator;

    [Header("Player Interact Settings")]
    [SerializeField]
    private Transform _interactPoint;
    [SerializeField]
    private float _interactRadius;
    [SerializeField]
    private float _throwForce;
    [SerializeField]
    private LayerMask _interactableLayer;

    [Header("Attack Settings")]
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private float _attackRadius = 1.0f;
    [SerializeField]
    private float _attackCooldown = 0.5f;
    [SerializeField]
    private float _knockbackForce = 5.0f;
    [SerializeField]
    private LayerMask _attackHitLayer;

    [Header("Meow Settings")]
    [SerializeField]
    private AudioClip[] _meowSFX;
    [SerializeField]
    private float _minPitch = 0.9f;
    [SerializeField]
    private float _maxPitch = 1f;
    [SerializeField]
    private float meowCooldown = 0.5f;

    //Internal States
    private AudioSource _audioSource;
    private PlayerPickup _currentlyHeldItem;
    private float _nextAttackTime;
    private float _nextMeowTime;

    private readonly Collider[] _hitBuffer = new Collider[10];

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_inputManager == null)
        {
            _inputManager = GetComponent<PlayerInputManager>();
        }
    }

    private void OnEnable()
    {
        _inputManager.OnInteractButtonPressed += PlayerInteract;
        _inputManager.OnAttackButtonPressed += PlayerAttack;
        _inputManager.OnMeowButtonPressed += PlayerMeow;
    }

    private void OnDisable()
    {
        _inputManager.OnInteractButtonPressed -= PlayerInteract;
        _inputManager.OnAttackButtonPressed -= PlayerAttack;
        _inputManager.OnMeowButtonPressed -= PlayerMeow;
    }

    private void PlayerInteract()
    {
        if (_currentlyHeldItem != null)
        {
            DropHeldItem();
            return;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _interactRadius, _hitBuffer, _interactableLayer);

        PlayerPickup closestItem = FindClosestPickup(hitCount);
        if (closestItem != null)
        {
            _currentlyHeldItem = closestItem;
            _currentlyHeldItem.PickUp(_interactPoint);
        }
    }

    private PlayerPickup FindClosestPickup(int hitCount)
    {
        PlayerPickup closest = null;
        float minDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < hitCount; i++)
        {
            if(_hitBuffer[i].TryGetComponent(out PlayerPickup item))
            {
                float distSqr = (item.transform.position - transform.position).sqrMagnitude;
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    closest = item;
                }
            }
        }
        return closest;
    }
    private void DropHeldItem()
    {
        Vector3 tossVelocity = transform.forward * _throwForce;
        _currentlyHeldItem.Drop(tossVelocity);
        _currentlyHeldItem = null;
    }

    private void PlayerAttack()
    {
        if(Time.time < _nextAttackTime) return;

        _nextAttackTime = Time.time + _attackCooldown;

        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        int hitCount = Physics.OverlapSphereNonAlloc(_attackPoint.position, _attackRadius, _hitBuffer, _attackHitLayer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider target = _hitBuffer[i];

            if (target.transform.root == transform) continue;

            if(target.TryGetComponent(out Rigidbody targetRb) && !targetRb.isKinematic)
            {
                Vector3 knockDirection = (target.transform.position - transform.position).normalized;
                knockDirection.y = 0.25f;
                targetRb.AddForce(knockDirection.normalized * _knockbackForce, ForceMode.Impulse);
            }
        }
    }

    private void PlayerMeow()
    {
        if (Time.time < _nextMeowTime) return;
        if (_meowSFX == null || _meowSFX.Length == 0) return;

        _nextMeowTime = Time.time + meowCooldown;

        AudioClip randomClip = _meowSFX[Random.Range(0, _meowSFX.Length)];
        _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
        _audioSource.PlayOneShot(randomClip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);

        if (_attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
    }

}
