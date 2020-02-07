using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;
    [SerializeField] float turn_smooth_time;

    private float turn_smooth_velocity;

    [SerializeField] float speed_smooth_time;
    private float speed_smooth_velocity;
    private float curr_speed;

    private float speed;
    private float animation_speed_percent;
    private Animator animator;

    private Transform camera_transform;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        camera_transform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input_direction = input.normalized;
        if (input_direction != Vector2.zero)
        {
            float target_rotation = Mathf.Atan2(input_direction.x, input_direction.y) * Mathf.Rad2Deg + camera_transform.eulerAngles.y;
            this.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(this.transform.eulerAngles.y, target_rotation, ref turn_smooth_velocity, turn_smooth_time);
        }

        bool is_running = Input.GetKey(KeyCode.LeftShift);

        float target_speed = (is_running ? run_speed : walk_speed) * input_direction.magnitude;
        curr_speed = Mathf.SmoothDamp(curr_speed, target_speed, ref speed_smooth_velocity, speed_smooth_time);

        this.transform.Translate(this.transform.forward * curr_speed * Time.deltaTime, Space.World);
        
        animation_speed_percent = (is_running ? 1f : 0.5f) * input_direction.magnitude;
        animator.SetFloat("speedPercent", animation_speed_percent, speed_smooth_time, Time.deltaTime);
    }
}
