using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCamera : MonoBehaviour
{
    public float keyboardMinSpeed = 30f;
    public float keyboardMaxSpeed = 45f;
    public float keyboardAcceleration = 5f;
    
    public float keyboardTimeBeforeAcceleration = 3f;
    public float mouseSpeedMax = 5f;
    public float mouseSpeedMin = 0.3f;

    public float zoomSpeed = 200f;
    public float smoothTime = 0.3f;

    public float distanceFromTerrain = 15f;
    public float maxHeight = 100f;

    public Terrain terrain;

    private bool buttonhold = false;
    private bool lastbuttonhold = false;
    private float currspeedadd = 0f;
    private float acctime = 0f;
    private float mousespeedchange;
    private float keyboardspeedchange;

    private UnityEvent zoomchange;
    private float _currentzoom = 0f;
    private Vector3 cameravelocity = Vector3.zero;
    private Vector3 targetPosition;
    public float currentzoom
    {
        set
        {
            _currentzoom = value;
            zoomchange.Invoke();
        }
        get { return _currentzoom; }
    }

    void Start()
    {
        if(terrain == null) terrain = GameMain.instance.mainTerrain;

        targetPosition = transform.position;

        zoomchange = new UnityEvent();
        zoomchange.AddListener(onZoomChange);

        mousespeedchange = mouseSpeedMin / maxHeight;
        keyboardspeedchange = keyboardMinSpeed / maxHeight;
        
    }

    void Update()
    {
        zoomControl();
        moveByDragMouse();
        moveByKeyboard();
    }

    private void FixedUpdate()
    {
        Vector3 newpos = transform.position;
        newpos = Vector3.SmoothDamp(transform.position, targetPosition, ref cameravelocity, smoothTime);
        transform.position = newpos;
    }

    private void zoomControl()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentzoom -= zoomSpeed;
            checkCameraDistanceFromTerrain();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentzoom += zoomSpeed;
            checkCameraDistanceFromTerrain();
        }
        
    }

    private void checkCameraDistanceFromTerrain()
    {
        float currentheight = terrain.SampleHeight(targetPosition) + distanceFromTerrain;
        if (targetPosition.y < currentheight) currentzoom = currentheight - maxHeight;
    }

    private void onZoomChange()
    {
        if (currentzoom > 0) currentzoom = 0;
            targetPosition.y = maxHeight + currentzoom;
    }

    private void moveByDragMouse()
    {
        if (!Input.GetKey(KeyCode.Mouse2)) return;
        Vector3 mousemovevector = Vector3.zero; 
        mousemovevector.x = Input.GetAxis("Mouse X");
        mousemovevector.z = Input.GetAxis("Mouse Y");
        if (mousemovevector != Vector3.zero)
        {
            targetPosition -= mousemovevector * Time.deltaTime * (mousespeedchange * currentzoom + mouseSpeedMax);
            checkCameraDistanceFromTerrain();
        }
    }


    private void moveByKeyboard()
    {
        Vector3 addPos = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            addPos.z = 1;
            buttonhold = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            addPos.z = -1;
            buttonhold = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            addPos.x = -1;
            buttonhold = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            addPos.x = 1;
            buttonhold = true;
        }

        if (lastbuttonhold && buttonhold)
        {
            if (acctime == 0f)
            {
                acctime = Time.time + keyboardTimeBeforeAcceleration;
            }
            else if (Time.time >= acctime)
                currspeedadd += keyboardAcceleration * Time.deltaTime;
            else currspeedadd = 0f;
        }
        else acctime = 0f;
      
        if (keyboardMinSpeed + currspeedadd > keyboardMaxSpeed)
            currspeedadd = keyboardMaxSpeed - keyboardMinSpeed;

        if (addPos != Vector3.zero)
        {
            float _speed = ((keyboardspeedchange * currentzoom + mouseSpeedMax) + currspeedadd);
            if (_speed > keyboardMaxSpeed)
                _speed = keyboardMaxSpeed;
            float movevalue = Time.deltaTime * _speed;
            targetPosition += Vector3.Normalize(addPos) * movevalue;
            checkCameraDistanceFromTerrain();
        }


        lastbuttonhold = buttonhold;
        buttonhold = false;
    }
}
