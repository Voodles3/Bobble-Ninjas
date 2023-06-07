using UnityEngine;

    public class PlayerLookAtMouse : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;

        private Camera mainCamera;

        public Vector3 direction;
        float period = 0f;

        private void Start()
        {
            // Cache the camera, Camera.main is an expensive operation.
            mainCamera = Camera.main;
        }

        private void Update()
        {
            CallAim();
        }

        void CallAim()
        {
            if (period >= 0.01f)
            {
                Aim();
                period = 0f;
            }
            period += Time.deltaTime;
        }

        private void Aim()
        {
            var (success, position) = GetMousePosition();
            
            if (success)
            {
                // Calculate the direction
                direction = position - transform.position;

                direction.y = 0;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);
            }
        }

        private (bool success, Vector3 position) GetMousePosition()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
            {
                // The Raycast hit something, return with the position.
                return (success: true, position: hitInfo.point);
            }
            else
            {
                // The Raycast did not hit anything.
                return (success: false, position: Vector3.zero);
            }
        }
    }