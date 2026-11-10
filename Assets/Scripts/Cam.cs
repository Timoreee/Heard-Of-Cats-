using System.Security.Cryptography;
using UnityEngine;

public class Cam : MonoBehaviour
{
    //player and input manager
    public Transform player;
    [SerializeField]
    private PlayerInputManager _inputManager;

    //camera setup
    [SerializeField]
    private float _cameraHeight;
    [SerializeField]
    private float _cameraDistance;
    private Vector3 _currentRotation;

    //camera sensitivity
    [SerializeField]
    private float _xSensitivity;
    [SerializeField]
    private float _ySensitivity;

    //lock camera angle
    [SerializeField]
    private float _minimumAngle;
    [SerializeField]
    private float _maximumAngle;

    //smoothing camera movement
    [SerializeField]
    private float _smoothingTime;
    private Vector3 _smoothVelocity;

    [SerializeField]
    private LayerMask _collisionLayer;
    [SerializeField]
    private float _collisionRadius;
    [SerializeField]
    private float _minimumPlayerDistance;
    
    //rotation of camera
    private float _yaw;
    private float _pitch;

    //per frame, runs after everything is set up
    void LateUpdate()
    {
        //checks if player and input manager exist
       if (player == null || _inputManager == null) return;

        //grabs input manager looks
       Vector2 lookInput = _inputManager.LookInput;
       //left and right movement or x axis
       _yaw += lookInput.x * _xSensitivity * 3;
       //up and down movement or y axis
       _pitch -= lookInput.y * _ySensitivity * 3;
        //locks camera camera to minimum and maximum angle
       _pitch = Mathf.Clamp(_pitch, _minimumAngle, _maximumAngle);

        //stores rotational angle in pitch and yaw
       Vector3 targetRotation = new Vector3(_pitch, _yaw, 0f);

       //smooths and dampens rotation over time rather than jerking
       _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _smoothVelocity, _smoothingTime);

        //converts degrees to quaternions for Unity
       Quaternion rotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0f);

        //keeps the player in the center of the camera
       Vector3 targetCenter = player.position + Vector3.up * _cameraHeight;

       Vector3 directionToCamera =- (rotation * Vector3.forward);

       float realDistance = _cameraDistance;

       if(Physics.SphereCast(targetCenter, _collisionRadius, directionToCamera, out RaycastHit hit, _cameraDistance, _collisionLayer, QueryTriggerInteraction.Ignore))
        {
            realDistance = Mathf.Clamp(hit.distance, _minimumPlayerDistance, _cameraDistance);
        }

        //sets the camera at a set distance away from the player
       transform.position = targetCenter - (rotation * Vector3.forward * realDistance);
       //looks at player
       transform.LookAt(targetCenter);
    }
}
