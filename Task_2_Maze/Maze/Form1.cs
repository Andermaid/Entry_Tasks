using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Maze
{
    public enum CellType
    {
        Wall,
        Path,
        VisitedPath
    }

    public partial class Form1 : Form
    {
        int mazeSize = 100;
        int cellSize = 5;
        CellType[,] Maze;
        Stack<Point> VisitedPoints;
        Point currentCell;
        Random rnd;

        public Form1()
        {
            InitializeComponent();
            panel1.Size = new Size(mazeSize * cellSize, mazeSize * cellSize);
            rnd = new Random();
            VisitedPoints = new Stack<Point>();
            Maze = new CellType[mazeSize, mazeSize];
            currentCell = new Point(1, 1);
        }

        private void GetTemplate()
        {
            for (int i = 0; i < mazeSize; i++)
            {
                for (int j = 0; j < mazeSize; j++)
                {
                    if (i % 2 != 0 && j % 2 != 0 && i < mazeSize - 1 && j < mazeSize - 1)
                        Maze[i, j] = CellType.Path;
                    else Maze[i, j] = CellType.Wall;
                }
            }
        }

        private void BuildMaze()
        {
            while (true)
            {
                Point[] availablePaths = GetAvailablePaths(currentCell);
                if (availablePaths.Length != 0)
                {
                    int randomPath = rnd.Next(availablePaths.Length);
                    Point nextCell = availablePaths[randomPath];
                    if (availablePaths.Length > 1)
                        VisitedPoints.Push(nextCell);
                    RemoveWall(currentCell, nextCell);
                    Maze[currentCell.X, currentCell.Y] = CellType.VisitedPath;
                    currentCell = nextCell;
                }
                else if (VisitedPoints.Count > 0)
                {
                    Maze[currentCell.X, currentCell.Y] = CellType.VisitedPath;
                    currentCell = VisitedPoints.Pop();
                }
                else break;
            }

            for (int i = 0; i < mazeSize; i++)
            {
                for (int j = 0; j < mazeSize; j++)
                {
                    if (Maze[i, j] == CellType.VisitedPath)
                        Maze[i, j] = CellType.Path;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetTemplate();
            BuildMaze();
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < mazeSize; i++)
            {
                for (int j = 0; j < mazeSize; j++)
                {
                    switch (Maze[i, j])
                    {
                        case CellType.Wall:
                            DrawCell(i, j, e.Graphics, Brushes.Black);
                            break;
                        case CellType.Path:
                            DrawCell(i, j, e.Graphics, Brushes.White);
                            break;
                        case CellType.VisitedPath:
                            DrawCell(i, j, e.Graphics, Brushes.Red);
                            break;
                    }
                }
            }
        }

        private Point[] GetAvailablePaths(Point cell)
        {
            int step = 2;
            Point pathUp = new Point(cell.X, cell.Y - step);
            Point pathDown = new Point(cell.X, cell.Y + step);
            Point pathLeft = new Point(cell.X - step, cell.Y);
            Point pathRight = new Point(cell.X + step, cell.Y);
            Point[] paths = new Point[] { pathUp, pathDown, pathLeft, pathRight };

            List<Point> availablePaths = new List<Point>();
            foreach(var p in paths)
            {
                if (p.X > 0 && p.X < mazeSize && p.Y > 0 && p.Y < mazeSize)
                {
                    if (Maze[p.X, p.Y] == CellType.Path)
                        availablePaths.Add(p);
                }
            }

            return availablePaths.ToArray();
        }

        private void RemoveWall(Point first, Point second)
        {
            int xDiff = second.X - first.X;
            int yDiff = second.Y - first.Y;

            int addX = xDiff != 0 ? xDiff / Math.Abs(xDiff) : 0;
            int addY = yDiff != 0 ? yDiff / Math.Abs(yDiff) : 0;

            int wallX = first.X + addX;
            int wallY = first.Y + addY;

            Maze[wallX, wallY] = CellType.VisitedPath;
        }

        private void DrawCell(int x, int y, Graphics graphics, Brush brush)
        {
            graphics.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
        }
    }
}
