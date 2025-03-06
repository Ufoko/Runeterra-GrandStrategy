using UnityEngine;

public class RTSCameraMovement : MonoBehaviour
{
    [SerializeField] private float _smoothingPanningModifier = 0.075f;
    [SerializeField] private float _panningSpeed = 0.1f;
    private const float _mousePanSpeedModifier = 0.125f;
    private Vector3 _lastPanningMovement;
    private Vector3 _cameraTarget;

    [SerializeField] private float _rotationSpeed = 0.1f;
    private Vector3 _lastMousePos;

    [SerializeField] private float _zoomSpeed = 50.0f;
    public float MaxZoom = 200.0f;
    [SerializeField] private float _minZoom = 5.0f;
    private float _currentZoom;

    [SerializeField] private LayerMask _groundLayer = 0;
    [SerializeField] private float _minDistanceFromGround = 5.0f;

    [SerializeField] private float _edgeScrollingZone = 20.0f;

    private float _lastNotTouchingGroundY;
    private float _lastTouchingGroundY;
    private bool _touchedGroundLastTick;

    private void Awake()
    {
        _currentZoom = MaxZoom;//_minZoom + ((MaxZoom - _minZoom) / 2);
    }

    private void Update()
    {
        if(Time.timeScale > 0)
        {
            // Inputs
            if(Player.local)
            {
                UpdatePanningInput();
                UpdateRotationInput();
                UpdateZoomInput();
            }

            // Executions
            UpdatePosition();
        }
        _lastMousePos = Input.mousePosition;
    }

    private void UpdatePanningInput()
    {
        Vector3 panMovement = Vector3.zero;

        // Keyboard movement
        if(Input.GetKey(KeyCode.W))
        {
            panMovement.z += 1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            panMovement.z -= 1;
        }
        if(Input.GetKey(KeyCode.A))
        {
            panMovement.x -= 1;
        }
        if(Input.GetKey(KeyCode.D))
        {
            panMovement.x += 1;
        }

        // Edge scrolling
        if(IsMouseWithinBounds())
        {
            if(Input.mousePosition.x <= _edgeScrollingZone)
            {
                panMovement.x -= 1;
            }
            else if(Input.mousePosition.x >= Screen.width - _edgeScrollingZone)
            {
                panMovement.x += 1;
            }
            if(Input.mousePosition.y <= _edgeScrollingZone)
            {
                panMovement.z -= 1;
            }
            else if(Input.mousePosition.y >= Screen.height - _edgeScrollingZone)
            {
                panMovement.z += 1;
            }
        }

        if(Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 mouseMovement = (Input.mousePosition - _lastMousePos);
            panMovement += new Vector3(-mouseMovement.x, 0.0f, -mouseMovement.y) * _mousePanSpeedModifier;
        }

        Vector3 smoothedMovement = Vector3.zero;
        if(_smoothingPanningModifier > 0)
        {
            smoothedMovement = Vector3.Lerp(_lastPanningMovement, panMovement, _smoothingPanningModifier);
            _lastPanningMovement = smoothedMovement;
        }

        float oldRotationX = transform.localEulerAngles.x; // Make sure the rotation doesn't mess up the camera forward movement
        transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);

        float zoomMultiplier = Mathf.Sqrt(_currentZoom);
        _cameraTarget = _cameraTarget + transform.TransformDirection(smoothedMovement) * _panningSpeed * zoomMultiplier * Time.deltaTime / Time.timeScale;

        transform.localEulerAngles = new Vector3(oldRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z); ;
    }

    private void UpdateRotationInput()
    {
        float deltaAngleH = 0.0f;
        float deltaAngleV = 0.0f;

        if((Input.GetMouseButton(1) || Input.GetMouseButton(2)) && !Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 mouseMovement = Input.mousePosition - _lastMousePos;
            deltaAngleH += mouseMovement.x;
            deltaAngleV -= mouseMovement.y;
        }

        float newRotationX = Mathf.Min(80.0f, Mathf.Max(5.0f, transform.localEulerAngles.x + deltaAngleV * _rotationSpeed * Time.deltaTime / Time.timeScale));
        float newRotationY = transform.localEulerAngles.y + deltaAngleH * _rotationSpeed * Time.deltaTime / Time.timeScale;
        transform.localEulerAngles = new Vector3(newRotationX, newRotationY, transform.localEulerAngles.z);
    }

    private void UpdateZoomInput()
    {
        float zoomMovement = 0.0f;

        float mScroll = Input.GetAxis("Mouse ScrollWheel");
        zoomMovement -= mScroll;
        //(zoomedOutRatio * 2.0f + 1.0f)
        float zoomMultiplier = Mathf.Sqrt(_currentZoom);
        _currentZoom = Mathf.Max(_minZoom, Mathf.Min(MaxZoom, _currentZoom + zoomMovement * _zoomSpeed * zoomMultiplier * Time.deltaTime / Time.timeScale));
    }

    private void UpdatePosition()
    {
        if(_minDistanceFromGround > 0.0f)
        {
            RaycastHit hit;
            if(!Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundLayer) || hit.distance >= _minDistanceFromGround)
            {
                _touchedGroundLastTick = true;
                _lastTouchingGroundY = hit.point.y + _minDistanceFromGround;
                _cameraTarget = new Vector3(_cameraTarget.x, _lastTouchingGroundY, _cameraTarget.z);
                transform.position = _cameraTarget;
                transform.Translate(Vector3.back * _currentZoom);
            }
            else
            {
                _cameraTarget = new Vector3(_cameraTarget.x, _lastNotTouchingGroundY, _cameraTarget.z);
                if(_touchedGroundLastTick)
                {
                    _lastNotTouchingGroundY = _cameraTarget.y;
                }
                transform.position = _cameraTarget;
                transform.Translate(Vector3.back * _currentZoom);
                if(!Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundLayer) || hit.distance >= _minDistanceFromGround)
                {
                    _touchedGroundLastTick = true;
                    if(hit.distance > 0.0f)
                    {
                        _cameraTarget = new Vector3(_cameraTarget.x, hit.point.y + _minDistanceFromGround, _cameraTarget.z);
                    }
                    else
                    {
                        _cameraTarget = new Vector3(_cameraTarget.x, _lastTouchingGroundY + _minDistanceFromGround, _cameraTarget.z);
                    }
                    transform.position = _cameraTarget;
                    transform.Translate(Vector3.back * _currentZoom);
                }
                else
                {
                    _touchedGroundLastTick = false;
                }
            }
        }
        /*if(_minDistanceFromGround > 0.0f)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, _minDistanceFromGround + 5.0f, _groundLayer) && hit.distance > 1.0f)
            {
                _touchedGroundLastTick = true;
                _cameraTarget = new Vector3(_cameraTarget.x, hit.point.y + _minDistanceFromGround, _cameraTarget.z);
                transform.position = _cameraTarget;
                transform.Translate(Vector3.back * _currentZoom);
            }
            else
            {
                _touchedGroundLastTick = false;
                _cameraTarget = new Vector3(_cameraTarget.x, _lastNotTouchingGroundY, _cameraTarget.z);
                if(_touchedGroundLastTick)
                {
                    _lastNotTouchingGroundY = _cameraTarget.y;
                }
                transform.position = _cameraTarget;
                transform.Translate(Vector3.back * _currentZoom);
            }
        }*/
    }

    private bool IsMouseWithinBounds()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if(!screenRect.Contains(Input.mousePosition))
        {
            return false;
        }
        return true;
    }
}
