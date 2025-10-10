using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Vector3 offset; // 카메라 위치 오프셋

    public float mouseSensitivity = 3.0f;

    private float yaw; // Y축 회전 (좌우)
    private float pitch; // X축 회전 (상하)

    void LateUpdate()
    {
        // pitch가 올라갈수록 offset.z 값 늘어나게
        float minPitch = -30f;
        float maxPitch = 80f;

        // 기본 offset
        Vector3 baseOffset = new Vector3(0f, 5f, -6f);

        // pitch 비율 (0 ~ 1)
        float t = Mathf.InverseLerp(minPitch, maxPitch, pitch);

        // 보간된 z 값 (예: z가 -6에서 -10까지 늘어나도록)
        float dynamicZ = Mathf.Lerp(-5f, -15f, t);

        // 최종 offset 적용
        Vector3 dynamicOffset = new Vector3(baseOffset.x, baseOffset.y, dynamicZ);

        // 마우스 입력
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30, 80); // 상하 회전 제한

        // 회전 적용
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = target.position + rotation * dynamicOffset;
        transform.rotation = rotation;

    }
}
