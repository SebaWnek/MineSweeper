using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    class GameBackEnd
    {
        public int[,] gameMap;
        private int rows;
        private int columns;
        private int mines;
        Random rnd = new Random();
        bool started = false;

        public GameBackEnd(int rows, int columns, int mines)
        {
            this.rows = rows;
            this.columns = columns;
            this.mines = mines;
            gameMap = new int[rows, columns];
        }

        public void ClearGameMap()
        {
            gameMap = new int[rows, columns];
            started = false;
        }

        public void GenerateMines(int startRow, int startColumn)
        {
            gameMap[startRow, startColumn] = -2;
            int nextRow;
            int nextColumn;
            for (int i = 0; i < mines; i++)
            {
                while (true)
                {
                    nextRow = rnd.Next(rows);
                    nextColumn = rnd.Next(columns);
                    if (gameMap[nextRow, nextColumn] == 0 &&
                        !((nextRow == startRow - 1 || nextRow == startRow + 1 || nextRow == startRow) &&
                        (nextColumn == startColumn - 1 || nextColumn == startColumn + 1 || nextColumn == startColumn)))
                    {
                        gameMap[nextRow, nextColumn] = -1;
                        break;
                    }
                }
            }
            CalculateNeighbors();
            started = true;
        }

        private void CalculateNeighbors()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if(gameMap[i,j] == -1)
                    {
                        AddToNeighbors(i, j);
                    }
                }
            }
        }

        private void AddToNeighbors(int row, int column)
        {
            int tmpRow;
            int tmpColumn;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tmpRow = row + i;
                    tmpColumn = column + j;
                    bool middle = i == 0 && j == 0;
                    if (tmpRow >= 0 && tmpRow < rows &&
                        tmpColumn >= 0 && tmpColumn < columns &&
                        !middle && gameMap[tmpRow,tmpColumn] >= 0)
                    {
                        gameMap[tmpRow, tmpColumn]++;
                    }
                }
            }
        }

        public int CheckField(int row, int column)
        {
            // 0 - empty 
            // -1 mine!
            // >0 number of neighbors
            if (!started)
            {
                MainWindow mainWindow = new MainWindow();
                GenerateMines(row, column);
                started = true;
                return 0;
            }
            return gameMap[row, column];
        }
    }
}
