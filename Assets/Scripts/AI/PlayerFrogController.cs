using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldOfFrogs.Input;
using UnityEngine.InputSystem;

namespace WorldOfFrogs.AI
{
    public class PlayerFrogController : MonoBehaviour
    {
        private PlayerInput inputSystem;
        private Rigidbody2D frogBody;
        private Transform head;

        [SerializeField] private GameObject tonguePrefab;

        // Start is called before the first frame update
        void Start()
        {
            inputSystem = GetComponent<PlayerInput>();
            frogBody = GetComponent<Rigidbody2D>();
            head = GetComponentInChildren<Transform>();
        }

        private void OnMove(InputValue value)
        {
            var direction = value.Get<Vector2>();
            if (direction == Vector2.zero)
                return;
            frogBody.velocity = direction;
            if (direction == Vector2.down)
            {
                frogBody.SetRotation(180);
            }
            else
            {
                frogBody.SetRotation(Quaternion.FromToRotation(Vector3.up, direction.normalized));
            }
        }

        private void OnShoot()
        {
            Instantiate(tonguePrefab, head.transform.position, head.transform.rotation);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}