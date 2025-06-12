using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Character Character;
    private CharacterAnimation Animation;
    private SpriteRenderer SpriteRenderer;
    private Rigidbody2D Rigidbody;

    public float pixelMoveSpeed { get; private set; }


    //private Animator animator;
    //[SerializeField] private Animator[] hologramAnimatorArr;
    //[SerializeField] private SpriteRenderer[] hologramRendererArr;
    //[SerializeField] private bool isHologramAnimate = false;

    public Vector2 MoveDirection => moveDirection;
    private Vector2 moveDirection;
    public bool IsFlipped => isFlipped;
    private bool isFlipped;

    public bool isKnockback; // 몬스터에게 피격 시 넉백 동안 true

    // CharacterAttack이 공격 방향에 참고하는 변수
    public Vector2 LastMoveDirection { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Character = GetComponent<Character>();
        Animation = GetComponent<CharacterAnimation>();

    }

    public void Init(float moveSpeed)
    {
        CalculatePixelSpeed();
        InitLastMoveDirection();
    }

    private void InitLastMoveDirection()
    {
        LastMoveDirection = Vector2.left;
    }

    private void CalculatePixelSpeed()
    {
        pixelMoveSpeed = (Character.player.moveSpeed * 25f) / 100f;
    }

    public void StartMove(Vector2 joystickDir)
    {
        if (isKnockback) return;

        moveDirection = joystickDir.normalized;
        if (moveDirection != Vector2.zero)
            LastMoveDirection = moveDirection;
        Rigidbody.linearVelocity = moveDirection * pixelMoveSpeed;

        isFlipped = moveDirection.x > 0;
        SpriteRenderer.flipX = isFlipped;
        Animation.SetBool("move", true);

        //UpdateHologramMoveAndFlip(true, moveDirection.x > 0);
    }

    public void EndMove()
    {
        Rigidbody.linearVelocity = Vector2.zero;
        Animation.SetBool("move", false);

        //UpdateHologramMoveAndFlip(false, moveDirection.x > 0);
    }
    public void SetMoveSpeed(float moveSpeed)
    {
        Character.player.moveSpeed = moveSpeed;
        CalculatePixelSpeed();
    }

    //private void UpdateHologramMoveAndFlip(bool isMoving, bool flipX)
    //{
    //    if (!isHologramAnimate) return;

    //    foreach (Animator hologramAnimator in hologramAnimatorArr)
    //    {
    //        if (hologramAnimator != null)
    //            hologramAnimator.SetBool("move", isMoving);
    //    }

    //    foreach (SpriteRenderer hologramRenderer in hologramRendererArr)
    //    {
    //        if (hologramRenderer != null)
    //            hologramRenderer.flipX = flipX;
    //    }
    //}
}
