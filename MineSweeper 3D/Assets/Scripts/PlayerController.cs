using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerConfiguration
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject Camera;
        [SerializeField] private GameObject Player;
        [SerializeField] private CharacterController Controller;
        [SerializeField] private Transform GroundCheck;
        [SerializeField] private LayerMask groundMask;

        public float mouseSensitivity = 100f;
        public float speed = 8f;
        public float gravity = -10f;
        public float jumpHeight = 2f;
        public bool IsFirstView = true;
        private float groundDistance = 0.3f;
        private float yRotation = 0f;
        private Vector3 velocity;
        private bool IsGrounded;

        void Start()
        {
            //lock cursor move out of screen bounds
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            IsGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
            velocity.y += gravity * Time.deltaTime;
            if (IsGrounded && velocity.y < 0) velocity.y = -2f;
            if (Input.GetButtonDown("Jump") && IsGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
            Controller.Move(velocity * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float axisX = Input.GetAxis("Horizontal");
            float axisZ = Input.GetAxis("Vertical");

            Vector3 move = Player.transform.right * axisX + Player.transform.forward * axisZ;
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);
            Camera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
            Player.transform.Rotate(Vector3.up * mouseX);
            Controller.Move(move * speed * Time.deltaTime);
        }
    }
}