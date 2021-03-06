﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotate : MonoBehaviour {

    public Transform mainCamera;
    public FloatReference mouseY;
    public FloatReference mouseX;


    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;

    private void Awake() {
        Init(transform);
    }

    public void Init(Transform character) {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = mainCamera.localRotation;
    }

    public void LookRotation(Transform character) {
        float yRot = mouseX * XSensitivity;
        float xRot = mouseY * YSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth) {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            mainCamera.localRotation = Quaternion.Slerp(mainCamera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else {
            character.localRotation = m_CharacterTargetRot;
            mainCamera.localRotation = m_CameraTargetRot;
        }

        UpdateCursorLock();
    }

    public void ResetRotation(Transform character, Transform camera, float y) {
        character.localRotation = Quaternion.Euler(new Vector3(0, y, 0));
        camera.localRotation = Quaternion.Euler(Vector3.zero);
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        //Init(character, camera);
    }

    public void SetCursorLock(bool value) {
        lockCursor = value;
        if (!lockCursor) {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock() {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0)) {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q) {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}
