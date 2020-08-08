using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int mazeRows;
    public int mazeColumns;

    [SerializeField]
    private GameObject mazeCell;
    public class Cell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cScript;

    
    }
    private Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
    private List<Cell> visited = new List<Cell>();
   
   

    private void Start()
    {
        GenerateMaze(mazeRows, mazeColumns);
    }

    private void GenerateMaze(int rows, int columns)
    {
        mazeRows = rows;
        mazeColumns = columns;
        CreateLayout();
        StartAlgorithm();
        
    }

    



    //This basically creates the grid of cells
    private void CreateLayout() 
    {
        float cellSize = 0.98f;
        Vector2 startPos = new Vector2(-9.5f, 4.5f);
        Vector2 spawnPos = startPos;
        for (int i = 0; i < mazeColumns; i++)
        {
            for(int j = 0; j < mazeRows; j++)//generating ith columns of jth row
            {
                createCell(spawnPos, new Vector2(j , i));
                spawnPos.y -= cellSize;

                
            }
            spawnPos.y = startPos.y;//Resetting to ground level
            spawnPos.x += cellSize;
        }

    }

    private void createCell(Vector2 spawnPos, Vector2 keyPos)
    {
        Cell newCell = new Cell();
        

        newCell.gridPos = keyPos;
        newCell.cellObject = Instantiate(mazeCell, spawnPos, Quaternion.identity);
        
        newCell.cellObject.name = "Cell - X:" + keyPos.x + " Y:" + keyPos.y;
        newCell.cScript = newCell.cellObject.GetComponent<CellScript>();
        
        allCells[keyPos] = newCell;
        
    }
    private void StartAlgorithm()
    {
        Cell currentCell = new Cell();
        List<Cell> Stack = new List<Cell>();
        Stack.Add(allCells[new Vector2(0, 0)]);
        Stack.Add(allCells[new Vector2(0, 0)]);
        while (visited.Count < allCells.Count )
        {
            
            currentCell = Stack[Stack.Count - 1];
            Debug.Log("Current Cell is " + currentCell.gridPos.x + "," + currentCell.gridPos.y); 
            Stack.Remove(currentCell);

            if (!visited.Contains(currentCell))
            {
                visited.Add(currentCell);
                
            }
            List<Cell> neighbors = GetUnvisitedNeighbour(currentCell);

            /*foreach (Cell neighboringCell in neighbors)
            {
                if (!(visited.Contains(neighboringCell) && Stack.Contains(neighboringCell) ) )
                {
                    Stack.Add(neighboringCell);
                }
            }*/
            if(neighbors.Count > 0)
            {
                Cell randomNeighbor = neighbors[neighbors.Count - 1];
                Debug.Log("Neighbor chosen " + randomNeighbor.gridPos.x + "," + randomNeighbor.gridPos.y);
                Stack.Add(randomNeighbor);
                Stack.Add(randomNeighbor);
                //inserting twice, one to pop at begin and another one to keep record of cells which have possible unvisited neighbors
                AbolishAdjacentWall(Stack[Stack.Count - 1], currentCell);
            }
            else
            {
                Stack.Remove(currentCell);
            }
            
            
        }
           
    }

    private void AbolishAdjacentWall(Cell neighbor, Cell currentCell)
    {
        Vector2 relativePosition = neighbor.gridPos - currentCell.gridPos;
        //Debug.Log("Relative position is " + relativePosition.x + "," +relativePosition.y);
        if (relativePosition.x == -1 )
        {
            Debug.Log(" D of neighbor and U of current cell abolished");
            neighbor.cScript.Wall_D.SetActive(false);
            currentCell.cScript.Wall_U.SetActive(false);
            
            return;
        }
        else if (relativePosition.x == 1)
        {
            Debug.Log(" U of neighbor and D of current cell abolished");
            neighbor.cScript.Wall_U.SetActive(false);
            currentCell.cScript.Wall_D.SetActive(false);
           
            return;
        }
        else if (relativePosition.y == -1)
        {
            Debug.Log(" R of neighbor and L of current cell abolished");
            neighbor.cScript.Wall_R.SetActive(false);
            currentCell.cScript.Wall_L.SetActive(false);
            return;
        }
        else if (relativePosition.y == 1)
        {
            
            Debug.Log(" L of neighbor and R of current cell abolished");
            neighbor.cScript.Wall_L.SetActive(false);
            currentCell.cScript.Wall_R.SetActive(false);
            return;
        }
    }

    

    private List<Cell> GetUnvisitedNeighbour(Cell curCell)
    {
        Vector2 pos = curCell.gridPos;
        List<Cell> neighborsList = new List<Cell>();
        

        
        Vector2 R_adjacent = pos;//position of right adjacent grid
        R_adjacent.x += 1;
        if (allCells.ContainsKey(R_adjacent))
        {
            Cell neighbor = new Cell();
            neighbor = allCells[R_adjacent];
            if (!visited.Contains(neighbor) && neighbor != curCell)
            {
                neighborsList.Add(neighbor);

            }

        }

        Vector2 D_adjacent = pos;//position of down adjacent grid
        D_adjacent.y += 1;
        if (allCells.ContainsKey(D_adjacent))
        {
            Cell neighbor = new Cell();
            neighbor = allCells[D_adjacent];
            if (!visited.Contains(neighbor) && neighbor != curCell)
            {
                neighborsList.Add(neighbor);

            }

        }


        Vector2 U_adjacent = pos;//position of up adjacent grid
        U_adjacent.y -= 1;
        if (allCells.ContainsKey(U_adjacent))
        {
            Cell neighbor = new Cell();
            neighbor = allCells[U_adjacent];
            if (!visited.Contains(neighbor) && neighbor != curCell)
            {
                neighborsList.Add(neighbor);

            }

        }


        Vector2 L_adjacent = pos;//position of left adjacent grid
        L_adjacent.x -= 1;
        if (allCells.ContainsKey(L_adjacent))
        {
            Cell neighbor = new Cell();
            neighbor = allCells[L_adjacent];

            if (!visited.Contains(neighbor) && neighbor != curCell)
            {
                neighborsList.Add(neighbor);

            }
        }
        neighborsList = neighborsList.OrderBy(x => Guid.NewGuid()).ToList();//to shuffle the list
        return neighborsList;
        
    }

    
}
