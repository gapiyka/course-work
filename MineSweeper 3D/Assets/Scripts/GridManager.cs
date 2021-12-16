using System.Collections;
using System.Threading.Tasks;
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
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bordersGO;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject[] tilesPrefabs;
    [SerializeField] private CameraShake cameraShake;

    private int gameDifficulty; // 3 levels of difficult
    private int gridSize; // range of tiles for axis
    private int mapSize; // axis * axis
    private float mapMiddle; // center of map (used to start point)
    private int mines—hance; // chance of generation mines on Map
    private int nMines; // number of mines at map
    private bool isCoroutineExecuting = false; // checker for restartGameCoroutine

    private const int sizeMultiplier = 9; // multipler for payable size
    private const int minesMultiplier = 20; // diviner to count n of mines
    private const int tileSize = 5; // size of plate from plane object scale

    public Tile[,] gridMatrix; // two-dimensional array to detect mines

    void Start()
    {
        // should take parameter `gameDifficulty` from settings
        ReloadMap(1);
    }

    void ReloadMap(int difficulty)
    {
        gameDifficulty = difficulty;

        gridSize = gameDifficulty * sizeMultiplier;
        mapSize = gridSize * gridSize;
        mapMiddle = gridSize / 2f;

        playerTransform.position = GetStartPos(1.1f); // spawn palyer at middle of map
        bordersGO.transform.localScale = new Vector3(difficulty, 1, difficulty);
        Instantiate(bordersGO, new Vector3(gridSize*tileSize, 0, gridSize * tileSize), Quaternion.Euler(0, 180, 0), this.transform);

        GenerateGrid();
    }

    // generation grid with mines
    void GenerateGrid()
    {
        const int randomLowerBorder = 0;
        const int randomUpperBorder = 100;
        int randomChance;

        nMines = 0;
        mines—hance = gameDifficulty * minesMultiplier;
        gridMatrix = new Tile[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                randomChance = UnityEngine.Random.Range(randomLowerBorder, randomUpperBorder);
                if (randomChance < mines—hance)
                {
                    gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[9], new Vector3((x * tileSize) + (tileSize / 2f), 0, (z * tileSize) + (tileSize / 2f)), Quaternion.identity, this.transform);
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
                int n = 0;
                if (gridMatrix[x, z].minesCounter == 9) continue;
                if(x != 0)
                {
                    if (gridMatrix[x - 1, z].minesCounter == 9) n++;
                }
                if (z != 0)
                {
                    if (gridMatrix[x, z - 1].minesCounter == 9) n++;
                }
                if (x != 0 && z != 0)
                {
                    if (gridMatrix[x - 1, z - 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z].minesCounter == 9) n++;
                }
                if (z != gridSize - 1)
                {
                    if (gridMatrix[x, z + 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1 && z != gridSize - 1)
                {
                    if (gridMatrix[x + 1, z + 1].minesCounter == 9) n++;
                }
                if (x != 0 && z != gridSize - 1)
                {
                    if (gridMatrix[x - 1, z + 1].minesCounter == 9) n++;
                }
                if (x != gridSize - 1 && z !=0)
                {
                    if (gridMatrix[x + 1, z - 1].minesCounter == 9) n++;
                }

                gridMatrix[x, z].gameObj = Instantiate(tilesPrefabs[n], new Vector3((x * tileSize) + (tileSize / 2f), 0f, (z * tileSize) + (tileSize / 2f)), Quaternion.identity, this.transform);
                gridMatrix[x, z].minesCounter = n;
            }
        }
    }

    void OpenTile(int x, int z)
    {
        Transform checkT = gridMatrix[x, z].gameObj.transform.GetChild(0);
        Transform bodyT = gridMatrix[x, z].gameObj.transform.GetChild(1);
        checkT.gameObject.SetActive(false);
        bodyT.gameObject.SetActive(true);
        gridMatrix[x, z].IsChecked = true;
        gridMatrix[x, z].IsVisitedByOpening = true;
    }

    int OpenCloseTiles(int x, int z)
    {
        OpenTile(x, z);
        if (gridMatrix[x, z].minesCounter == 0)
        {
            if (x != 0)
            {
                if (!gridMatrix[x - 1, z].IsVisitedByOpening) OpenCloseTiles(x-1, z);
            }
            if (z != 0)
            {
                if (!gridMatrix[x, z - 1].IsVisitedByOpening) OpenCloseTiles(x, z-1);
            }
            if (x != 0 && z != 0)
            {
                if (!gridMatrix[x - 1, z - 1].IsVisitedByOpening) OpenCloseTiles(x-1, z-1);
            }
            if (x != gridSize - 1)
            {
                if (!gridMatrix[x + 1, z].IsVisitedByOpening) OpenCloseTiles(x+1, z);
            }
            if (z != gridSize - 1)
            {
                if (!gridMatrix[x, z + 1].IsVisitedByOpening) OpenCloseTiles(x, z+1);
            }
            if (x != gridSize - 1 && z != gridSize - 1)
            {
                if (!gridMatrix[x + 1, z + 1].IsVisitedByOpening) OpenCloseTiles(x+1, z+1);
            }
            if (x != 0 && z != gridSize - 1)
            {
                if (!gridMatrix[x - 1, z + 1].IsVisitedByOpening) OpenCloseTiles(x-1, z+1);
            }
            if (x != gridSize - 1 && z != 0)
            {
                if (!gridMatrix[x + 1, z - 1].IsVisitedByOpening) OpenCloseTiles(x+1, z-1);
            }
        }

        if (gridMatrix[x, z].minesCounter == 9)
        {
            StartCoroutine(OpenBomb(gridMatrix[x, z].gameObj));
            return -1;
        }
        return 1;
    }

    IEnumerator OpenBomb(GameObject mine)
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

        RestartGame();

        isCoroutineExecuting = false;
    }

    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
    }
    
    void RemoveFlagFromTile(Transform parent, int x, int z)
    {
        Destroy(parent.Find("Flag(Clone)").gameObject);// could be changed on SetActive
        gridMatrix[x, z].IsFlag = false;
    }

    int SwitchTileState(Tile tile, int button, int xInArray, int zInArray)
    {
        bool IsAlreadyChecked = tile.IsChecked;
        bool IsAlreadyFlag = tile.IsFlag;
        
        if (!IsAlreadyChecked)
        {
            if (button == 1 && !IsAlreadyFlag) button = OpenCloseTiles(xInArray, zInArray);

            if (button == 2) FlagClaim(xInArray, zInArray, IsAlreadyFlag);

            return button;
        }
        return 0;
    }

    public int FindPointedTile(GameObject obj, int button)
    {
        for (int x = 0; x < gridMatrix.GetLength(0); x++)
        {
            for (int z = 0; z < gridMatrix.GetLength(1); z++)
            {
                Tile tile = gridMatrix[x, z];
                if (tile.gameObj.transform.position == obj.transform.position) return SwitchTileState(tile, button, x, z);
            }
        }
        return 0;
    }
}