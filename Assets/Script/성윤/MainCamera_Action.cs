using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera_Action : MonoBehaviour
{
    // 카메라가 따라가야할 오브젝트의 정보
    Transform PlayerToFollow;
    // 카메라가 따라가는 스피드
    public float followSpeed = 10.0f;
    // 마우스 감도
    public float sensitivity;
    // 마우스 제한 각도
    public float clamAngle;

    // 마우스 입력
    private float rotX;
    private float rotY;
    // 카메라의 정보
    Transform realCamera;
    // 카메라의 방향
    public Vector3 dirNormalized;
    // 카메라의 최종적으로 정해진 방향
    public Vector3 finalDir;
    // 카메라 최소거리
    public float minDistance;
    // 카메라 최대거리
    public float maxDistance;
    // 카메라의 최종적으로 정해진 거리
    public float finalDistance;

    public float smoothness = 10.0f;

    Vector2 cameraDir = Vector2.zero;
    PlayerInputActions inputActions;


    private void Awake()
    {
        PlayerToFollow = FindObjectOfType<TestPlayer>().transform.GetChild(2).transform;
        realCamera = transform.GetChild(0);
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Mouse.performed += PlayerMouse;
        inputActions.Player.Mouse.canceled += PlayerMouse;
    }

    private void OnDisable()
    {
        inputActions.Player.Mouse.canceled -= PlayerMouse;
        inputActions.Player.Mouse.performed -= PlayerMouse;
        inputActions.Player.Disable();
    }
    private void Start()
    {
        // 카메라 인풋 초기화
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;
        // 벡터값 초기화해서 시키기
        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 dir = new Vector2(cameraDir.x, cameraDir.y);
        rotX += -(dir.y * sensitivity) * Time.deltaTime;
        rotY += dir.x * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clamAngle, clamAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position,PlayerToFollow.position, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;
        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }



    private void PlayerMouse(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        cameraDir = dir;
    }
}
