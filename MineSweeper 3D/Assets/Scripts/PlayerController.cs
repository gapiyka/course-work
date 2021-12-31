using UnityEngine;

namespace PlayerConfiguration
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject player;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private AudioController audioController;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private GameObject tool;

        private float mouseSensitivity = 100f;
        private float speed = 8f;
        private float gravity = -10f;
        private float jumpHeight = 2f;

        private float groundDistance = 0.3f;
        private float yRotation = 0f;
        private Vector3 velocity;
        private bool IsGrounded = true;
        private bool IsPressedLMB = false; 
        private bool IsPressedRMB = false;
        private bool animationNotExecuting = true;
        public bool IsPressedESC = false;
        public bool IsGameRunning = false;
        public bool IsGamePaused = false;

        void Start()
        {
            UnlockScreenCursor();
        }

        void Update()
        {
            IsPressedESC = Input.GetKeyDown(KeyCode.Escape);
            if (gridManager.gameState == GameState.Lose || gridManager.gameState == GameState.Win) IsPressedESC = false;
            if (IsGameRunning && !IsGamePaused)
            {
                CalculateMovements();
                CalculateMouseActions();
            }
        }

        void CalculateMovements()
        {
            float axisX = Input.GetAxis("Horizontal");
            float axisZ = Input.GetAxis("Vertical");

            animator.SetFloat("x", axisX);
            animator.SetFloat("z", axisZ);

            IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            velocity.y += gravity * Time.deltaTime;
            Vector3 move = player.transform.right * axisX + player.transform.forward * axisZ;
            if (IsGrounded && velocity.y < 0) OnTouchDown();
            if (Input.GetButtonDown("Jump") && IsGrounded) OnJump();
            controller.Move(velocity * Time.deltaTime);
            controller.Move(move * speed * Time.deltaTime);
        }

        void CalculateMouseActions()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);
            camera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

            player.transform.Rotate(Vector3.up * mouseX);

            MouseButtonsPressing();
        }

        void MouseButtonsPressing()
        {
            IsPressedLMB = Input.GetButtonUp("Fire1"); // detect pressed button
            IsPressedRMB = Input.GetButtonUp("Fire2"); // detect pressed button

            if (IsPressedLMB || IsPressedRMB) OnButtonPressed();
        }

        void OnTouchDown()
        {
            bodyTransform.localPosition = new Vector3(0, 0, 0);
            bodyTransform.localRotation = Quaternion.identity;
            //velocity.y = -2f;
            animator.SetBool("isGrounded", true);
            tool.SetActive(true);
        }

        void OnJump()
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
            audioController.PlaySoundJump();
            animator.SetBool("isGrounded", false);
            tool.SetActive(false);
        }

        void OnButtonPressed()
        {
            const int tileDistance = 4;
            RaycastHit hit;
            Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));// point at center of screen
            int button = IsPressedRMB ? 2 : 1; // 2==right / 1==left

            if (Physics.Raycast(cameraCenter, camera.gameObject.transform.forward, out hit, tileDistance))
            {
                GameObject obj = GetFullParent(hit.transform).gameObject; 
                int res = gridManager.FindPointedTile(obj, button);
                PlaySoundsByButton(res, button);
            }

            IsPressedLMB = false;
            IsPressedRMB = false;
        }

        Transform GetFullParent(Transform child)
        {
            if (child.parent != null)
            {
                Transform parentTransform = child.parent;
                if(parentTransform.name != "PlayGrid") return GetFullParent(parentTransform);
                else return child;
            }
            else return child;
        }

        void PlaySoundsByButton(int res, int button)
        {
            if (button == res) audioController.PlaySoundSelect(res - 1);
            if (res == -1) audioController.PlaySoundBoom();
        }

        public void UnlockScreenCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        //lock cursor move out of screen bounds
        public void LockScreenCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UpdateSettings(SaveJsonObject saveObject)
        {
            mouseSensitivity = saveObject.mouseSensitivity;
            audioController.ChangeVolumes(saveObject.musicVolume, saveObject.vfxVolume);
        }

        public void WinAnimate()
        {
            if (animationNotExecuting) {
                animationNotExecuting = false;
                //move camera ahead of palyer and rotate to look at aniamtion
                player.transform.eulerAngles = new Vector3(0, 0, 0);
                camera.transform.eulerAngles = new Vector3(0, 180, 0);
                camera.transform.position += 2f * Vector3.forward;;

                animator.Play("Win");
            }
        }
    }
}