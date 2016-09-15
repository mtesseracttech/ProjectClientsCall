using UnityEngine;
using System.Collections;
using System.Text;

public class MovementBehavior : MonoBehaviour {

    public float _moveSpeed = 6; 
    public float _turnSpeed = 90; 
    public float _lerpSpeed = 10; 
    public float gravity = 2; 
    private bool _isGrounded;
    private float _deltaGround = 0.2f; 
    public float _jumpSpeed = 10; 
    public float _climbRange = 1; // range to detect target wall
    private Vector3 _surfaceNormal; // current surface normal
    private Vector3 _myNormal; // character normal
    private float _distGround; // distance from character position to ground
    private bool _jumping = false; 
    public float _vertSpeed = 5;
    private bool _climbing = false;
    private float _grounded;

    private Transform _myTransform;
    private BoxCollider _boxCollider; 
    private Rigidbody _rigidbody;

   
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _myNormal = transform.up;
        _myTransform = transform;
        _rigidbody.freezeRotation = true; // distance from transform.position to ground
        _distGround = _boxCollider.size.y - _boxCollider.center.y;
    }

 
    private void FixedUpdate()
    {
        // apply constant weight force according to character normal:
        _rigidbody.AddForce(-gravity * _rigidbody.mass * _myNormal);
    }

    private bool isGrounded()
    {
        // Physics.CheckCapsule(collider.bounds.center,
        //   new Vector3(collider.bounds.center.x, collider.bounds.min.y - 0.1f, collider.bounds.center.z), 0.18f);
        return Physics.Raycast(transform.position, -Vector3.up, _grounded + 0.1f);
        //raycast down direction to check if the ground is there
        //i add +0.1f of small iregularities or inclination
    }

    private void Update()
    {
        Debug.DrawLine(_myTransform.position, _myTransform.position + _myTransform.forward * _climbRange, Color.blue);
        if (_climbing) return; 
        if (_jumping) return;

        Ray ray;
        RaycastHit hit;
        ray = new Ray(_myTransform.position, _myTransform.forward);

        if (Physics.Raycast(ray, out hit, _climbRange))
        {
            ClimbToWall(hit.point, hit.normal);

        }



        if (Input.GetKeyDown(KeyCode.Space)&& !isGrounded() && !_jumping)
        { 
            JumpOffWall();
        }


        ray = new Ray(_myTransform.position, -_myNormal); // cast ray downwards
        if (Physics.Raycast(ray, out hit))
        { 
            _isGrounded = hit.distance <= _distGround + _deltaGround;          
            _surfaceNormal = hit.normal;
            Debug.DrawLine(_myTransform.position,_myTransform.position + ray.direction,Color.white);
        }
        else
        {
            _isGrounded = false;           
            _surfaceNormal = Vector3.up;
        }

        
        _myNormal = Vector3.Lerp(_myNormal, _surfaceNormal, _lerpSpeed * Time.deltaTime);
        // find forward direction with new myNormal:
        Vector3 myForward = Vector3.Cross(_myTransform.right, _myNormal);
        // align character to the new myNormal while keeping the forward direction:
        Quaternion targetRot = Quaternion.LookRotation(myForward, _myNormal);
        _myTransform.rotation = Quaternion.Lerp(_myTransform.rotation, targetRot, _lerpSpeed * Time.deltaTime);
        // move the character forth/back with Vertical axis:
        _myTransform.Translate(0, 0, Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime);
        
    }



    //climbing is fucked. need fixing
    private void ClimbToWall(Vector3 point, Vector3 normal)
    {
        _climbing = true; 
        _rigidbody.isKinematic = true; 
        Vector3 orgPos = _myTransform.position;
        Vector3 dstPos = point + normal * (_distGround + 0.1f); 
        StartCoroutine(jumpTime(orgPos, dstPos, normal, 0.1f));
    }

    private void JumpOffWall()
    {
        _jumping = true;
        Vector3 orgPos = _myTransform.position;
        StartCoroutine(jumpTime(orgPos,  orgPos, Vector3.up, 0.2f));
    }

    private IEnumerator jumpTime(Vector3 orgPos, Vector3 dstPos,  Vector3 normal, float time)
    {
        _rigidbody.velocity += _jumpSpeed * _myNormal;
        for (float t = 0.0f; t < time;)
        {
            t += Time.deltaTime;
            _myTransform.position = Vector3.Lerp(orgPos, dstPos, t);
            yield return null; 
        }
        _myNormal = normal;
        _rigidbody.isKinematic = false; 
        _climbing = false;
        _jumping = false;
    }
}
