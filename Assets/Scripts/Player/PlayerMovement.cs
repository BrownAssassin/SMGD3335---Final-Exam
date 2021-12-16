using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    private Vector3 velocity;
    private float gravity = -9.8f;
    private float speed = 6.0f;
    private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private CinemachineFreeLook cinemachineFreeLook;
    float cameraAngle;
    float cameraAngleSpeed;

    private Joystick movementStick;
    private Joystick cameraStick;

    public PhotonView photonView;

    void Update()
    {
        if (photonView.IsMine)
        {
            float horizontal = movementStick.Horizontal;
            float vertical = movementStick.Vertical;

            Vector3 initDirection = new Vector3(horizontal, 0.0f, vertical).normalized;

            float targetAngle = Mathf.Atan2(initDirection.x, initDirection.z) * Mathf.Rad2Deg + cinemachineFreeLook.transform.eulerAngles.y;
            float finalAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            Vector3 finalDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

            if (initDirection.magnitude >= 0.1f)
            {
                transform.rotation = Quaternion.Euler(0.0f, finalAngle, 0.0f);

                characterController.Move(finalDirection.normalized * speed * Time.deltaTime);
            }

            cinemachineFreeLook.m_XAxis.Value += cameraStick.Horizontal * 100 * Time.deltaTime;
            cinemachineFreeLook.m_YAxis.Value += -cameraStick.Vertical * Time.deltaTime;
            
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    public void JoystickSetup(GameObject camera)
    {
        Joystick[] joystickList = camera.GetComponentsInChildren<Joystick>();

        foreach (Joystick joystick in joystickList)
        {
            if (joystick.tag == "Move Stick")
                movementStick = joystick;
            else if (joystick.tag == "Cam Stick")
                cameraStick = joystick;
        }

        cinemachineFreeLook = camera.GetComponentInChildren<CinemachineFreeLook>();
        cinemachineFreeLook.LookAt = GameObject.Find("Neck").transform;
        cinemachineFreeLook.Follow = transform;
    }
}
