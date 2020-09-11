namespace Minesweeper_WPF
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CustomRectangle[][] mineField;
        private Random rand;
        private int numberMines = 70;   // 8x8: 10,  16x16: 40, 30x16: 99, 30x24: <= 667
        private int blockSize = 20;
        private int fieldsizeX = 520 / 20;
        private int fieldsizeY = 320 / 20;
        private int fieldOffsetX;
        private int fieldOffsetY;
        private bool start = true;
        private bool running = false;
        private volatile int progressVisited;
        private Stopwatch stopwatch;

        public MainWindow()
        {
            InitializeComponent();
            ////fieldsizeX = 3;
            ////fieldsizeY = 3;
            ////mineNumber = 2;
            rand = new Random();
            stopwatch = new Stopwatch();
            mineField = new CustomRectangle[fieldsizeX][];
            for (int i = 0; i < fieldsizeX; i++)
            {
                mineField[i] = new CustomRectangle[fieldsizeY];
            }

            fieldOffsetX = Convert.ToInt32(rec_game.Margin.Left);
            fieldOffsetY = Convert.ToInt32(rec_game.Margin.Top);
            CreateBlocks();
            CreateMines();
        }

        private void CreateBlocks()
        {
            for (int i = 0; i < fieldsizeX; i++)
            {
                for (int j = 0; j < fieldsizeY; j++)
                {
                    mineField[i][j] = new CustomRectangle
                    {
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Height = blockSize,
                        Width = blockSize
                    };
                    mineField[i][j].Margin = new Thickness(blockSize * i + fieldOffsetX, blockSize * j + fieldOffsetY, 0, 0);
                    ////mineField[i, j].ToolTip = i + "|" + j;
                    grid_main.Children.Add(mineField[i][j]);
                }
            }
        }

        private void CreateMines()
        {
            for (int n = 0; n < numberMines; n++)
            {
                int rand_x = rand.Next(0, fieldsizeX);
                int rand_y = rand.Next(0, fieldsizeY);
                if (!mineField[rand_x][rand_y].HasMine)
                {
                    mineField[rand_x][rand_y].HasMine = true;
                }
                else
                {
                    n--;
                    continue;
                }

                // add one to the number of mines in adjacent fields
                for (int i = rand_x - 1; i <= rand_x + 1; i++)
                {
                    for (int j = rand_y - 1; j <= rand_y + 1; j++)
                    {
                        if (i >= 0 && i < fieldsizeX && j >= 0 && j < fieldsizeY)
                        {
                            mineField[i][j].AdjacentMines++;
                        }
                    }
                }
            }
        }

        private void Reset()
        {
            for (int i = 0; i < fieldsizeX; i++)
            {
                for (int j = 0; j < fieldsizeY; j++)
                {
                    mineField[i][j].IsFlagged = false;
                    mineField[i][j].HasMine = false;
                    mineField[i][j].AdjacentMines = 0;
                    mineField[i][j].Uncovered = false;
                    mineField[i][j].ShowNumber(false);
                    mineField[i][j].ShowFlag(false);
                    mineField[i][j].ShowMine(false);
                    mineField[i][j].ShowVisited(false);
                }
            }

            ////RemoveLabel();
            progressVisited = 0;
            CreateMines();
            lb_time.Content = "00:00";
            lb_progress.Content = "Progress: 0,00%";
            start = true;
        }

        private async void Update()
        {
            while (running)
            {
                lb_time.Content = stopwatch.Elapsed.Minutes.ToString("00") + ":" + stopwatch.Elapsed.Seconds.ToString("00");
                lb_progress.Content = "Progress: " + (100.0 * progressVisited / (fieldsizeX * fieldsizeY)).ToString("0.00") + "%";
                await Task.Delay(100);
            }

            stopwatch.Stop();
            stopwatch.Reset();
        }

        private void CheckBlock(int x, int y)
        {
            if (running)
            {
                if (!mineField[x][y].Uncovered && !mineField[x][y].IsFlagged)
                {
                    if (!mineField[x][y].HasMine)
                    {
                        CheckSurroundings(x, y);
                    }
                    else
                    {
                        mineField[x][y].ShowMine(true);
                        running = false;
                    }
                }
            }
        }

        private void CheckSurroundings(int x, int y)
        {
            if (mineField[x][y].HasMine)
            {
                return;
            }

            if (!mineField[x][y].Uncovered)
            {
                mineField[x][y].Uncovered = true;
                mineField[x][y].ShowVisited(true);
                progressVisited++;
                if (mineField[x][y].AdjacentMines == 0)
                {
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        if (i >= 0 && i < fieldsizeX)
                        {
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (j >= 0 && j < fieldsizeY && !mineField[i][j].HasMine)
                                {
                                    ////mineField[x, y].ShowNumber(true);
                                    CheckSurroundings(i, j);
                                }
                            }
                        }
                    }
                }
                else
                {
                    mineField[x][y].ShowNumber(true);
                }
            }
        }

        private void CheckMines()
        {
            bool stop = true;
            for (int i = 0; i < fieldsizeX; i++)
            {
                for (int j = 0; j < fieldsizeY; j++)
                {
                    // stop loops if not all mines marked
                    if (!mineField[i][j].HasMine && !mineField[i][j].Uncovered)
                    {
                        stop = false;
                        break;
                    }
                }
            }

            if (stop)
            {
                running = false;
                ////MessageBox.Show("Gewonnen!\nZeit: " + stopwatch.Elapsed.Minutes.ToString("00") + ":" + stopwatch.Elapsed.Seconds.ToString("00"));
                ////Reset();
            }
        }

        private void SwitchFlag(int x, int y)
        {
            if (!mineField[x][y].Uncovered)
            {
                mineField[x][y].IsFlagged = !mineField[x][y].IsFlagged;
                mineField[x][y].ShowFlag(mineField[x][y].IsFlagged);
            }
        }

        private void Mainwindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // check if mouse event in playfield
            if (e.GetPosition(this).X < fieldOffsetX || e.GetPosition(this).Y < fieldOffsetY)
            {
                e.Handled = false;
                return;
            }

            int gridX = (int)(e.GetPosition(this).X - fieldOffsetX) / blockSize;
            int gridY = (int)(e.GetPosition(this).Y - fieldOffsetY) / blockSize;

            if (start)
            {
                stopwatch.Start();
                running = true;
                Update();
                start = false;
            }

            CheckBlock(gridX, gridY);
            CheckMines();
        }

        private void Mainwindow_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // check if mouse event in playfield
            if (e.GetPosition(this).X < fieldOffsetX || e.GetPosition(this).Y < fieldOffsetY)
            {
                e.Handled = false;
                return;
            }

            int gridX = (int)(e.GetPosition(this).X - fieldOffsetX) / blockSize;
            int gridY = (int)(e.GetPosition(this).Y - fieldOffsetY) / blockSize;

            if (running)
            {
                SwitchFlag(gridX, gridY);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            Reset();
        }
    }
}
