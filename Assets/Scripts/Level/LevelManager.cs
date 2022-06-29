using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Level building
    GameObject player;
    public Vector2 playerSpawnLocation;
    float startingFloorHeight = 8;
    float currentLevel;
    float currentFloorY;
    public int floorHeight = 15;
    float characterHeightOffset = 1.57f;
    float ascenderOffset = 2.5f;
    [SerializeField] float fourthWallOffset = 10f;
    float deleteDelay = 1f;

    // Level spawning logic
    Vector2 ascenderPosition;
    GameObject lockedAscenderInstance;
    bool validSpawn;
    int minFreeSpawns = 2;
    float obstacleSpawnChance = 0.1f;
    float powerUpSpawnChance = 0.6f;
    float powerUpSpawnOffsetMax = 2f;

    // Enemy spawning logic
    int numberOfEnemies;
    int minNumEnemies = 1, maxNumEnemies = 2;
    List<GameObject> aliveEnemies;
    public event EventHandler<OnEnemyDiedArgs> OnEnemyEliminated;
    public event EventHandler OnAllEnemiesEliminated;

    // Prefabs and names
    GameObject floorGroup;
    public Transform SpawnPointsParent;
    Transform[] SpawnPoint;
    List<int> occupiedSpawns;
    int ascenderSpawnPointIndex;
    public GameObject emptyGameObjectPrefab;
    public GameObject floorPrefab;
    public GameObject lockedAscenderPrefab;
    public GameObject ascenderPrefab;
    public GameObject usedAscenderPrefab;
    public GameObject levelNumberPlaquePrefab;
    public GameObject enemyPrefab;
    public GameObject fourthWallPrefab;
    public GameObject walls;
    public SpriteRenderer[] wallSpriteRenderers;
    [SerializeField] private GameObject[] Obstacles;
    [SerializeField] private GameObject[] _powerUps;
    public Transform towerBackground;
    public Transform skyBackground;
    string floorName = "Level {0} Floor";
    string ceilingName = "Level {0} Ceiling";
    string lockedAscenderName = "Level {0} Locked Ascender";
    string ascenderName = "Level {0} Ascender";
    string usedAscenderName = "Level {0} Used Ascender From Level {1}";
    string bottomFourthWallName = "Level {0} Bottom Fourth Wall";
    string topFourthWallName = "Level {0} Top Fourth Wall";
    string levelNumberPlaqueName = "Level {0} Plaque";
    string enemyName = "Level {0} Enemy #{1}";
    string powerUpName = "Level {0} {1} Powerup";

    void Awake()
    {
        currentFloorY = startingFloorHeight;
        int numSpawnPoints = SpawnPointsParent.childCount;
        SpawnPoint = new Transform[numSpawnPoints];
        occupiedSpawns = new List<int>();
        aliveEnemies = new List<GameObject>();
        // Load spawn points
        for (int i = 0; i < numSpawnPoints; i++) { SpawnPoint[i] = SpawnPointsParent.GetChild(i); }
        for (int j = 0; j < SpawnPoint.Length; j++) { Transform _spawnPoint = SpawnPoint[j]; }
        OnAllEnemiesEliminated += OnAllEnemiesEliminated_UnlockAscender;
    }

    private void Update() {
        if (aliveEnemies.Count == 0)
        {
            OnAllEnemiesEliminated?.Invoke(this, EventArgs.Empty);
            OnAllEnemiesEliminated -= OnAllEnemiesEliminated_UnlockAscender;
        }

    }

    public void GenerateFirstLevel(int startingLevel)
    {
        this.currentLevel = startingLevel;

        // Spawn player in random position
        player = FindObjectOfType<GameManager>().Player;
        playerSpawnLocation = findValidSpawn(player, objName: "Player");
        player.transform.position = playerSpawnLocation;

        // Group the level assets
        floorGroup = Instantiate(emptyGameObjectPrefab);
        floorGroup.name = "Level " + currentLevel;
        SpawnFloor();
        SpawnCeiling();
        SpawnUsedAscender(firstFloor: true);  // TEMP
        SpawnLockedAscender();
        currentFloorY += floorHeight;
    }

    public void GenerateNewLevel(int currentLevel)
    {
        occupiedSpawns.Clear();
        aliveEnemies.Clear();
        OnAllEnemiesEliminated += OnAllEnemiesEliminated_UnlockAscender;
        Debug.Log("-------------------------------");
        this.currentLevel = currentLevel;

        // Group the level assets
        floorGroup = Instantiate(emptyGameObjectPrefab);
        floorGroup.name = "Level " + currentLevel;
        SpawnFloor();
        SpawnCeiling();
        SpawnUsedAscender();
        SpawnLockedAscender();
        SpawnLevelNumberPlaque();

        SpawnObstacles();
        SpawnEnemies();
        SpawnPowerups();

        riseBuildingAndBackground();
        currentFloorY += floorHeight;
    }


    void SpawnFourthWall(bool floor, Vector2 parentPos) {
        Vector2 fourthWallPos;
        if (floor) {
            fourthWallPos = new Vector2(parentPos.x, parentPos.y - fourthWallOffset);
        } else {
            fourthWallPos = new Vector2(parentPos.x, parentPos.y + fourthWallOffset-1);
        }

        GameObject fourthWall = Instantiate(fourthWallPrefab, fourthWallPos, Quaternion.identity);
        if (floor)
        {
            fourthWall.name = string.Format(bottomFourthWallName, currentLevel);
        } else
        {
            fourthWall.name = string.Format(topFourthWallName, currentLevel);
        }
       
        fourthWall.transform.SetParent(floorGroup.transform);
    }
    
    void SpawnLevelNumberPlaque() 
    {

    }
    
    void riseBuildingAndBackground()
    {
        // Stretch the walls up
        foreach (SpriteRenderer sr in walls.GetComponentsInChildren<SpriteRenderer>())
        {
            float currentWallSize = sr.size.y;
            int stretchAmount = 50;
            sr.size = new Vector2(sr.size.x, currentWallSize + stretchAmount);
        }
                
        ascendElement(towerBackground);
        ascendElement(skyBackground);
        ascendElement(SpawnPointsParent);
    }

    void ascendElement(Transform transformObject)
    {
        transformObject.position = new Vector2(transformObject.position.x, transformObject.position.y + floorHeight);
    }

    public void TeleportPlayer()
    {
        player.transform.position = new Vector2(player.transform.position.x, currentFloorY + characterHeightOffset);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Reset velocity 
    }


    void SpawnFloor()
    {
        GameObject floor = Instantiate(floorPrefab, new Vector3(0f, currentFloorY, 0f), Quaternion.identity);
        floor.name = string.Format(floorName, currentLevel);
        floor.transform.SetParent(floorGroup.transform);

        // Under floor, spawn a fourth wall
        SpawnFourthWall(floor: true, parentPos: floor.transform.position);
    }


    void SpawnCeiling()
    {
        GameObject ceiling = Instantiate(floorPrefab, new Vector3(0f, currentFloorY + floorHeight, 0f), Quaternion.identity);
        ceiling.name = string.Format(ceilingName, currentLevel);
        ceiling.transform.SetParent(floorGroup.transform);

        SpawnFourthWall(floor: false, parentPos: ceiling.transform.position);
    }

    private void SpawnLockedAscender()
    {
        // Spawn a randomly-placed locked ascender in the current level
        ascenderPosition = findValidSpawn(ascenderPrefab, objName: "LockedAscender", isAscender: true);
        GameObject lockedAscender = Instantiate(lockedAscenderPrefab, ascenderPosition, Quaternion.identity);
        lockedAscenderInstance = lockedAscender;
        lockedAscender.name = string.Format(ascenderName, currentLevel);
        lockedAscender.transform.SetParent(floorGroup.transform);
    }

    private void SpawnUsedAscender(bool firstFloor = false)
    {
        // Spawn an ascender behind the player to give the illusion that they used the ascender
        int usedAscenderXPosition = -1;
        if (firstFloor)
        {
            usedAscenderXPosition = (int) playerSpawnLocation.x;
        }
        else
        {
            usedAscenderXPosition = (int) SpawnPoint[ascenderSpawnPointIndex].transform.position.x;
            occupiedSpawns.Add(ascenderSpawnPointIndex);
        }
        GameObject instance = Instantiate(usedAscenderPrefab, new Vector2(usedAscenderXPosition, currentFloorY + ascenderOffset), Quaternion.identity);
        instance.name = string.Format(usedAscenderName, currentLevel, currentLevel-1);
        instance.transform.SetParent(floorGroup.transform);
    }

    public void OnAllEnemiesEliminated_UnlockAscender(object sender, EventArgs e)
    {
        unlockAscender();
    }

    public void unlockAscender()
    {
        if (lockedAscenderInstance == null) { return; }
        // Replace locked ascender with unlocked ascender
        GameObject unlockedAscender = Instantiate(ascenderPrefab, new Vector2(lockedAscenderInstance.transform.position.x, lockedAscenderInstance.transform.position.y), Quaternion.identity);
        Destroy(lockedAscenderInstance);
        Debug.Log("Unlocked the ascender for floor " + currentLevel);
    }

    private void SpawnObstacles() {
        int numPotentialObstacleSpawns = SpawnPoint.Length - occupiedSpawns.Count;
        if (numPotentialObstacleSpawns <= minFreeSpawns) { return; } // Do not spawn more objects if there are too many in the scene
        
        for (int i = 0; i < numPotentialObstacleSpawns; i++)
        {
            float seed = UnityEngine.Random.Range(0f, 1f);
            if (seed < obstacleSpawnChance)
            {
                // Spawn random obstacle
                int randomObstacleSeed = UnityEngine.Random.Range(0, Obstacles.Length);
                GameObject randomObstacle = Obstacles[randomObstacleSeed];
                Vector3 obstacleSpawnPosition = findValidSpawn(randomObstacle, randomObstacle.name);
                GameObject instance = Instantiate(randomObstacle, obstacleSpawnPosition, Quaternion.identity);
                instance.transform.SetParent(floorGroup.transform);
            }
        }
    }

    private void SpawnPowerups() {
        bool spawnPowerup = UnityEngine.Random.Range(0f, 1f) < powerUpSpawnChance;
        if (spawnPowerup)
        {
            // Choose a random powerup
            GameObject randomPowerUp = _powerUps[UnityEngine.Random.Range(0,_powerUps.Length)];
            float randomPowerupOffset = UnityEngine.Random.Range(0, powerUpSpawnOffsetMax+1);
            Vector2 powerUpSpawnPosition = findValidSpawn(randomPowerUp, randomPowerUp.name, additionalOffset: randomPowerupOffset);
            GameObject instance = Instantiate(randomPowerUp, powerUpSpawnPosition, Quaternion.identity);
            instance.name = string.Format(currentLevel.ToString(), randomPowerUp.name);
            instance.transform.SetParent(floorGroup.transform);
        }
    }

    private void SpawnEnemies()
    {
        int numberOfEnemiesToSpawn = UnityEngine.Random.Range(minNumEnemies, maxNumEnemies+1);
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            Vector2 enemySpawnPosition = findValidSpawn(enemyPrefab, objName: "Enemy");
            if (enemySpawnPosition != Vector2.zero)
            {
                GameObject enemy = Instantiate(enemyPrefab, enemySpawnPosition, Quaternion.identity);
                enemy.GetComponent<EnemyAI>().OnEnemyDied += OnEnemyEliminated_RemoveFromList;
                enemy.name = string.Format(enemyName, currentLevel, i.ToString());
                enemy.transform.SetParent(floorGroup.transform);
                aliveEnemies.Add(enemy);
            }
        }
    }

    float calculateOffset(SpriteRenderer sr)
    {
        float srHeight = sr.bounds.size.y;
        return srHeight/2;
    }

    private void OnEnemyEliminated_RemoveFromList(object sender, OnEnemyDiedArgs args)
    {
        aliveEnemies.Remove(args.enemyObject);
    }

    private Vector2 findValidSpawn(GameObject prefab, string objName, float additionalOffset=0f, bool isAscender = false)
    {
        Transform spawnPoint;
        int spawnPointIndex = -1;
        int spawnAttempts = 0;
        validSpawn = false;
        while (!validSpawn)
        {
            spawnPointIndex = UnityEngine.Random.Range(0, SpawnPointsParent.childCount);
            if (!occupiedSpawns.Contains(spawnPointIndex)) 
            {
                validSpawn = true;
                if (isAscender) { 
                    ascenderSpawnPointIndex = spawnPointIndex;
                } 
            }
            spawnAttempts++;
            if (AllSpawnPointsOccupied()) { return Vector2.zero; }
        }
        spawnPoint = SpawnPoint[spawnPointIndex];
        Vector2 objectSpawnPosition = new Vector2(spawnPoint.transform.position.x, currentFloorY + calculateOffset(prefab.GetComponent<SpriteRenderer>())+additionalOffset);
        occupiedSpawns.Add(spawnPointIndex);
        return objectSpawnPosition;
    }

    bool AllSpawnPointsOccupied()
    {
        if (occupiedSpawns.Count == SpawnPointsParent.childCount)
            return true;
        else
            return false;
    }


    public void DestroyPreviousLevel()
    {
        for (int i = 0; i < floorGroup.transform.childCount; i++)
        {
            GameObject obj = floorGroup.transform.GetChild(i).gameObject;
            // Destory fourth wall immediately  
            if (obj.tag == "FourthWall")
            {
                Destroy (obj.gameObject);
            }
            
            // Destroy health bars
            if (obj.tag == "Enemy")
            {
                Destroy(obj.GetComponent<HealthManager>().Instance, deleteDelay);
            }
            Destroy(obj.gameObject, deleteDelay);
        }
        Destroy(floorGroup, deleteDelay);
    }
}
