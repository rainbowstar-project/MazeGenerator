using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;

public class MazeGenerator : MonoBehaviour
{
    public int rows;
    public int cols;
    public GameObject Prefab;
    public MazeCell[,] mMazeCells;
    public Maze maze { get; private set; }
    Stack<Cell> _stack = new Stack<Cell>();

    void Start()
    {
        int START_X = -cols / 2;
        int START_Y = -rows / 2;

        maze = new Maze(rows, cols);
        mMazeCells = new MazeCell[cols, rows];

        for (int i = 0; i < cols; ++i) {
            for (int j = 0; j < rows; ++j) {
                GameObject obj = Instantiate(Prefab);
                obj.transform.parent = transform;
                Cell cell = maze.GetCell(i, j);
                cell.onSetDiFlag = OnCellSetDirFlag;
                obj.transform.position = new Vector3(
                    START_X + cell.x,
                    START_Y + cell.y,
                    1.0f);

                mMazeCells[i, j] = obj.GetComponent<MazeCell>();
                mMazeCells[i, j].Cell = cell;
            }
        }
        CreateNewMaze();
    }

    public void CreateNewMaze()
    {
        int x = Random.Range(0, cols);
        int y = Random.Range(0, rows);

        _stack.Push(maze.GetCell(x, y));
        StartCoroutine(Coroutine_Generate());
    }


    public void OnCellSetDirFlag(int x, int y, Directions dir, bool f)
    {
        mMazeCells[x, y].SetActive(dir, f);
    }

    bool GenerateStep()
    {
        if (_stack.Count == 0) {
            return true;
        }
        Cell c = _stack.Peek();
        var neighbours = maze.GetUnvisitedNeighbours(c.x, c.y);

        if (neighbours.Count != 0) {
            var index = 0;

            if (neighbours.Count > 1) {
                index = Random.Range(0, neighbours.Count);
            }
            var item = neighbours[index];
            Cell neighbour = item.Item2;
            neighbour.visited = true;
            maze.RemoveCellWall(c.x, c.y, item.Item1);

            _stack.Push(neighbour);
        } else {
            _stack.Pop();
        }
        return false;
    }

    IEnumerator Coroutine_Generate()
    {
        bool flag = false;
        while (!flag) {
            flag = GenerateStep();

            yield return new WaitForSeconds(0.07f);
        }
    }
}