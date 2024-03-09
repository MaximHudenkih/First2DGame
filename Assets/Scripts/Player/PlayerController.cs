using Scripts.Animators;
using Scripts.Configs;
using Scripts.Enums;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        [Inject] private PlayerConfig _playerConfig;
        [SerializeField] private GroundCheck _groundCheck;
        
        private Rigidbody2D _rigidbody2D;
        private CustomAnimator _animator;
        private AudioSource _audioSource;
        private CapsuleCollider2D _colliderSize;

        private float _horizontalAxis;
        private float _verticalAxis;
        private bool _isFacingRight = true;

        private bool _stopWalk = false;

        private Vector2 _defaultColliderSize;

        private void Awake()
        {
            _animator = GetComponent<CustomAnimator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _colliderSize = GetComponent<CapsuleCollider2D>();
            
            _defaultColliderSize = _colliderSize.size;
        }

        private void FixedUpdate()
        {
            if (_stopWalk == false)
            {
                _horizontalAxis = Input.GetAxis("Horizontal");
                
                if (_groundCheck.IsGround)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        Run();
                    }
                    else if (Input.GetKey(KeyCode.W))
                    {
                        Jump();
                    }
                    else
                    {
                        Move();
                    }
                }
                else if (_groundCheck.IsGround == false)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        Run();
                    }
                    else
                    {
                        Move();
                        Landing();
                    }
                }
                FlipX();
            }
        }
        
        private void Move()
        {
            _animator.SetMoveSpeed(_rigidbody2D.velocity.magnitude);
            
            _rigidbody2D.velocity = new Vector2(_horizontalAxis * _playerConfig.WalkSpeed, _rigidbody2D.velocity.y);
            _colliderSize.size = new Vector2(_defaultColliderSize.x, _defaultColliderSize.y);
        }
        private void Run()
        {
            _animator.Play(EAnimationType.Run);
            
            _rigidbody2D.velocity = new Vector2(_horizontalAxis * _playerConfig.WalkSpeed * 1.5F, _rigidbody2D.velocity.y);
            _colliderSize.size = new Vector2(0.5F, 0.4F);
        }
        
        private void Jump()
        {
            _audioSource.clip = _playerConfig.JumpAudioClip;
            _audioSource.Play();
            
            _animator.Play(EAnimationType.Jump);
            _rigidbody2D.AddForce(Vector2.up * _playerConfig.JumpForce, ForceMode2D.Impulse);
        }
        
        private void Landing()
        {
            if (_rigidbody2D.velocity.y < -.1f)
            {
                _audioSource.clip = _playerConfig.FalAudioClip;
                _audioSource.Play();
                
                _animator.Play(EAnimationType.Landing);
            }
        }
        
        public void Stay(bool value)
        {
            _stopWalk = value;
        }
        
        private void FlipX() // <- MAYBE UTILS?
        {
            if (_isFacingRight && _horizontalAxis < 0f || !_isFacingRight && _horizontalAxis > 0f)
            {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }
}