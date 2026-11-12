using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class ManagerPage : Page
    {
        public ManagerPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new CircusEntities())
                {
                    ShowsGrid.ItemsSource = context.Представления.ToList();
                    ToursGrid.ItemsSource = context.Расписание_Гастролей.ToList();
                    WorkersGrid.ItemsSource = context.Работники.ToList();
                    OrdersGrid.ItemsSource = context.Заказы.ToList();

                    TotalOrdersTB.Text = $"Заказов: {context.Заказы.Count()}";
                    TotalRevenueTB.Text = $"Выручка: {(context.Заказы.Select(o => (decimal?)o.Сумма).Sum() ?? 0m):C}";
                    TotalClientsTB.Text = $"Клиентов: {context.Клиенты.Count()}";
                    TotalShowsTB.Text = $"Представлений: {context.Представления.Count()}";
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void AddShowBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new ShowEditWindow();
            if (win.ShowDialog() == true) LoadData();
        }

        private void EditShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowsGrid.SelectedItem == null) return;
            var win = new ShowEditWindow((Представления)ShowsGrid.SelectedItem);
            if (win.ShowDialog() == true) LoadData();
        }

        private void DeleteShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowsGrid.SelectedItem == null) return;
            if (MessageBox.Show("Удалить представление?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try
            {
                var sel = (Представления)ShowsGrid.SelectedItem;
                using (var context = new CircusEntities())
                {
                    var db = context.Представления.Find(sel.ID_Представления);
                    if (db != null) { context.Представления.Remove(db); context.SaveChanges(); }
                }
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
        }

        private void AddTourBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new ScheduleEditWindow();
            if (win.ShowDialog() == true) LoadData();
        }

        private void EditTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToursGrid.SelectedItem == null) return;
            var win = new ScheduleEditWindow((Расписание_Гастролей)ToursGrid.SelectedItem);
            if (win.ShowDialog() == true) LoadData();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
}