using System;
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

    //public bool disableCellSprite;
    public class Cell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cScript;
    }
    private Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
    private List<Cell> unvisited = new List<Cell>();
    
   

    private void Start()
    {
        GenerateMaze(mazeRows, mazeColumns);
    }

    private void GenerateMaze(int rows, int columns)
    {
        //if (mazeParent != null) Destroy(mazeParent);

        mazeRows = rows;
        mazeColumns = columns;
        CreateLayout();
        StartAlgorithm();
        //MakeExit();
    }

    



    //This basically creates the grid of cells
    private void CreateLayout() 
    {
        //mazeParent = new GameObject();
        // mazeParent.transform.position = Vector2.zero;
        // mazeParent.name = "Maze";
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
        unvisited.Add(newCell);
    }
    private void StartAlgorithm()
    {
        Cell currentCell = unvisited[0];
        List<Cell> Stack = new List<Cell>();
        Stack.Add(currentCell);

        while(unvisited.Count > 0 && Stack.Count > 0)
        {
            //getting an unvisited neighbor
            List<Cell> neighbors = GetUnvisitedNeighbour(currentCell);
            Cell neighboringCell = new Cell();
            //if neighbor is found, we abolish the wall between current cell and neighbor cell
            //and add the neighbor to the stack
            //else we mark the current cell as visited and remove it from the stack
            
            if(neighbors.Count > 0)
            {
                for(int i = 0; i < neighbors.Count; i++)
                {
                    Stack.Add(neighbors[i]);
                }
                neighboringCell = Stack[Stack.Count - 1];
                AbolishAdjacentWall(neighboringCell, currentCell);
                currentCell = neighboringCell;
            }
            else
            {
                Stack.Remove(currentCell);
                unvisited.Remove(currentCell);
                currentCell = Stack[Stack.Count - 1];
            }
            

        }
     
    }

    private void AbolishAdjacentWall(Cell neighbor, Cell currentCell)
    {
        Vector2 relativePosition = neighbor.gridPos - currentCell.gridPos;
        if (relativePosition.x == -1 )
        {
            neighbor.cScript.Wall_R.SetActive(false);
            currentCell.cScript.Wall_L.SetActive(false);
            return;
        }
        if (relativePosition.x == 1)
        {
            neighbor.cScript.Wall_L.SetActive(false);
            currentCell.cScript.Wall_R.SetActive(false);
            return;
        }
        /*if (relativePosition.y == -1)
        {
            neighbor.cScript.Wall_D.SetActive(false);
            currentCell.cScript.Wall_U.SetActive(false);
            return;
        }
        if (relativePosition.y == 1)
        {
            neighbor.cScript.Wall_U.SetActive(false);
            currentCell.cScript.Wall_D.SetActive(false);
            return;
        }*/
    }

    private List<Cell> GetUnvisitedNeighbour(Cell curCell)
    {
        Vector2 pos = curCell.gridPos;
        List<Cell> neighborsList = new List<Cell>();
        Cell neighbor = new Cell();
        Vector2 U_adjacent = pos;//position of up adjacent grid
        U_adjacent.y -= 1;
        if(allCells.ContainsKey(U_adjacent))
        {
            neighbor = allCells[U_adjacent];
        }
        if(unvisited.Contains(neighbor) && neighbor != curCell)
        {
            neighborsList.Add(neighbor);
        }


        Vector2 D_adjacent = pos;//position of down adjacent grid
        D_adjacent.y += 1;
        if (allCells.ContainsKey(D_adjacent))
        {
            neighbor = allCells[D_adjacent];
        }
        if (unvisited.Contains(neighbor) && neighbor != curCell)
        {
            neighborsList.Add(neighbor);
        }


        Vector2 R_adjacent = pos;//position of right adjacent grid
        R_adjacent.x += 1;
        if (allCells.ContainsKey(R_adjacent))
        {
            neighbor = allCells[R_adjacent];
        }
        if (unvisited.Contains(neighbor) && neighbor != curCell)
        {
            neighborsList.Add(neighbor);
        }


        Vector2 L_adjacent = pos;//position of left adjacent grid
        L_adjacent.x -= 1;
        if (allCells.ContainsKey(L_adjacent))
        {
            neighbor = allCells[L_adjacent];
        }
        if (unvisited.Contains(neighbor) && neighbor != curCell)
        {
            neighborsList.Add(neighbor);
        }

        return neighborsList;
    }
}
