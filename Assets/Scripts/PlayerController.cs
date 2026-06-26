using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float currentHP = 100f;
    public float maxHP = 100f;
    public Slider hpBar;

    public float[] laneXPositions = new float[] { -2f, 0f, 2f };
    private int currentLane = 1;

    public Vector3 playerPosition; 
    public float laneSwitchSpeed = 5f;

    public float jumpForce = 5f;
    public float gravity = 9.8f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;
    private float groundY = -3f; 

    private float regenTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private float hitTimer = 0f;
    public float hitDuration = 0.2f; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerPosition = new Vector3(laneXPositions[currentLane], groundY, 0f);

        if (hpBar != null)
        {
            hpBar.minValue = 0f;
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }
    }

    void Update()
    {
        KeyMovement();
        PhysicsAndMovement();
        Regeneration();
        HitColor(); 
        UpdateUI();
    }

    void KeyMovement()
    {
        if (!isJumping)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentLane > 0) currentLane--;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentLane < 2) currentLane++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
        }
    }

    void PhysicsAndMovement()
    {
        float targetX = laneXPositions[currentLane];
        playerPosition.x = Mathf.MoveTowards(playerPosition.x, targetX, laneSwitchSpeed * Time.deltaTime * 50f);

        if (isJumping)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            playerPosition.y += verticalVelocity * Time.deltaTime;

            if (playerPosition.y <= groundY)
            {
                playerPosition.y = groundY;
                verticalVelocity = 0f;
                isJumping = false;
            }
        }

        float zDepth = Mathf.Max(0.1f, CameraComponent.focalLength + playerPosition.z); 
        float perspective = CameraComponent.focalLength / zDepth; 

        float calculatedX = playerPosition.x * perspective;

        float ground = Mathf.Lerp(groundY, 6f, 1f - perspective); 
        
        float jumpHeightOffset = playerPosition.y - groundY;
        float calculatedY = ground + jumpHeightOffset;

        transform.position = new Vector2(calculatedX, calculatedY);

        transform.localScale = Vector3.one * perspective * 1.2f;
    }

    void Regeneration()
    {
        if (currentHP < maxHP)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f)
            {
                currentHP = Mathf.Min(maxHP, currentHP + 1f);
                regenTimer = 0f;
            }
        }
    }

    void HitColor()
    {
        if (spriteRenderer == null) return;

        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
            
            float colorchange = hitTimer / hitDuration;
            spriteRenderer.color = Color.Lerp(Color.white, Color.red, colorchange);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    void UpdateUI()
    {
        if (hpBar != null)
        {
            hpBar.value = Mathf.MoveTowards(hpBar.value, currentHP, 100f * Time.deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Max(0f, currentHP - amount);
        
        hitTimer = hitDuration; 
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; 
        }

        if (currentHP <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    public int GetCurrentLane() => currentLane;
    public bool IsJumping() => isJumping;
}