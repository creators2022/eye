using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace LHProject.Eye.Game
{
    public sealed class Movable : MonoBehaviour, IMovable
    {
        [SerializeField]
        private Rigidbody2D rigidBody2D;

        [SerializeField]
        private float maxWalkSpeed = 3.0f;

        [SerializeField]
        private float walkForce = 30.0f;

        private Vector2 direction;

        private void Start()
        {
            direction = Vector2.zero;

            this.UpdateAsObservable()
                .Subscribe(_ => rigidBody2D.AddForce(Vector2.right * ((direction.x * maxWalkSpeed) - rigidBody2D.velocity.x) * walkForce));
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;

            if (transform.right.x * direction.x < 0)
            {
                transform.Rotate(new Vector3(0, 180.0f, 0));
            }
        }
    }
}
