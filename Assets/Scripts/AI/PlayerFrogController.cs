using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldOfFrogs.Input;
using UnityEngine.InputSystem;
using System;

namespace WorldOfFrogs.AI
{
    public class PlayerFrogController : MonoBehaviour
    {
        const float Epsilon = 1e-6f;

        private PlayerInput inputSystem;
        private Rigidbody2D frogBody;
        private Transform head;

        [SerializeField] private GameObject tonguePrefab;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float jumpDistance;
        [SerializeField] private float rotationDelay;

        private bool rotationLock;
        private bool jumpLock;
        private float currentRotation;
        private float jumpTimer;
        private Vector2 direction;
        private bool isJumping;

        void Start()
        {
            inputSystem = GetComponent<PlayerInput>();
            frogBody = GetComponent<Rigidbody2D>();
            head = GetComponentInChildren<Transform>();
        }

        /// <summary>
        /// Occurs when trying to move the frog.
        /// </summary>
        /// <param name="value">New value that is passed from the input source.</param>
        private void OnMove(InputValue value)
        {
            direction = value.Get<Vector2>();
        }

        /// <summary>
        /// Updates the rotation according to the specified direction.
        /// </summary>
        /// <param name="direction">The direction to rotate the frog to.</param>
        private void UpdateRotation(Vector2 direction)
        {
            if (!isJumping && !rotationLock)
            {
                float newRotation = GetRotation(direction);
                if (Mathf.Abs(currentRotation - newRotation) > Epsilon)
                {
                    currentRotation = newRotation;
                    rotationLock = true;
                    StartCoroutine(Rotate());
                }
            }
        }

        /// <summary>
        /// Performs the smooth rotation to the specified rotation.
        /// </summary>
        private IEnumerator Rotate()
        {
            bool oldLock = jumpLock;
            jumpLock = true;
            float time = 0;
            float oldRotation = frogBody.rotation;
            while (time < rotationDelay)
            {
                time += Time.deltaTime;
                frogBody.rotation = Mathf.LerpAngle(oldRotation, currentRotation, time / rotationDelay);
                yield return new WaitForEndOfFrame();
            }
            rotationLock = false;
            jumpLock = oldLock;
        }

        /// <summary>
        /// Checks if the frog should jump.
        /// </summary>
        private void PrepareForJump()
        {
            if (jumpTimer < 0 && !isJumping && !jumpLock)
            {
                isJumping = true;
                StartCoroutine(Jump());
            }
        }

        /// <summary>
        /// Jumps the frog.
        /// </summary>
        private IEnumerator Jump()
        {
            Debug.Log("Started jump");
            float targetTime = jumpDistance / jumpSpeed;
            frogBody.velocity = jumpSpeed * direction.normalized;
            transform.localScale *= 1.1f;
            yield return new WaitForSeconds(targetTime);
            transform.localScale /= 1.1f;
            frogBody.velocity = Vector2.zero;
            isJumping = false;
            jumpTimer = jumpCooldown;
            Debug.Log("Ended jump");
        }

        /// <summary>
        /// Gets the rotation angle for the frog.
        /// </summary>
        /// <param name="direction">The direction of the forward frog side.</param>
        /// <returns>Angle in degrees for the specified direction.</returns>
        private float GetRotation(Vector2 direction)
        {
            return Vector2.SignedAngle(Vector2.up, direction);
        }

        private void OnShoot()
        {
            Instantiate(tonguePrefab, head.transform.position, head.transform.rotation);
        }
        
        void Update()
        {
            if (direction == Vector2.zero)
            {
                return;
            }
            if (!isJumping)
            {
                UpdateRotation(direction);
                PrepareForJump();
            }
            jumpTimer -= Time.deltaTime;
        }
    }
}