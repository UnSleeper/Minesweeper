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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int field = 9;
        public int bombs = 9;

        public MainWindow()
        {
            InitializeComponent();
        }

        public bool StarGame(int size, int bomb)
        {
            Random rand = new Random();
            List<List<ButtonField>> rows = new List<List<ButtonField>>();
            for (int i = 0; i < size; i++)
            {
                rows.Add(new List<ButtonField>());
                for (int j = 0; j < size; j++)
                {
                    ButtonField button = new ButtonField();
                    button.Width = 20;
                    button.Height = 20;
                    button.x = j;
                    button.y = i;
                    button.Click += FieldClickEvent;
                    button.MouseRightButtonUp += FieldLockEvent;
                    if (rand.Next(0, size*size) < bomb)
                    {
                        button.isBomb = true;
                    }
                    rows[i].Add(button);
                }
            }
            BombBox.ItemsSource = rows;
            return true;
        }

        class ButtonField:Button
        {
            public int x;
            public int y;
            public bool isBomb;
            public bool isLock;
            public bool isOpen;
        }

        private void FieldLockEvent(object sender, MouseButtonEventArgs e)
        {
            if(((ButtonField)sender).isOpen) return;
            ((ButtonField)sender).isLock = !((ButtonField)sender).isLock;
            if(((ButtonField)sender).isLock)
            {
                ((ButtonField)sender).Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            }
            else
            {
                ((ButtonField)sender).Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            }
        }

        private void FieldClickEvent(object sender, RoutedEventArgs e)
        {
            if (((ButtonField)sender).isLock) return;
            if (((ButtonField)sender).isBomb)
            {
                LoseGame(sender);
            }
            else
            {
                ClearField(sender);
            }
        }

        private void LoseGame(object sender)
        {
            
            List<List<ButtonField>> rows = (List<List<ButtonField>>)BombBox.ItemsSource;
            foreach (List<ButtonField> cols in rows)
            {
                foreach (ButtonField btn in cols)
                {
                    btn.Click -= FieldClickEvent;
                    btn.MouseRightButtonUp -= FieldLockEvent;
                    if (btn.isBomb)
                    {
                        btn.Content = "*";
                        btn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    }
                }
            }
            ((ButtonField)sender).Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            BombBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            //(ButtonField)sender).isOpen = true;
        }
        private void ClearField(object sender)
        {
            ((ButtonField)sender).isOpen = true;
            ((ButtonField)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            ((ButtonField)sender).Content = CountNearbyBombs(sender);
        }
        private int CountNearbyBombs(object sender)
        {
            return 0;
        }

        private void MenuItem_Reload(object sender, RoutedEventArgs e)
        {

            MenuItem menuItem = (MenuItem)sender;
            MessageBox.Show(menuItem.Header.ToString());
        }

        private void MenuItem_Easy(object sender, RoutedEventArgs e)
        {
            field = 9;
            bombs = 9;
            StarGame(field, bombs);
        }

        private void MenuItem_Normal(object sender, RoutedEventArgs e)
        {
            field = 16;
            bombs = 50;
            StarGame(field, bombs);
        }

        private void MenuItem_Hard(object sender, RoutedEventArgs e)
        {
            field = 30;
            bombs = 270;
            StarGame(field, bombs);
        }

        private void MenuItem_Custom(object sender, RoutedEventArgs e)
        {
            CustomGameWindow customGameWindow = new CustomGameWindow();

            MessageBox.Show(customGameWindow.fieldBox.ToString());
            MessageBox.Show(customGameWindow.bombBox.ToString());
            /*if (customGameWindow.ShowDialog() == true)
            {
                field = Int16.Parse(customGameWindow.fieldBox.ToString());
                bombs = Int16.Parse(customGameWindow.bombBox.ToString());
            }*/

        }
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
