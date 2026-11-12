using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class AdminPage : Page
    {
        public AdminPage()
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
                    UsersGrid.ItemsSource = context.Пользователи.ToList();
                    ShowsGrid.ItemsSource = context.Представления.ToList();
                    WorkersGrid.ItemsSource = context.Работники.ToList();
                    ToursGrid.ItemsSource = context.Расписание_Гастролей.ToList();
                    OrdersGrid.ItemsSource = context.Заказы.ToList();

                    TotalOrdersTB.Text = $"Заказов: {context.Заказы.Count()}";
                    TotalRevenueTB.Text = $"Выручка: {(context.Заказы.Select(o => (decimal?)o.Сумма).Sum() ?? 0m):C}";
                    TotalClientsTB.Text = $"Клиентов: {context.Клиенты.Count()}";
                    TotalShowsTB.Text = $"Представлений: {context.Представления.Count()}";
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new UserEditWindow();
            if (win.ShowDialog() == true) LoadData();
        }

        private void EditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null) return;
            var win = new UserEditWindow((Пользователи)UsersGrid.SelectedItem);
            if (win.ShowDialog() == true) LoadData();
        }

        private void ToggleUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null) { MessageBox.Show("Выберите пользователя."); return; }
            var user = (Пользователи)UsersGrid.SelectedItem;
            try
            {
                using (var context = new CircusEntities())
                {
                    var db = context.Пользователи.Find(user.ID_Пользователя);
                    if (db != null) { db.Активен = !db.Активен; context.SaveChanges(); }
                }
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
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

        private void AddWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new WorkerEditWindow();
            if (win.ShowDialog() == true) LoadData();
        }

        private void EditWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersGrid.SelectedItem == null) return;
            var win = new WorkerEditWindow((Работники)WorkersGrid.SelectedItem);
            if (win.ShowDialog() == true) LoadData();
        }

        private void DeleteWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersGrid.SelectedItem == null) return;
            if (MessageBox.Show("Удалить работника?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try
            {
                var sel = (Работники)WorkersGrid.SelectedItem;
                using (var context = new CircusEntities())
                {
                    var db = context.Работники.Find(sel.ID_Работника);
                    if (db != null) { context.Работники.Remove(db); context.SaveChanges(); }
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

        private void CancelOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;
            if (MessageBox.Show("Отменить заказ?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try
            {
                var sel = (Заказы)OrdersGrid.SelectedItem;
                using (var context = new CircusEntities())
                {
                    var db = context.Заказы.Find(sel.ID_Заказа);
                    if (db != null) { db.Статус = "Отменён"; context.SaveChanges(); }
                }
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка отмены: " + ex.Message); }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
}