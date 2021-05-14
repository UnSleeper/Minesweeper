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
        private int field = 9;
        private int bombs = 9;
        private int defused;
        private int needOpenFields;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StarGame(int size, int bomb)
        {
            defused = 0;
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
                    button.x = i;
                    button.y = j;
                    button.Click += FieldClickEvent;
                    button.MouseRightButtonUp += FieldLockEvent;
                    if (defused < bomb && rand.Next(0, size*size) < bomb)
                    {
                        button.isBomb = true;
                        defused++;
                    }
                    rows[i].Add(button);
                }
            }
            BombBox.ItemsSource = rows;
            needOpenFields = (size * size) - defused;
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
                if(((ButtonField)sender).isBomb) defused--;
            }
            else
            {
                ((ButtonField)sender).Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                if (((ButtonField)sender).isBomb) defused++;
            }
            CheckWinGame();
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
                CountNearbyBombs((ButtonField)sender);
            }
            CheckWinGame();
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
            MessageBox.Show("Вы проиграли");
        }

        private void CheckWinGame()
        {
            if(needOpenFields == 0 && defused == 0)
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
                            btn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        }
                    }
                }
                MessageBox.Show("Вы выиграли!");
            }
        }
        private void CountNearbyBombs(ButtonField btn)
        {
            int count = 0;
            int x = btn.x;
            int y = btn.y;
            btn.Background = new SolidColorBrush(Color.FromArgb(255, 0, 155, 0));
            btn.isOpen = true;
            needOpenFields--;

            List<List<ButtonField>> fields = (List<List<ButtonField>>)BombBox.ItemsSource;

            if (FieldExist(x, y - 1) && fields[x][y - 1].isBomb) count++;
            if (FieldExist(x, y + 1) && fields[x][y + 1].isBomb) count++;

            if (FieldExist(x - 1, y) && fields[x - 1][y].isBomb) count++;
            if (FieldExist(x + 1, y) && fields[x + 1][y].isBomb) count++;

            if (FieldExist(x - 1, y - 1) && fields[x - 1][y - 1].isBomb) count++;
            if (FieldExist(x - 1, y + 1) && fields[x - 1][y + 1].isBomb) count++;
            
            if (FieldExist(x + 1, y + 1) && fields[x + 1][y + 1].isBomb) count++;
            if (FieldExist(x + 1, y - 1) && fields[x + 1][y - 1].isBomb) count++;
            
            if (count != 0) btn.Content = count;
            else
            {
                if (FieldExist(x - 1, y) && !fields[x - 1][y].isOpen) CountNearbyBombs(fields[x - 1][y]);
                if (FieldExist(x - 1, y - 1) && !fields[x - 1][y - 1].isOpen) CountNearbyBombs(fields[x - 1][y - 1]);
                if (FieldExist(x - 1, y + 1) && !fields[x - 1][y + 1].isOpen) CountNearbyBombs(fields[x - 1][y + 1]);

                if (FieldExist(x + 1, y) && !fields[x + 1][y].isOpen) CountNearbyBombs(fields[x + 1][y]);
                if (FieldExist(x + 1, y + 1) && !fields[x + 1][y + 1].isOpen) CountNearbyBombs(fields[x + 1][y + 1]);
                if (FieldExist(x + 1, y - 1) && !fields[x + 1][y - 1].isOpen) CountNearbyBombs(fields[x + 1][y - 1]);

                if (FieldExist(x, y - 1) && !fields[x][y - 1].isOpen) CountNearbyBombs(fields[x][y - 1]);
                if (FieldExist(x, y + 1) && !fields[x][y + 1].isOpen) CountNearbyBombs(fields[x][y + 1]);
            }
        }
        private bool FieldExist(int x,int y)
        {
            return 0 <= x && x < field && 0 <= y && y < field;
        }

        private void MenuItem_Reload(object sender, RoutedEventArgs e)
        {
            StarGame(field, bombs);
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
