using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BunnyController : MonoBehaviourPun
{
    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;
    [SerializeField] float turn_smooth_time;

    private float turn_smooth_velocity;

    [SerializeField] float speed_smooth_time;
    private float speed_smooth_velocity;
    private float curr_speed;

    private float speed;
    //private float animation_speed_percent;
    //private Animator animator;

    private float gravity = -12f;
    private float velocity_y;
    [SerializeField] float jump_speed;

    [SerializeField] Transform camera_transform;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        //animator = this.GetComponent<Animator>();
        controller = this.GetComponent<CharacterController>();
        camera_transform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // controller input
        Vector2 input_direction = input.normalized; // normalize the direction vector to prevent fast diagonal movement
        if (input_direction != Vector2.zero) // rotate the player model towards the input direction
        {
            float target_rotation = Mathf.Atan2(input_direction.x, input_direction.y) * Mathf.Rad2Deg + camera_transform.eulerAngles.y;
            this.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(this.transform.eulerAngles.y, target_rotation, ref turn_smooth_velocity, turn_smooth_time);
        }

        bool is_running = Input.GetKey(KeyCode.LeftShift); // check if running

        float target_speed = (is_running ? run_speed : walk_speed) * input_direction.magnitude; // the speed we want to reach
        curr_speed = Mathf.SmoothDamp(curr_speed, target_speed, ref speed_smooth_velocity, speed_smooth_time); // damp to the target speed from our current speed

        if (controller.isGrounded && input_direction != Vector2.zero)
        {
            velocity_y = jump_speed;
        }

        velocity_y += gravity * Time.deltaTime;
        Vector3 velocity = this.transform.forward * curr_speed + Vector3.up * velocity_y; // velocity

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocity_y = 0;
        }

        //controller.Move(velocity * Time.deltaTime);

        //animation_speed_percent = (is_running ? 1f : 0.5f) * input_direction.magnitude; // handles the animation speed percent
        //animator.SetFloat("speedPercent", animation_speed_percent, speed_smooth_time, Time.deltaTime); // dampen the animation to the target animation
    }
}
