using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float dashDistance = 10f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] LayerMask groundLayer;
    Quaternion targetRotation;
    CameraController cameraController;
    Rigidbody rb;
    bool isGrounded;
    bool canDash = true;
    float originalMoveSpeed;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        rb = GetComponent<Rigidbody>();
        originalMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v);
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput;

        if (moveAmount > 0)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash(moveDir));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }

    private IEnumerator Dash(Vector3 direction)
    {
        canDash = false;
        moveSpeed *= dashDistance / dashDuration;

        yield return new WaitForSeconds(dashDuration);

        moveSpeed = originalMoveSpeed;

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }
}
