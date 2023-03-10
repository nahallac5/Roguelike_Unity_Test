using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    [Header("Time")]
    [SerializeField] private float basetime = 0.1f;
    [SerializeField] private float delayTime; //Read-only
    [SerializeField] private bool isPlayerTurn = true;

    [Header("Entities")]
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities = new List<Entity>();
    [SerializeField] private List<Actor> actors = new List<Actor>();

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;

    public bool IsPlayerTurn {get => isPlayerTurn; }
    public List<Entity> Entities { get => entities; }
    public List<Actor> Actors { get => actors; }
    public Sprite DeadSprite { get => deadSprite; }

    // Start is called before the first frame update
    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartTurn() 
    {
        //Debug.Log($"{entities[entityNum].name} starts its turn!");
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = true;
        }
        else 
        {
            if (actors[actorNum].GetComponent<HostileEnemy>())
            {
                actors[actorNum].GetComponent<HostileEnemy>().RunAI();
            }
            else
            {
                Action.SkipAction();
            }
        }
    }

    public void EndTurn() 
    {
        //Debug.Log($"{entities[entityNum].name} ends its turn!");
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = false;
        }
        if (actorNum == actors.Count - 1)
        {
            actorNum = 0;
        }
        else
        {
            actorNum++;
        }

        StartCoroutine(TurnDelay());
    }

    private IEnumerator TurnDelay() 
    {
        yield return new WaitForSeconds(delayTime);
        StartTurn();
    }

    public void AddEntity(Entity entity) 
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }

        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity) 
    {
        entity.gameObject.SetActive(false);
        entities.Remove(entity);
    }

    public void AddActor(Actor actor) 
    {
        actors.Add(actor);
        delayTime = SetTime();
    }

    public void InsertActor(Actor actor, int index) 
    {
        actors.Insert(index, actor);
        delayTime = SetTime();
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
        delayTime = SetTime();
    }

    public Actor GetBlockingActorAtLocation(Vector3 location) 
    {
        foreach (Actor actor in Actors)
        {
            if (actor.BlocksMovement && actor.transform.position == location)
            {
                return actor;
            }
        }
        return null;
    }

    private float SetTime() => basetime / actors.Count;
}
