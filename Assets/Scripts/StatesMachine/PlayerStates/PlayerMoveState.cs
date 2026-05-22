using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IdleTime = Animator.StringToHash("IdleTime");
    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private const float AnimatorBlendValueMin = 0f;
    private const float AnimatorBlendValueMax = 1f;
    private const float MovementMagnitudeMin = 0.01f;
    private float _idleTimer = 0;
    private float _verticalVelocity;
    private float _currentSpeed;
    private bool _isRunning;
    private Vector3 _currentMovement;

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        _currentSpeed = stateMachine.MovementSpeed;
        stateMachine.InputReader.RunEvent += Run;
    }

    public override void Tick(float deltaTime)
    {
        Vector2 input = stateMachine.InputReader.MovementValue;
        Vector3 targetMovement = CalculateMovement(input);
        Vector3 horizontalMovement = SmoothMovement(targetMovement, deltaTime);
        float speed = SmoothSpeed(deltaTime);
        Vector3 movement = ApplyGravity(horizontalMovement * speed, deltaTime);

        stateMachine.Controller.Move(movement * deltaTime);
        if (input.sqrMagnitude < MovementMagnitudeMin)
        {
            
            _idleTimer += deltaTime;
            float idleBlendDuration = Mathf.Max(stateMachine.IdleBlendDuration, 0.01f);
            float idleBlend = Mathf.SmoothStep(AnimatorBlendValueMin, AnimatorBlendValueMax, Mathf.Clamp01(_idleTimer / idleBlendDuration));

            stateMachine.Animator.SetFloat(IdleTime, idleBlend); 
            stateMachine.Animator.SetFloat(Speed, horizontalMovement.magnitude);
            stateMachine.Animator.SetFloat(WalkSpeed, AnimatorBlendValueMin);
            
            return;
        }

        _idleTimer = 0;
        stateMachine.Animator.SetFloat(IdleTime, _idleTimer);
        stateMachine.Animator.SetFloat(Speed, horizontalMovement.magnitude);
        stateMachine.Animator.SetFloat(WalkSpeed, GetWalkRunBlend());
        
        FaceMovementDirection(horizontalMovement, deltaTime);

    }

    public override void Exit()
    {
        _currentMovement = Vector3.zero;
        stateMachine.InputReader.RunEvent -= Run;

    }

    private void FaceMovementDirection(Vector3 movementDirection, float deltaTime)
    {
        if (movementDirection.sqrMagnitude < MovementMagnitudeMin)
        {
            return;
        }

        stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, Quaternion.LookRotation(movementDirection), deltaTime * stateMachine.RotationSpeed);

    }

    private Vector3 CalculateMovement(Vector2 input)
    {
        Transform cameraTransform = stateMachine.MainCameraTransform;
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : stateMachine.transform.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : stateMachine.transform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
        
        return Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
    }

    private Vector3 SmoothMovement(Vector3 targetMovement, float deltaTime)
    {
        _currentMovement = Vector3.MoveTowards(_currentMovement, targetMovement, stateMachine.MovementAcceleration * deltaTime);

        return _currentMovement;
    }

    private Vector3 ApplyGravity(Vector3 movement, float deltaTime)
    {
        if (stateMachine.Controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = stateMachine.GroundedGravity;
        }
        else
        {
            _verticalVelocity += stateMachine.Gravity * deltaTime;
        }

        movement.y = _verticalVelocity;
        return movement;
    }

    private float SmoothSpeed(float deltaTime)
    {
        float targetSpeed = _isRunning ? stateMachine.RunSpeed : stateMachine.MovementSpeed;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, stateMachine.MovementAcceleration * deltaTime);

        return _currentSpeed;
    }

    private float GetWalkRunBlend()
    {
        if (stateMachine.RunSpeed <= stateMachine.MovementSpeed)
        {
            return _isRunning ? AnimatorBlendValueMax : AnimatorBlendValueMin;
        }

        return Mathf.InverseLerp(stateMachine.MovementSpeed, stateMachine.RunSpeed, _currentSpeed);
    }

    private void Run(bool isRunning)
    {
        _isRunning = isRunning;
    }
    
}
