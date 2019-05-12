using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MineSweeper
{
    /// <summary>
    /// Logika interakcji dla klasy SizeDialog.xaml
    /// </summary>
    public partial class SizeDialog : Window
    {
        public SizeDialog()
        {
            InitializeComponent();
        }

        private int rows;
        private int columns;
        private int mines;
        public int Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        public int Mines
        {
            get { return mines; }
            set { mines = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(rowsSetBox.Text, out rows) || !int.TryParse(columnSetBox.Text, out columns) || !int.TryParse(minesSetBox.Text, out mines))
            {
                MessageBox.Show("Wrong input!", "Wrong input", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
            else if (Rows > 30 || Rows < 5 || Columns > 30 || Columns < 5)
            {
                MessageBox.Show("Number of rows and columns should be between 5 and 30", "Wrong input", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
            else if (mines < 1 || mines > (rows * columns / 2))
            {
                MessageBox.Show("Number of mines must be greater than 0 and lower than half fields count", "Wrong input", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
            else if (int.TryParse(rowsSetBox.Text, out rows) && int.TryParse(columnSetBox.Text, out columns) && int.TryParse(minesSetBox.Text, out mines))
            {
                DialogResult = true; 
            }
        }
    }
}
