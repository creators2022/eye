using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

namespace LHProject.Eye.Game
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rigidBody2D;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField]
        private float jumpForce = 60.0f;

        [SerializeField]
        private int onGroundTime = 200;

        [SerializeField]
        private bool jumpColorCahange = false;

        private IMovable movable;

        private bool isJumping = false;

        private bool onGround = false;

        private void Start()
        {
            movable = GetComponent<IMovable>();

            // 着地したときにUnitが発行されるObservable
            var onLandObservable = this.OnCollisionEnter2DAsObservable()
                                       .Where(other => other.gameObject.CompareTag("Ground"))
                                       .Select(_ => Unit.Default)
                                       .Publish();
            onLandObservable.Connect();

            onLandObservable.Subscribe(_ => OnLand());

            // 着地してから300ms後にジャンプする
            onLandObservable.Delay(TimeSpan.FromMilliseconds(onGroundTime))
                            .Where(_ => !isJumping)
                            .Subscribe(_ => Jump());

            this.OnCollisionExit2DAsObservable()
                .Where(other => other.gameObject.CompareTag("Ground"))
                .Subscribe(_ => OnTakeOff());
        }

        private void OnEnable()
        {
            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMoveStop;
        }

        private void OnDisable()
        {
            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMoveStop;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<Vector2>();
            movable.SetDirection(value);
        }

        private void OnMoveStop(InputAction.CallbackContext obj)
        {
            movable.SetDirection(Vector2.zero);
        }

        private void OnLand()
        {
            isJumping = false;
            onGround = true;

            if (jumpColorCahange)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void OnTakeOff()
        {
            onGround = false;
        }

        private void Jump()
        {
            rigidBody2D.AddForce(transform.up * jumpForce);
            isJumping = true;

            if (jumpColorCahange)
            {
                spriteRenderer.color = new Color(0.75f, 0.25f, 0.25f);
            }
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUILayout.Label($"velocity: {rigidBody2D.velocity}");
            GUILayout.Label($"isJumping: {isJumping}");
            GUILayout.Label($"onGround: {onGround}");
        }
#endif
    }
}
