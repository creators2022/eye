using System;
using UnityEngine;
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
        private float jumpForce = 60.0f;

        private bool isJumping = false;

        private bool onGround = false;

        private void Start()
        {
            // 着地したときにUnitが発行されるObservable
            var onLandObservable = this.OnCollisionEnter2DAsObservable()
                                       .Where(other => other.gameObject.CompareTag("Ground"))
                                       .Select(_ => Unit.Default)
                                       .Publish();
            onLandObservable.Connect();

            onLandObservable.Subscribe(_ => OnLand());

            // 着地してから300ms後にジャンプする
            onLandObservable.Delay(TimeSpan.FromMilliseconds(300))
                            .Subscribe(_ => Jump());

            this.OnCollisionExit2DAsObservable()
                .Where(other => other.gameObject.CompareTag("Ground"))
                .Subscribe(_ => OnTakeOff());
        }

        private void OnLand()
        {
            isJumping = false;
            onGround = true;

            spriteRenderer.color = Color.white;
        }

        private void OnTakeOff()
        {
            onGround = false;
        }

        private void Jump()
        {
            rigidBody2D.AddForce(transform.up * jumpForce);
            isJumping = true;

            spriteRenderer.color = new Color(0.75f, 0.25f, 0.25f);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUILayout.Label($"isJumping: {isJumping}");
            GUILayout.Label($"onGround: {onGround}");
        }
#endif
    }
}
