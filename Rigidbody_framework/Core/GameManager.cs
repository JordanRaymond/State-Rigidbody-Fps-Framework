using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance
        {
            get { return instance; }
        }

        public static InputHandler InputHandler
        {
            get { return inputHandlerInstance; }
        }

        public static FPS.MouseLook Camera
        {
            get { return mouseLookInstance; }
        }

        private static GameManager instance;
        private static InputHandler inputHandlerInstance;

        private static FPS.MouseLook mouseLookInstance;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("Already have an instance of GameManager.");
            }

            InitInputHandler();
        }

        private void InitInputHandler()
        {
            if (inputHandlerInstance == null)
            {
                inputHandlerInstance = gameObject.AddComponent(typeof(InputHandler)) as InputHandler;
            }
            else
            {
                Debug.LogError("Already have an instance of InputHandler.");
            }
        }

        private void InitMouslook()
        {
            if (mouseLookInstance == null)
            {
                mouseLookInstance = FindObjectOfType<FPS.MouseLook>();
            }
            else
            {
                Debug.LogError("Already have an instance of MouseLook.");
            }
        }
    }
}