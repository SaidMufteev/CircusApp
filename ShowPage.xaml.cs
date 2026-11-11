using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CircusApp
{
    public partial class ShowPage : Page
    {
        private int CountRecords;
        private int CountPage;
        private int CurrentPage = 0;
        private List<Представления> CurrentPageList = new List<Представления>();
        private List<Представления> TableList;

        public ShowPage()
        {
            InitializeComponent();
            LoadGenresAndTypes();
            RButtonUp.IsChecked = true;
            UpdateShows();
        }

        private void LoadGenresAndTypes()
        {
            using (var context = new CircusEntities())
            {
                var genres = context.Представления.Select(s => s.Жанр).Distinct().OrderBy(g => g).ToList();
                ComboGenre.Items.Add("Все");
                foreach (var g in genres)
                    if (!string.IsNullOrEmpty(g)) ComboGenre.Items.Add(g);
                ComboGenre.SelectedIndex = 0;

                var types = context.Представления.Select(s => s.Тип).Distinct().OrderBy(t => t).ToList();
                ComboType.Items.Add("Все");
                foreach (var t in types)
                    if (!string.IsNullOrEmpty(t)) ComboType.Items.Add(t);
                ComboType.SelectedIndex = 0;
            }
        }

        private void UpdateShows()
        {
            if (ComboGenre.SelectedIndex == -1 || ComboType.SelectedIndex == -1) return;

            using (var context = new CircusEntities())
            {
                var shows = context.Представления.ToList();

                if (ComboGenre.SelectedIndex > 0)
                {
                    string genre = ComboGenre.SelectedItem.ToString();
                    shows = shows.Where(s => s.Жанр == genre).ToList();
                }

                if (ComboType.SelectedIndex > 0)
                {
                    string type = ComboType.SelectedItem.ToString();
                    shows = shows.Where(s => s.Тип == type).ToList();
                }

                if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
                {
                    string search = TBoxSearch.Text.ToLower();
                    shows = shows.Where(s => s.Название.ToLower().Contains(search)).ToList();
                }

                if (RButtonUp.IsChecked == true)
                    shows = shows.OrderBy(s => s.Название).ToList();
                else if (RButtonDown.IsChecked == true)
                    shows = shows.OrderByDescending(s => s.Название).ToList();

                TableList = shows;
                ChangePage(0, 0);
            }
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
                CountPage = CountRecords / 10 + 1;
            else
                CountPage = CountRecords / 10;

            bool ifUpdate = true;
            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage < CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                        CurrentPageList.Add(TableList[i]);
                }
                else ifUpdate = false;
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else ifUpdate = false;
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else ifUpdate = false;
                        break;
                }
            }

            if (ifUpdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                    PageListBox.Items.Add(i);
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = "из " + CountRecords.ToString();

                ShowListView.ItemsSource = CurrentPageList;
                ShowListView.Items.Refresh();
            }
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateShows();
        private void ComboGenre_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateShows();
        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateShows();
        private void RButtonUp_Checked(object sender, RoutedEventArgs e) => UpdateShows();
        private void RButtonDown_Checked(object sender, RoutedEventArgs e) => UpdateShows();

        private void LeftDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(1, null);
        private void RightDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(2, null);

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PageListBox.SelectedItem == null) return;
            int selectedPage = Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1;
            ChangePage(0, selectedPage);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditShowPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var currentShow = (sender as Button).DataContext as Представления;
            if (currentShow == null) return;
            Manager.MainFrame.Navigate(new AddEditShowPage(currentShow));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentShow = (sender as Button).DataContext as Представления;
            if (currentShow == null) return;

            using (var context = new CircusEntities())
            {
                bool hasPrograms = context.Программа.Any(p => p.ID_Представления == currentShow.ID_Представления);
                if (hasPrograms)
                {
                    MessageBox.Show("Невозможно удалить представление, так как для него есть сеансы в программе.");
                    return;
                }
            }

            if (MessageBox.Show("Вы точно хотите удалить представление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new CircusEntities())
                    {
                        var showToDelete = context.Представления.Find(currentShow.ID_Представления);
                        if (showToDelete != null)
                        {
                            context.Представления.Remove(showToDelete);
                            context.SaveChanges();
                        }
                    }
                    MessageBox.Show("Представление удалено.");
                    UpdateShows();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var currentShow = (sender as Button).DataContext as Представления;
            if (currentShow == null) return;
            Manager.MainFrame.Navigate(new OrderPage(currentShow));
        }
    }
}