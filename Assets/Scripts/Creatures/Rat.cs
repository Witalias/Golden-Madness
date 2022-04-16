using System;
using System.Collections;
using UnityEngine;

public class Rat : MonoBehaviour
{
    [SerializeField] private float obstacleCheckBetweenTime = 0.2f;
    [SerializeField] private float stayBetweenTimeMin = 1;
    [SerializeField] private float stayBetweenTimeMax = 5;
    [SerializeField] private float stayTimeMin = 1;
    [SerializeField] private float stayTimeMax = 5;
    [SerializeField] private float playerCheckBetweenTime = 0.5f;
    [SerializeField] private float aggressiveModeTime = 5f;
    [SerializeField] private float aggressiveSpeed = 1.5f;

    private Creature creature;
    private SpriteRenderer sprite;

    private Coroutine temporarilyStop;
    private Coroutine activateAggressiveMode;

    private LayerMask playerMask;
    private float obstacleCheckRadius = 0.01f;
    private float obstacleCheckOffsetX = 0.4f;
    private float obstacleCheckOffsetY = 1;
    private float normalSpeed;
    private bool angry = false;
    private bool isMoving = true;

    private void Awake()
    {
        creature = GetComponent<Creature>();
        sprite = GetComponent<SpriteRenderer>();
        normalSpeed = creature.Speed;
        playerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        StartCoroutine(CheckObstacle());
        temporarilyStop = StartCoroutine(TemporarilyStop());
        StartCoroutine(CheckPlayer());
    }

    private void Update()
    {
        if (isMoving)
            Run();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<Player>();
        if (player)
            creature.ChangeDirectionTowards(player.transform.position);
    }

    private void Run()
    {
        creature.State = States.Walk;
        var direction = transform.right * creature.DirectionValue;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + direction, creature.Speed * Time.deltaTime);
    }

    private void Stop()
    {
        creature.State = States.Idle;
    }

    private IEnumerator CheckObstacle()
    {
        while (true)
        {
            var layer = 1 << 3 | 1 << 6; // 3 - Ground; 6 - Enemies
            if (angry)
                layer = 1 << 3; // Ground only

            var checkingPoint1 = new Vector2(transform.position.x + obstacleCheckOffsetX * creature.DirectionValue, transform.position.y);
            var checkingPoint2 = new Vector2(checkingPoint1.x, checkingPoint1.y - obstacleCheckOffsetY);

            if (Physics2D.OverlapCircleAll(checkingPoint1, obstacleCheckRadius, layer).Length > 0 ||
                (Physics2D.OverlapCircleAll(checkingPoint2, obstacleCheckRadius, layer).Length == 0 && !angry && !creature.Attacked))
                creature.ChangeDirection();
            yield return new WaitForSeconds(obstacleCheckBetweenTime);
        }
    }

    private IEnumerator CheckPlayer()
    {
        while (!angry)
        {
            yield return new WaitForSeconds(playerCheckBetweenTime);
            var startPoint = new Vector2(transform.position.x + obstacleCheckOffsetX * creature.DirectionValue, transform.position.y);
            var layer = 1 << 3 | 1 << 7; // 3 - Ground; 7 - Player
            var raycastHit = Physics2D.Raycast(startPoint, transform.right * creature.DirectionValue, Mathf.Infinity, layer);
            if (raycastHit)
            {
                var player = raycastHit.collider.GetComponent<Player>();
                if (player)
                    StartCoroutine(ActivateAggressiveMode());
            }
        }
    }

    private IEnumerator TemporarilyStop()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(stayBetweenTimeMin, stayBetweenTimeMax));
        isMoving = false;
        Stop();

        yield return new WaitForSeconds(UnityEngine.Random.Range(stayTimeMin, stayTimeMax));
        isMoving = true;
        temporarilyStop = StartCoroutine(TemporarilyStop());
    }

    private IEnumerator ActivateAggressiveMode()
    {
        StopCoroutine(temporarilyStop);
        angry = true;
        isMoving = true;
        creature.Speed = aggressiveSpeed;

        yield return new WaitForSeconds(aggressiveModeTime);

        angry = false;
        creature.Speed = normalSpeed;
        temporarilyStop = StartCoroutine(TemporarilyStop());
        StartCoroutine(CheckPlayer());
    }
}
