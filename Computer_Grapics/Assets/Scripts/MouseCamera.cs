using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public Transform target; // ���� ��� (�÷��̾�)
    public Vector3 offset; // ī�޶� ��ġ ������

    public float mouseSensitivity = 3.0f;

    private float yaw; // Y�� ȸ�� (�¿�)
    private float pitch; // X�� ȸ�� (����)

    void LateUpdate()
    {
        // pitch�� �ö󰥼��� offset.z �� �þ��
        float minPitch = -30f;
        float maxPitch = 80f;

        // �⺻ offset
        Vector3 baseOffset = new Vector3(0f, 5f, -6f);

        // pitch ���� (0 ~ 1)
        float t = Mathf.InverseLerp(minPitch, maxPitch, pitch);

        // ������ z �� (��: z�� -6���� -10���� �þ����)
        float dynamicZ = Mathf.Lerp(-5f, -15f, t);

        // ���� offset ����
        Vector3 dynamicOffset = new Vector3(baseOffset.x, baseOffset.y, dynamicZ);

        // ���콺 �Է�
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30, 80); // ���� ȸ�� ����

        // ȸ�� ����
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = target.position + rotation * dynamicOffset;
        transform.rotation = rotation;

    }
}
