using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface Memsher;
    [SerializeField]
    private Vector3 playerSpawnOffset;
    [SerializeField] 
    private Vector3 enemySpawnOffset;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private MazeCell _mazeCellPreFab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    private MazeCell[,] _mazeGrid;
    [SerializeField]
    private int exits = 4;

    [SerializeField] 
    private float minEnemySpawnDist = 4f;
    private Vector2 player_location;
    private GameObject playerInstance;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];
        
        
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPreFab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);
        ExitR();
        Memsher.BuildNavMesh();
    }
    public void ExitR()
    {
        for (int i = 1; i < exits; i++)
        {
            int randomHorizontalExits = Random.Range(0, _mazeDepth);
            Destroy(_mazeGrid[0, randomHorizontalExits].gameObject);

            int randomVerticalExits = Random.Range(0, _mazeWidth);
            Destroy(_mazeGrid[randomVerticalExits, 0].gameObject);
            
        }

    }

    

    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.0001f);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
            
        } while (nextCell != null);


    }
        

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1>=0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            } 
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
    [ContextMenu("SpawnPlayer")]
    public void SpawnPlayer()
    {
        int x = Random.Range(0,_mazeWidth);
        int y = Random.Range(0, _mazeDepth);
        GameObject spawnTile = _mazeGrid[x,y].gameObject;
        playerInstance = Instantiate(player, spawnTile.transform.position+playerSpawnOffset, spawnTile.transform.rotation);
        print(playerInstance.name + " skibidi");
        player_location = new(x, y);
    }
    [ContextMenu("SpawnEnemy")]
    public void SpawnEnemy()
    {
        int opp_x = Random.Range(0, _mazeWidth);
        int opp_y = Random.Range(0, _mazeDepth);
        while (Vector2.Distance(player_location, new(opp_x, opp_y)) < minEnemySpawnDist)
        {
            opp_x = Random.Range(0, _mazeWidth);
            opp_y = Random.Range(0, _mazeDepth);
        }
        GameObject spawnTile2 = _mazeGrid[opp_x, opp_y].gameObject;
        if (Instantiate(enemy, spawnTile2.transform.position + enemySpawnOffset, spawnTile2.transform.rotation).TryGetComponent(out FieldofView fov))
        {
            print("skibidi");
            print(playerInstance.name + " skibidi");
            fov.playerRef = playerInstance;
            if (fov.gameObject.TryGetComponent(out NavigationBehaviour nb))
            {
                nb.player = playerInstance.transform;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
