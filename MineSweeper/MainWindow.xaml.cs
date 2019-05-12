using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MineSweeper
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int sizeRows;
        private int sizeColumns;
        private int minesCount;
        private int minesSelectedCount;
        private int minesLeft;
        private int openedCount;
        private int emptyCells;
        private bool isGameOn = false;
        DispatcherTimer timer;
        Stopwatch stopWatch;
        Button[,] buttons;
        GameBackEnd game;


        public MainWindow()
        {
            InitializeComponent();
            SetUpGame(1);
        }

        private void SetUpGame(int level)
        {
            stopWatch = new Stopwatch();
            switch (level)
            {
                case 1:
                    sizeRows = 10;
                    sizeColumns = 10;
                    minesCount = 10;
                    break;
                case 2:
                    sizeRows = 20;
                    sizeColumns = 20;
                    minesCount = 50;
                    break;
                case 3:
                    sizeRows = 25;
                    sizeColumns = 25;
                    minesCount = 100;
                    break;
                default:
                    var Dialog = new SizeDialog();
                    if (Dialog.ShowDialog() == true)
                    {
                        sizeRows = Dialog.Rows;
                        sizeColumns = Dialog.Columns;
                        minesCount = Dialog.Mines;
                    }
                    break;

            }
            CreateGameGrid();
            minescCountBox.Text = minesCount.ToString();
            emptyCells = sizeRows * sizeColumns - minesCount;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            ClearGame();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timerBox.Text = stopWatch.Elapsed.ToString(@"mm\:ss");
        }

        internal void Start()
        {
            stopWatch.Start();
            timer.Start();
        }

        private void CreateGameGrid()
        {
            gameGrid.Children.Clear();
            buttons = new Button[sizeRows, sizeColumns];
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < sizeRows; i++)
            {
                gameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < sizeColumns; i++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < sizeRows; i++)
            {
                for (int j = 0; j < sizeColumns; j++)
                {
                    buttons[i, j] = new Button();
                    string name = "button" + i + "_" + j;
                    buttons[i, j].Name = name;
                    buttons[i, j].Click += GameButton_Click;
                    buttons[i, j].MouseRightButtonDown += GameButton_RightClick;
                    buttons[i, j].SetValue(Grid.RowProperty, i);
                    buttons[i, j].SetValue(Grid.ColumnProperty, j);
                    buttons[i, j].Content = " ";
                    gameGrid.Children.Add(buttons[i, j]);
                }
            }

            game = new GameBackEnd(sizeRows, sizeColumns, minesCount);
        }

        private void GameButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            Button thisButton = sender as Button;
            if (thisButton.Content.ToString() == " ")
            {
                minesSelectedCount++;
                int row = Grid.GetRow(thisButton);
                int column = Grid.GetColumn(thisButton);
                thisButton.Content = "!";
                if (game.CheckField(row, column) == -1)
                {
                    minesLeft--;
                    CheckIfWon();
                }
                minesSelected.Text = minesSelectedCount.ToString();
            }
            else if (thisButton.Content.ToString() == "!")
            {
                thisButton.Content = "?";
                minesSelectedCount--;
                minesSelected.Text = minesSelectedCount.ToString();
            }
            else if (thisButton.Content.ToString() == "?")
            {
                thisButton.Content = " ";
            }
        }

        private void CheckIfWon()
        {
            if ((minesLeft == 0 && minesSelectedCount == minesCount) || openedCount == emptyCells)
            {
                timer.Stop();
                stopWatch.Stop();
                ShowMines();
                isGameOn = false;
                MessageBox.Show($"You won! Your time was {stopWatch.Elapsed.ToString(@"mm\:ss\:fff")}");
                gameGrid.IsEnabled = false;
            }
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            Button thisButton = sender as Button;
            CheckField(thisButton);
            if (!isGameOn)
            {
                Start();
                isGameOn = true;
                CheckIfWon();
            }
        }

        private void CheckField(Button thisButton)
        {
            openedCount++;
            if (thisButton.Content.ToString() == "!" || thisButton.Content.ToString() == "?")
            {
                return;
            }
            if (thisButton.Content.ToString() != " ")
            {
                CheckIfFoundNeighborMines(thisButton);
            }
            int row = Grid.GetRow(thisButton);
            int column = Grid.GetColumn(thisButton);
            int result = game.CheckField(row, column);
            if (result == -1)
            {
                LostGame();
            }
            if (result > 0)
            {
                thisButton.Content = result.ToString();
                thisButton.Background = new SolidColorBrush(((SolidColorBrush)thisButton.Background).Color) { Opacity = 0.4 };
                thisButton.Foreground = CreateNumberColor(result);
            }
            if (result == 0)
            {
                thisButton.IsEnabled = false;
                int tmpRow;
                int tmpColumn;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        tmpRow = row + i;
                        tmpColumn = column + j;
                        bool middle = i == 0 && j == 0;
                        if (tmpRow >= 0 && tmpRow < sizeRows &&
                            tmpColumn >= 0 && tmpColumn < sizeColumns &&
                            !middle && buttons[tmpRow, tmpColumn].IsEnabled == true)
                        {
                            CheckField(buttons[tmpRow, tmpColumn]);
                        }
                    }
                }
            }
            if (openedCount == emptyCells && isGameOn)
            {
                CheckIfWon();
            }
        }

        private Brush CreateNumberColor(int mines)
        {
            byte r = (byte)((mines - 1) * 36);
            byte g = 0;
            byte b = (byte)(255 - ((mines - 1) * 36));
            SolidColorBrush result = new SolidColorBrush(Color.FromRgb(r, g, b));
            return result;
        }

        private void CheckIfFoundNeighborMines(Button thisButton)
        {
            int numberOfMinesArround = int.Parse(thisButton.Content.ToString());
            int numberOfSelectedMinesArround = 0;
            int row = Grid.GetRow(thisButton);
            int column = Grid.GetColumn(thisButton);
            int tmpRow;
            int tmpColumn;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tmpRow = row + i;
                    tmpColumn = column + j;
                    bool middle = i == 0 && j == 0;
                    if (tmpRow >= 0 && tmpRow < sizeRows &&
                        tmpColumn >= 0 && tmpColumn < sizeColumns &&
                        !middle && buttons[tmpRow, tmpColumn].Content.ToString() == "!")
                    {
                        numberOfSelectedMinesArround++;
                    }
                }
            }
            if (numberOfSelectedMinesArround == numberOfMinesArround)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        tmpRow = row + i;
                        tmpColumn = column + j;
                        bool middle = i == 0 && j == 0;
                        if (tmpRow >= 0 && tmpRow < sizeRows &&
                            tmpColumn >= 0 && tmpColumn < sizeColumns &&
                            !middle && buttons[tmpRow, tmpColumn].IsEnabled == true && buttons[tmpRow, tmpColumn].Content.ToString() == " ")
                        {
                            CheckField(buttons[tmpRow, tmpColumn]);
                        }
                    }
                }
            }
        }

        private void LostGame()
        {
            timer.Stop();
            stopWatch.Stop();
            ShowMines();
            isGameOn = false;
            MessageBox.Show("You lost! :-(");
            gameGrid.IsEnabled = false;
        }

        private void ShowMines()
        {
            int row;
            int column;
            foreach (Button button in buttons)
            {
                row = Grid.GetRow(button);
                column = Grid.GetColumn(button);
                if (game.CheckField(row, column) == -1)
                {
                    button.Content = "X";
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearGame();
        }

        private void ClearGame()
        {
            minesSelectedCount = 0;
            minesSelected.Text = "0";
            game.ClearGameMap();
            minesLeft = minesCount;
            foreach (Button button in buttons)
            {
                button.Content = " ";
                button.IsEnabled = true;
                button.Opacity = 1;
                button.ClearValue(Button.BackgroundProperty);
                button.ClearValue(Button.ForegroundProperty);
            }
            openedCount = 0;
            stopWatch.Reset();
            timerBox.Text = "00:00";
            isGameOn = false;
            gameGrid.IsEnabled = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            switch (menuItem.Header.ToString())
            {
                case "Easy":
                    SetUpGame(1);
                    break;
                case "Medium":
                    SetUpGame(2);
                    break;
                case "Hard":
                    SetUpGame(3);
                    break;
                case "Custom":
                    SetUpGame(4);
                    break;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit now?", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
