using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviourPun
{
    [Header("IK/Hunter/Canvas")]
    [SerializeField] Hunter hunter;
    [SerializeField] IKSolver ik_solver;

    [SerializeField] Canvas aim_canvas;

    [Space]
    [Header("Rendering")]
    [SerializeField] Material disappear_mat;
    [SerializeField] Material body_mat;
    [SerializeField] Renderer skin;
    [SerializeField] float disappear_transition;
    //[SerializeField] float conjure_transition;
    private const string FADE_NAME = "_Fade_Amount";
    //[SerializeField] Color skin_color;

    [Space]
    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;
    [SerializeField] float turn_smooth_time;

    private float turn_smooth_velocity;

    [SerializeField] float speed_smooth_time;
    [SerializeField] float throw_smooth_time;
    private float speed_smooth_velocity;
    private float curr_speed;

    private float speed;
    private float animation_speed_percent;
    private float animation_throw;
    private Animator animator;

    [Range(0, 1)]
    [SerializeField] float air_control_percent;
    [SerializeField] float jump_height;
    private float gravity = -12f;
    private float velocity_y;

    private Transform camera_transform;
    [SerializeField] Transform camera_move_to;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        camera_transform = Camera.main.transform;
        controller = this.GetComponent<CharacterController>();
        hunter = this.GetComponent<Hunter>();
        ik_solver = this.GetComponent<IKSolver>();
        skin.material = body_mat;
        //conjure_transition = 1f;
        disappear_transition = 0f;
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
            this.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(this.transform.eulerAngles.y, target_rotation, ref turn_smooth_velocity, GetModifiedSmoothTime(speed_smooth_time));
        }

        bool is_running = Input.GetKey(KeyCode.LeftShift); // check if running

        bool is_crouching = Input.GetKey(KeyCode.LeftControl) && controller.isGrounded;

        float target_speed = (is_running ? run_speed : walk_speed) * input_direction.magnitude; // the speed we want to reach

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // start cloaking
            photonView.RPC("HandleHunterCloaking", RpcTarget.All);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // decloak
            photonView.RPC("HandleHunterDecloaking", RpcTarget.All);
        }

        if (is_crouching) // if we are crouching
        {
            if (input_direction != Vector2.zero) // if we have input movement, then move in the walk speed
                target_speed = walk_speed;
            else
                target_speed = 0f; // otherwise crouch still
        }

        //photonView.RPC("Disappear", RpcTarget.All, is_crouching);

        curr_speed = Mathf.SmoothDamp(curr_speed, target_speed, ref speed_smooth_velocity, GetModifiedSmoothTime(speed_smooth_time)); // damp to the target speed from our current speed

        if (Input.GetKey(KeyCode.Space) && !is_crouching)
        {
            ik_solver.enableFeetIk = false;
            Jump();
            animator.SetTrigger("jump");
            animator.SetInteger("air", 1);
        }

        if (controller.isGrounded)
        {
            ik_solver.enableFeetIk = true;
            animator.SetInteger("air", 0);
        }

        velocity_y += gravity * Time.deltaTime; // account gravity

        Vector3 velocity = this.transform.forward * curr_speed + Vector3.up * velocity_y; // velocity

        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("crouching", is_crouching);

        animation_speed_percent = (is_running ? 1f : 0.5f) * input_direction.magnitude; // handles the animation speed percent
        // Debug.Log(animation_speed_percent);
        animator.SetFloat("speedPercent", animation_speed_percent, speed_smooth_time, Time.deltaTime); // dampen the animation to the target animation

        HandleConjure();
    }

    [PunRPC]
    private void HandleHunterCloaking()
    {
        StartCoroutine(Cloak());
    }

    [PunRPC]
    private void HandleHunterDecloaking()
    {
        StartCoroutine(Decloak());
    }

    private void HandleConjure()
    {
        if (hunter.GetConjureTransition() == 1)
        {
            hunter.can_conjure = true;
        }
        else
        {
            hunter.can_conjure = false;
        }
        if (Input.GetMouseButtonDown(1) && hunter.can_conjure) // if you can conjure a spear and start aiming
        {
            hunter.CreateSpearWrapper();
            hunter.FadeInSpearWrapper();
        }
        else if (Input.GetMouseButtonUp(1) && hunter.has_spear) // if you never threw the spear and stop aiming
        {
            hunter.FadeOutSpearWrapper();
        }
        //else if (Input.GetMouseButtonUp(1) && !hunter.has_spear) // if you stop aiming and no longer have a spear
        //{
        //    //conjure_transition = 1;
        //    hunter.SetConjureTransitionNetworkWrapper(1f); // set the transition value back to 1
        //}
        bool is_aiming = Input.GetMouseButton(1) && hunter.has_spear;
        if (is_aiming)
        {
            //conjure_transition -= Time.deltaTime;
            //conjure_transition = Mathf.Clamp(conjure_transition, 0, 1);
            //hunter.conjured_spear_mat.SetFloat(FADE_NAME, conjure_transition);
            //hunter.FadeInSpearWrapper();

            aim_canvas.gameObject.SetActive(is_aiming);
            bool throw_spear = (is_aiming && Input.GetMouseButtonDown(0));
            animator.SetBool("aiming", is_aiming); // dampen the animation to the target animation
            if (throw_spear)
            {
                animator.SetTrigger("throw");
            }
        }
        else
        {
            //hunter.SetConjureTransition(1);
            animator.SetBool("aiming", false);
            aim_canvas.gameObject.SetActive(false);
        }
        //if(conjure_transition == 1)
    }

    private IEnumerator Cloak()
    {
        float duration = 1f; // 1 second
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            disappear_transition = totalTime;
            disappear_transition = Mathf.Clamp(disappear_transition, 0, 1);
            body_mat.SetFloat(FADE_NAME, disappear_transition);
            yield return null;
        }
        disappear_transition = 1;
    }
    private IEnumerator Decloak()
    {
        float duration = 0f; 
        float totalTime = 1f; // 1 second
        while (totalTime >= duration)
        {
            totalTime -= Time.deltaTime;
            disappear_transition = totalTime;
            disappear_transition = Mathf.Clamp(disappear_transition, 0, 1);
            body_mat.SetFloat(FADE_NAME, disappear_transition);
            yield return null;
        }
        disappear_transition = 0;
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            float jump_velocity = Mathf.Sqrt(-2 * gravity * jump_height);
            velocity_y = jump_velocity;
        }
    }
    
    private float GetModifiedSmoothTime(float smooth_time)
    {
        if (controller.isGrounded)
        {
            return smooth_time;
        }
        if(air_control_percent == 0)
        {
            return float.MaxValue;
        }
        return smooth_time / air_control_percent;
    }

    [PunRPC]
    public void Disappear(bool is_crouching) // send material data swap over the network
    {
        if (is_crouching)
        {
            skin.material = disappear_mat;
            disappear_transition += Time.deltaTime;
            disappear_transition = Mathf.Clamp(disappear_transition, 0, 1);
            skin.material.SetFloat(FADE_NAME, disappear_transition);
        }
        else
        {
            disappear_transition -= Time.deltaTime;
            disappear_transition = Mathf.Clamp(disappear_transition, 0, 1);
            skin.material.SetFloat(FADE_NAME, disappear_transition);
            if (disappear_transition <= 0)
            {
                skin.material = body_mat;
            }
        }
    }

    //public void SetConjureTransition(float val)
    //{
    //    conjure_transition = val;
    //}
}
