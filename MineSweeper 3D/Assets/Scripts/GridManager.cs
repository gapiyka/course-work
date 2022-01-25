using System;
using System.Collections;
using UnityEngine;

public struct Tile
{
    public GameObject gameObj;
    public int minesCounter;
    public bool IsChecked;
    public bool IsFlag;
    public bool IsVisitedByOpening;
}

public class GridManager : MonoBehaviour
{
    #region Attributes
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bordersGO;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject[] tilesPrefabs;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private TimerController timerController;

    private int gameDifficulty; // 3 levels of difficult
    private int gridSize; // range of tiles for axis
    private int mapSize; // axis * axis
    private float mapMiddle; // center of map (used to start point)
    private int mines—hance; // chance of generation mines on Map
    private int nMines; // number of mines at map
    private bool isCoroutineExecuting = false; // checker for restartGameCoroutine
    private GameObject tempBorder; // temporary Border for destroying

    private const int sizeMultiplier = 9; // multipler for payable size
    private const int minesMultiplier = 10; // diviner to count n of mines
    private const int tileSize = 5; // size of plate from plane object scale
    private string[] result; // get string timer + mines counter

    public Tile[,] gridMatrix; // two-dimensional array to detect mine
    public GameState gameState;
#endregion

    public void ReloadMap(int difficulty)
    {
        ClearExistingMap();
        CalculateGameParameters(difficulty);

        playerTransform.position = GetStartPos(1.1f); // spawn player at middle of map
        bordersGO.transform.localScale = CalcBorderScale(gameDifficulty); // relocate borders

        Destroy(tempBorder);// destroy temporary borders
        tempBorder = Instantiate(bordersGO, // // create new temporary borders
            CalcBorderPos(gridSize, tileSize), 
            Quaternion.Euler(0, 180, 0), this.transform);

        GenerateGrid();
        RunTimer(true);
    }

    public int[] CalculateGameParameters(int difficulty)
    {
        gameDifficulty = difficulty;
        gridSize = (gameDifficulty * sizeMultiplier) - (gameDifficulty - 1);
        mapSize = gridSize * gridSize;
        mapMiddle = gridSize / 2f;

        return new int[] { gameDifficulty, gridSize, mapSize };
    }

    public Vector3 CalcBorderScale(int difficulty)
    {
        return new Vector3(difficulty, 1, difficulty);
    }

    public Vector3 CalcBorderPos(int gridSize, int tileSize)
    {
        return new Vector3(gridSize * tileSize, 0, gridSize * tileSize);
    }

    void ClearExistingMap()
    {
        if (gridMatrix != null)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    if (gridMatrix[x, z].gameObj != null) Destroy(gridMatrix[x, z].gameObj);
                }
            }
        }
    }

    // generation grid with mines
    void GenerateGrid()
    {
        const int randomLowerBorder = 0;
        const int randomUpperBorder = 100;
        int randomChance;

        nMines = 0;
        mines—hance = (gameDifficulty * tileSize) + minesMultiplier;
        gridMatrix = new Tile[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                randomChance = UnityEngine.Random.Range(randomLowerBorder, randomUpperBorder);
                if (randomChance < mines—hance)
                {
                    gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[9], 
                        new Vector3((x * tileSize) + (tileSize / 2f), 0, (z * tileSize) + (tileSize / 2f)), 
                        Quaternion.identity, this.transform);
                    gridMatrix[x, z].minesCounter = 9;
                    nMines++;
                }
                gridMatrix[x, z].IsChecked = false; // set tile not checked
                gridMatrix[x, z].IsFlag = false; // set tile without flag
                gridMatrix[x, z].IsVisitedByOpening = false; // set tile not visited
            }
        }
        PaintMineArea();
    }

    //should be optimized
    void PaintMineArea()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (gridMatrix[x, z].minesCounter == 9) continue;
                int n = MatrixCheckSquare(x, z, 
                    (int1, int2) => { return 1; }, 
                    (xCheck, zCheck) => { return gridMatrix[xCheck, zCheck].minesCounter == 9; });

                gridMatrix[x, z].minesCounter = n;
                gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[n], 
                    new Vector3((x * tileSize) + (tileSize / 2f), 0f, (z * tileSize) + (tileSize / 2f)), 
                    Quaternion.identity, this.transform);
            }
        }
    }

    int OpenTile(int x, int z)
    {
        Transform checkT = gridMatrix[x, z].gameObj.transform.GetChild(0);
        Transform bodyT = gridMatrix[x, z].gameObj.transform.GetChild(1);
        checkT.gameObject.SetActive(false);
        bodyT.gameObject.SetActive(true);
        gridMatrix[x, z].IsChecked = true;
        gridMatrix[x, z].IsVisitedByOpening = true;
        return 1;
    }

    int OpenCloseTiles(int x, int z)
    {
        if (gridMatrix[x, z].IsChecked)
        {
            MatrixCheckSquare(x, z, OpenCloseTiles,
                (xCheck, zCheck) => { return !gridMatrix[xCheck, zCheck].IsFlag && 
                    !gridMatrix[xCheck, zCheck].IsVisitedByOpening; });
        }

        OpenTile(x, z);

        if (gridMatrix[x, z].minesCounter == 0)
        {
            MatrixCheckSquare(x, z, OpenCloseTiles, 
                (xCheck, zCheck) => { return !gridMatrix[xCheck, zCheck].IsVisitedByOpening; });
        }

        if (gridMatrix[x, z].minesCounter == 9)
        {
            StartCoroutine(OpenMine(gridMatrix[x, z].gameObj));
            return -1;
        }

        return 1;
    }

    int MatrixCheckSquare(int x, int z, Func<int, int, int> callback, Func<int, int, bool> someCondition)
    {
        int n = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = z - 1; j <= z + 1; j++)
            {
                if (i < 0) continue;
                if (j < 0) continue;
                if (i >= gridSize) continue;
                if (j >= gridSize) continue;
                if (someCondition(i, j))
                {
                    n += callback(i, j);
                }
            }
        }
        return n;
    }

    IEnumerator OpenMine(GameObject mine)
    {
        if (isCoroutineExecuting)
            yield break;

        const int indexBody = 1;// index 1 of mine == "Body"
        const int indexMine = 0;// index 0 of body == "Mine"
        const int indexExplosionEffect = 2;// index 2 of mine == "ExplosionEffect"
        const float timeDelay = 1f;

        Transform effectT = mine.transform
            .GetChild(indexBody)
            .GetChild(indexMine)
            .GetChild(indexExplosionEffect);
        if (effectT.TryGetComponent(out ParticleSystem ps))
        {
            ps.Play();
        }

        cameraShake.shakeDuration = timeDelay;

        isCoroutineExecuting = true;

        yield return new WaitForSeconds(timeDelay);

        EndGame(GameState.Lose);

        isCoroutineExecuting = false;
    }

    void EndGame(GameState state)
    {
        result = new string[2] { timerController.GetStringTimer(), nMines.ToString() };
        gameState = state;
        StopTimer();
    }
     
    Vector3 GetStartPos(float height)
    {
        return new Vector3(tileSize * mapMiddle, height, tileSize * mapMiddle);
    }

    void FlagClaim(int x, int z, bool IsAlreadyFlag)
    {
        Transform CheckerTransform = gridMatrix[x, z].gameObj.transform.GetChild(0);
        if (IsAlreadyFlag) RemoveFlagFromTile(CheckerTransform, x, z);
        else PutFlagOnTile(CheckerTransform, x, z);
    }

    void PutFlagOnTile(Transform parent, int x, int z)
    {
        Instantiate(flagPrefab, parent.position, Quaternion.identity, parent);
        gridMatrix[x, z].IsFlag = true;
        timerController.MinesDecrease();
    }
    
    void RemoveFlagFromTile(Transform parent, int x, int z)
    {
        Destroy(parent.Find("Flag(Clone)").gameObject);// could be changed on SetActive
        gridMatrix[x, z].IsFlag = false;
        timerController.MinesIncrease();
    }

    int SwitchTileState(Tile tile, int button, int xInArray, int zInArray)
    {
        bool IsAlreadyFlag = tile.IsFlag;
        
            if (button == 1 && !IsAlreadyFlag) button = OpenCloseTiles(xInArray, zInArray);

            if (button == 2) FlagClaim(xInArray, zInArray, IsAlreadyFlag);

            if (CheckOnWin()) EndGame(GameState.Win);

            return button;
    }

    bool CheckOnWin()
    {
        if (timerController.GetCurrentMinesCounter() == 0)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Tile currTile = gridMatrix[x, z];
                    if (currTile.IsChecked || currTile.IsFlag) continue;
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public int FindPointedTile(GameObject obj, int button)
    {
        for (int x = 0; x < gridMatrix.GetLength(0); x++)
        {
            for (int z = 0; z < gridMatrix.GetLength(1); z++)
            {
                Tile tile = gridMatrix[x, z];
                if (tile.gameObj.transform.position == obj.transform.position) 
                    return SwitchTileState(tile, button, x, z);
            }
        }
        return 0;
    }

    public void RunTimer(bool newTimer)
    {
        timerController.RunTimer(nMines, newTimer);
    }

    public void StopTimer()
    {
        timerController.StopTimer();
    }

    public string[] GetResults()
    {
        return result;
    }
}