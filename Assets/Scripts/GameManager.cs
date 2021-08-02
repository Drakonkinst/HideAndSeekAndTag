/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * This is my own work, and I did not cheat on this assignment.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform World;
    public Transform HunterParent;
    public GameObject HunterPrefab;
    public Transform CollectibleParent;
    public GameObject CollectiblePrefab;
    public GameObject BlankTile;
    public GameObject FilledTile;
    public Player Player;
    public GameObject[] HunterSpawnPoints;
    public Text TimeRemainingDisplay;
    public LoopingAudio HunterAudio;

    public List<Hunter> Hunters = new List<Hunter>();
    public bool ShowHiddenObjects = false;
    public string mapFile = "map";

    // Constants
    public int PlayerSightDistance = 4 + 3; // How far the player can see, in tiles
    public int HunterSightDistance = 4; // How far Hunters can see, in tiles
    public int MaxFootprintAge = 15; // Age until footprint expires, in seconds

    private Grid grid;
    private Vector2 worldBottomLeft;
    private float tileSize;
    private float hidden = 0.0f;

    // Game State
    private List<Point> playerFootprints = new List<Point>();
    private List<Point> hunterFootprints = new List<Point>();
    private List<Collectible> collectibles = new List<Collectible>();
    private Vector2 lastPlayerPos;
    private bool wasTrackingPlayer = false;
    private int timeRemaining;
    private bool isGameOver = false;

    void Awake()
    {
        GameSettings.SetManager(this);
        grid = GridLoader.LoadFromFile(mapFile);
        if(grid == null)
        {
            Debug.LogWarning("Grid failed to load!");
            return;
        }
        grid.RefreshGrid();

        CalculateTileOffsets();
        GenerateWorldTiles();

        // Debug mode
        if(ShowHiddenObjects)
        {
            hidden = 0.5f;
        }
    }
    
    void Start()
    {
        // Different behavior depending on mode
        if (GameSettings.GameMode == GameSettings.Mode.COUNTDOWN)
        {
            timeRemaining = GameSettings.GameDifficulty.TimeNeeded;
        }
        else
        {
            timeRemaining = 0;
        }
        TimeRemainingDisplay.text = "" + timeRemaining;

        SpawnCollectibles(GameSettings.NumCollectibles);
        SpawnHunters();
        GameSettings.HighScoreAchievedLastGame = false;

        InvokeRepeating("UpdateEverySecond", 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        DoHunterBehavior();

        // Update steerables
        UpdateAllSteerables();

        // Reset grid colors
        grid.ResetTileColors();

        // Update player sight;
        UpdatePlayerSight();

        UpdateHunterAudio();
    }

    /* Game Win/Lose Conditions */
    public void PlayerWin()
    {
        if(isGameOver)
        {
            return;
        }

        OnGameOver();
        SceneManager.LoadScene(GameSettings.VictoryScene);
    }

    public void PlayerLose()
    {
        if(isGameOver)
        {
            return;
        }

        OnGameOver();
        SceneManager.LoadScene(GameSettings.DefeatScene);
    }

    public void OnGameOver()
    {
        GameSettings.PlayerScore = Player.GetScore();

        if (GameSettings.GameMode == GameSettings.Mode.COUNTDOWN)
        {
            GameSettings.PlayerSurvivalTime = GameSettings.GameDifficulty.TimeNeeded - timeRemaining;
            if(timeRemaining == 0)
            {
                // Player won, update num stars high score
                float percent = GameSettings.PlayerScore * 1.0f / GameSettings.NumCollectibles;
                GameSettings.SetCountdownHighScore(Victory.CalculateNumStars(percent));
            }
        }
        else
        {
            if(GameSettings.GameMode == GameSettings.Mode.SURVIVAL)
            {
                GameSettings.SetSurvivalHighScore(timeRemaining);
            }
            else if(GameSettings.GameMode == GameSettings.Mode.COLLECTION)
            {
                if(GameSettings.PlayerScore == GameSettings.NumCollectibles)
                {
                    // Player won
                    GameSettings.SetCollectionHighScore(timeRemaining);
                }
            }
            GameSettings.PlayerSurvivalTime = timeRemaining;
        }
        
        isGameOver = true;
    }

    /* Misc Functions */
    
    private void SpawnHunters()
    {
        int numHunters = GameSettings.GameDifficulty.NumHunters;
        if(numHunters >= HunterSpawnPoints.Length)
        {
            numHunters = HunterSpawnPoints.Length;
        }

        for(int i = 0; i < numHunters; ++i)
        {
            Vector2 pos = HunterSpawnPoints[i].transform.position;
            GameObject obj = Instantiate(HunterPrefab, pos, Quaternion.identity, HunterParent);
            Hunter hunter = obj.GetComponent<Hunter>();
            Hunters.Add(hunter);
        }
    }
    private void SpawnCollectibles(int howMany)
    {
        const int maxFails = 100;

        // Do not spawn any directly on top of the player
        Point playerTile = Player.GetPoint();
        float collectibleRadius = CollectiblePrefab.transform.localScale.x * CollectiblePrefab.GetComponent<CircleCollider2D>().radius;

        int numFails = 0;
        HashSet<Point> found = new HashSet<Point>();
        while(howMany > 0 && numFails < maxFails)
        {
            int pX = Random.Range(0, grid.GetSizeX());
            int pY = Random.Range(0, grid.GetSizeY());
            Point p = new Point(pX, pY);
            Tile tile = grid.GetTile(p);

            if(tile == null)
            {
                Debug.LogWarning("No tile for " + p);
                ++numFails;
                continue;
            }

            if(!tile.IsWalkable() || found.Contains(p) || p.Equals(playerTile))
            {
                ++numFails;
                continue;
            }

            // Valid tile
            found.Add(p);
            Vector2 offset = new Vector2(tileSize - collectibleRadius, tileSize - collectibleRadius) * Random.Range(-0.5f, 0.5f);
            AddCollectible(p, offset);
            howMany--;
        }
    }

    private void AddCollectible(Point p, Vector2 offset)
    {
        Vector2 pos = PointToWorldPos(p, true) + offset;
        GameObject obj = Instantiate(CollectiblePrefab, pos, Quaternion.identity, CollectibleParent);
        Collectible collectible = obj.GetComponent<Collectible>();
        collectible.SetPoint(p);
        collectibles.Add(collectible);
    }

    public void RemoveCollectible(Collectible collectible)
    {
        collectibles.Remove(collectible);
    }

    public void AddFootprintToList(Point pos, bool isPlayer)
    {
        if (isPlayer)
        {
            playerFootprints.Add(pos);
        }
        else
        {
            hunterFootprints.Add(pos);
        }
    }

    /* Constant Updates */

    // Manually update steerables to preserve update order
    private void UpdateAllSteerables()
    {
        Player.DoUpdate();
        foreach(Hunter hunter in Hunters)
        {
            hunter.DoUpdate();
        }
    }

    private void UpdateHunterAudio()
    {
        const float maxVolume = 0.3f;
        const float minThreshold = 7.0f;

        float minDistance = 9999.0f;
        Vector2 playerPos = Player.GetPosition();
        foreach(Hunter hunter in Hunters)
        {
            float dist = Vector2.Distance(hunter.GetPosition(), playerPos);
            if(dist < minDistance)
            {
                minDistance = dist;
            }
        }

        if(minDistance < minThreshold)
        {
            float multiplier = 1 - (minDistance / minThreshold);
            float volume = maxVolume * multiplier;
            HunterAudio.SetVolume(volume);
        }
        else
        {
            HunterAudio.SetVolume(0.0f);
        }
    }

    // Change opacity of different objects based on if the player can see them or not
    private void UpdatePlayerSight()
    {
        Vector2 playerPos = Player.gameObject.transform.position;
        Point playerPoint = WorldPosToPoint(playerPos);
        int[,] playerSight = grid.GetSight(playerPoint, PlayerSightDistance);

        UpdateSeenHunters(playerSight);
        UpdateAllSeenFootprints(playerSight);
        UpdateSeenCollectibles(playerSight);

        // Color in tiles that can be seen
        grid.ColorPlayerSight(playerSight);
    }

    private void UpdateSeenHunters(int[,] playerSight)
    {
        foreach (Hunter hunter in Hunters)
        {
            Point hunterPoint = hunter.GetPoint();
            if (playerSight[hunterPoint.GetX(), hunterPoint.GetY()] >= 0)
            {
                hunter.SetOpacity(1.0f);
            }
            else
            {
                hunter.SetOpacity(hidden);
            }
        }
    }

    private void UpdateAllSeenFootprints(int[,] playerSight)
    {
        UpdateSeenFootprints(playerSight, playerFootprints);
        UpdateSeenFootprints(playerSight, hunterFootprints);
    }

    private void UpdateSeenFootprints(int[,] playerSight, List<Point> footprints)
    {
        foreach(Point p in footprints)
        {
            Tile tile = grid.GetTile(p);
            if(tile == null)
            {
                continue;
            }
            if(playerSight[p.GetX(), p.GetY()] >= 0)
            {
                tile.TileObject.SetFootprintOpacity(1.0f);
            } else
            {
                tile.TileObject.SetFootprintOpacity(hidden);
            }   
        }
    }

    private void UpdateSeenCollectibles(int[,] playerSight)
    {
        foreach(Collectible c in collectibles)
        {
            Point p = c.GetPoint();
            if(playerSight[p.GetX(), p.GetY()] >= 0)
            {
                c.SetOpacity(1.0f);
            } else
            {
                c.SetOpacity(hidden);
            }
        }
    }

    /* Every Second Updates */

    // Called every second
    // https://answers.unity.com/questions/1220440/how-to-display-call-a-function-every-second.html
    private void UpdateEverySecond()
    {
        // Different behavior depending on mode
        if (GameSettings.GameMode == GameSettings.Mode.COUNTDOWN)
        {
            DecrementTime();
            if (timeRemaining <= 0)
            {
                PlayerWin();
            }
        }
        else
        {
            IncrementTime();
        }

        UpdateFootprints();
    }

    private void DecrementTime()
    {
        if(timeRemaining <= 0)
        {
            return;
        }

        timeRemaining--;
        TimeRemainingDisplay.text = "" + timeRemaining;
    }

    private void IncrementTime()
    {
        timeRemaining++;
        TimeRemainingDisplay.text = "" + timeRemaining;
    }

    private void UpdateFootprints()
    {
        // Clean up all footprints
        CleanupFootprints(hunterFootprints);
        CleanupFootprints(playerFootprints);

        // Add new player footprints
        Point playerPoint = Player.GetPoint();
        Tile playerTile = grid.GetTile(playerPoint);
        if(playerTile != null)
        {
            playerTile.PlaceFootprint(true, Player.GetVelocity());
        }

        foreach(Hunter hunter in Hunters)
        {
            Point hunterPoint = hunter.GetPoint();
            Tile tile = grid.GetTile(hunterPoint);
            if(tile != null)
            {
                tile.PlaceFootprint(false, hunter.GetVelocity());
                tile.TileObject.SetFootprintOpacity(hidden); // Start hidden, this can be corrected later
            }
        }
    }

    private void CleanupFootprints(List<Point> footprints)
    {
        for (int i = footprints.Count - 1; i >= 0; --i)
        {
            Point point = footprints[i];
            Tile tile = grid.GetTile(point);

            if (tile == null || !tile.FootprintExists())
            {
                footprints.RemoveAt(i);
                continue;
            }

            tile.IncrementFootprintAge(MaxFootprintAge);

            if (tile.GetFootprintAge() > MaxFootprintAge)
            {
                footprints.RemoveAt(i);
                tile.RemoveFootprint();
            }
        }
    }

    /* Hunter AI */

    // Assign target to Hunters based on what ALL hunters can see.
    private void DoHunterBehavior()
    {
        int[,] hunterSight = CalculateHunterSight();

        Point playerPoint = Player.GetPoint();
        if(hunterSight[playerPoint.GetX(), playerPoint.GetY()] >= 0)
        {
            AssignAllHunterTargets(Player.GetPosition(), Hunter.TargetType.PLAYER);
            wasTrackingPlayer = true;
            lastPlayerPos = Player.GetPosition();
        } else
        {
            if(wasTrackingPlayer)
            {
                wasTrackingPlayer = false;
                AssignAllHunterTargets(lastPlayerPos, Hunter.TargetType.LAST_PLAYER_POS);
                return;
            }

            // Iterate through footprints, find the youngest non-found one
            int minAge = 999;
            Tile footprint = null;
            foreach(Point p in playerFootprints)
            {
                if(hunterSight[p.GetX(), p.GetY()] < 0)
                {
                    continue;
                }
                Tile tile = grid.GetTile(p);
                if(tile == null)
                {
                    continue;
                }
                if(!tile.IsFootprintFound())
                {
                    int age = tile.GetFootprintAge();
                    if(age < minAge)
                    {
                        minAge = age;
                        footprint = tile;
                    }
                }
            }

            if(footprint != null)
            {
                footprint.SetFootprintFound(true);
                AssignAllHunterTargets(PointToWorldPos(footprint.GetPos()), Hunter.TargetType.FOOTPRINT);
            }
        }
    }

    // Merges sight of all hunters together
    private int[,] CalculateHunterSight()
    {
        int[,] hunterSight = grid.CreateBlankSight();

        foreach (Hunter hunter in Hunters)
        {
            Point point = hunter.GetPoint();
            int[,] sight = grid.GetSight(point, HunterSightDistance);
            hunterSight = grid.MergeSight(hunterSight, sight);
        }

        return hunterSight;
    }

    private void AssignAllHunterTargets(Vector2 pos, Hunter.TargetType type)
    {
        foreach(Hunter hunter in Hunters)
        {
            hunter.AssignTarget(pos, type);
        }
    }

    private void ClearAllHunterTargets()
    {
        foreach(Hunter hunter in Hunters)
        {
            hunter.ResetTarget();
        }
    }

    /* World Generation */

    private void CalculateTileOffsets()
    {
        tileSize = BlankTile.GetComponent<SpriteRenderer>().bounds.size.x;
        float offsetX = tileSize * grid.GetSizeX() / 2.0f - tileSize / 2.0f;
        float offsetY = tileSize * grid.GetSizeY() / 2.0f - tileSize / 2.0f;
        worldBottomLeft = World.position - new Vector3(offsetX, offsetY, 0);
    }

    private void GenerateWorldTiles()
    {
        for(int tileX = 0; tileX < grid.GetSizeX(); ++tileX)
        {
            for(int tileY = 0; tileY < grid.GetSizeY(); ++tileY)
            {
                Tile tile = grid.GetTile(tileX, tileY);

                GameObject prefab = BlankTile;

                if(!tile.IsWalkable())
                {
                    prefab = FilledTile;
                }

                GameObject tileObject = Instantiate(prefab, World);
                tileObject.transform.position = PointToWorldPos(tileX, tileY);

                tile.TileObject = tileObject.GetComponent<TileObject>();
            }
        }
    }

    /* Helpers */

    public TileObject GetTileObjectAtPos(Vector2 pos)
    {
        Point point = WorldPosToPoint(pos);
        if(!grid.IsValidPoint(point))
        {
            return null;
        }
        Tile tile = grid.GetTile(point);
        return tile.TileObject;
    }

    public Tile GetTileAtPos(Vector2 pos)
    {
        Point point = WorldPosToPoint(pos);
        if (!grid.IsValidPoint(point))
        {
            return null;
        }
        Tile tile = grid.GetTile(point);
        return tile;
    }

    public Point WorldPosToPoint(Vector2 pos)
    {
        Vector2 transformed = (pos - worldBottomLeft) / tileSize;
        int x = (int) Mathf.Round(transformed.x);
        int y = (int) Mathf.Round(transformed.y);
        Point point = new Point(x, y);
        //Debug.Log(transformed + "->" + point);
        return point;
    }

    public Vector2 PointToWorldPos(Point p, bool center=false)
    {
        return PointToWorldPos(p.GetX(), p.GetY());
    }

    public Vector2 PointToWorldPos(int x, int y, bool center=false)
    {
        float posX = x * tileSize;
        float posY = y * tileSize;
        if(center)
        {
            posX += tileSize / 2.0f;
            posY += tileSize / 2.0f;
        }
        return worldBottomLeft + new Vector2(posX, posY);
    }

    public Vector2 CheckGameBoundaries(Vector2 pos)
    {
        if(pos.x < worldBottomLeft.x)
        {
            pos.x = worldBottomLeft.x;
        }
        else if(pos.x >= worldBottomLeft.x + GetGameSizeX())
        {
            pos.x = worldBottomLeft.x + GetGameSizeX();
        }

        if (pos.y < worldBottomLeft.y)
        {
            pos.y = worldBottomLeft.y;
        }
        else if (pos.y >= worldBottomLeft.y + GetGameSizeY())
        {
            pos.y = worldBottomLeft.y + GetGameSizeY();
        }
        return pos;
    }

    public bool IsOutOfBounds(Vector2 pos)
    {
        return pos != CheckGameBoundaries(new Vector2(pos.x, pos.y));
    }

    public float GetGameSizeX()
    {
        return tileSize * (grid.GetSizeX() - 1);
    }

    public float GetGameSizeY()
    {
        return tileSize * (grid.GetSizeY() - 1);
    }

    public Grid GetGrid()
    {
        return grid;
    }
}
