using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;

    private Camera attachedCamera;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 35f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float xRotation;

    private void Start()
    {
        attachedCamera = GetComponent<Camera>();
        EnsureOnlyThisCameraIsActive();
        LockCursor();
    }

    private void Update()
    {
        if (ReadCursorUnlockPressed())
        {
            UnlockCursor();
        }

        if (ReadCursorLockPressed())
        {
            LockCursor();
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        Vector2 lookInput = ReadLookInput();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private Vector2 ReadLookInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.delta.ReadValue();
        }
#endif

        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private bool ReadCursorUnlockPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        }
#endif

        return Input.GetKeyDown(KeyCode.Escape);
    }

    private bool ReadCursorLockPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }
#endif

        return Input.GetMouseButtonDown(0);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnsureOnlyThisCameraIsActive()
    {
        if (attachedCamera == null)
        {
            attachedCamera = GetComponent<Camera>();
        }

        if (attachedCamera == null)
        {
            return;
        }

        attachedCamera.enabled = true;
        attachedCamera.tag = "MainCamera";

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera camera = cameras[i];
            if (camera != attachedCamera)
            {
                camera.enabled = false;
            }
        }

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        for (int i = 0; i < listeners.Length; i++)
        {
            AudioListener listener = listeners[i];
            listener.enabled = listener.gameObject == gameObject;
        }

        AudioListener ownListener = GetComponent<AudioListener>();
        if (ownListener == null)
        {
            gameObject.AddComponent<AudioListener>();
            ownListener = GetComponent<AudioListener>();
        }

        if (ownListener != null)
        {
            ownListener.enabled = true;
        }
    }
}
