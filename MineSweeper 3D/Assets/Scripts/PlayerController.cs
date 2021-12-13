using UnityEngine;

namespace PlayerConfiguration
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject player;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform pointer;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private GridManager gridManager;

        public float mouseSensitivity = 100f;
        public float speed = 8f;
        public float gravity = -10f;
        public float jumpHeight = 2f;

        private float groundDistance = 0.3f;
        private float yRotation = 0f;
        private Vector3 velocity;
        private bool IsGrounded = true;
        private bool IsPressedLMB = false;
        private bool IsPressedRMB = false;

        void Start()
        {
            //lock cursor move out of screen bounds
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            velocity.y += gravity * Time.deltaTime;
            if (IsGrounded && velocity.y < 0) velocity.y = -2f;
            if (Input.GetButtonDown("Jump") && IsGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
            controller.Move(velocity * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float axisX = Input.GetAxis("Horizontal");
            float axisZ = Input.GetAxis("Vertical");
            IsPressedLMB = Input.GetButtonUp("Fire1"); // detect pressed button
            IsPressedRMB = Input.GetButtonUp("Fire2"); // detect pressed button

            Vector3 move = player.transform.right * axisX + player.transform.forward * axisZ;
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);
            camera.gameObject.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

            player.transform.Rotate(Vector3.up * mouseX);
            controller.Move(move * speed * Time.deltaTime);
            if (IsPressedLMB || IsPressedRMB) OnButtonPressed();
        }

        void OnButtonPressed()
        {
            const int tileDistance = 3;
            RaycastHit hit;
            Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
            int button = IsPressedRMB ? 2 : 1;

            
            if (Physics.Raycast(cameraCenter, camera.gameObject.transform.forward, out hit, tileDistance))
            {
                GameObject obj = hit.transform.gameObject;
                gridManager.FindPointedTile(obj, button);
            }

            IsPressedLMB = false;
            IsPressedRMB = false;
        }
    }
}