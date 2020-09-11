namespace Minesweeper_WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaktionslogik für CustomRectangle.xaml
    /// </summary>
    public partial class CustomRectangle : UserControl
    {
        private bool hasMine, hasFlag, visited;
        private int posX, posY, adjacentMines;

        public CustomRectangle()
        {
            InitializeComponent();
        }

        public bool HasMine { get => hasMine; set => hasMine = value; }

        public bool IsFlagged { get => hasFlag; set => hasFlag = value; }

        public bool Uncovered { get => visited; set => visited = value; }

        ////public int PosX { get => posX; set => posX = value; }

        ////public int PosY { get => posY; set => posY = value; }

        public int AdjacentMines { get => adjacentMines; set => adjacentMines = value; }

        public void ShowNumber(bool show)
        {
            txt_number.Text = this.AdjacentMines.ToString();
            txt_number.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            ShowVisited(show);
        }

        public void ShowMine(bool show)
        {
            rec_block.Fill = show ? Brushes.Red : Brushes.DarkGray;
        }

        public void ShowFlag(bool show)
        {
            rec_block.Fill = show ? Brushes.Yellow : Brushes.DarkGray;
        }

        public void ShowVisited(bool show)
        {
            rec_block.Fill = show ? Brushes.LightGray : Brushes.DarkGray;
        }
    }
}
