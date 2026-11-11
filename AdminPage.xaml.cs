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
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        // ---- Пользователи ----
        private void AddUserBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Форма добавления пользователя (TODO)");

        private void EditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null) { MessageBox.Show("Выберите пользователя."); return; }
            MessageBox.Show("Форма редактирования пользователя (TODO)");
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

        // ---- Представления ----
        private void AddShowBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Форма добавления представления (TODO)");

        private void EditShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowsGrid.SelectedItem == null) return;
            MessageBox.Show("Форма редактирования представления (TODO)");
        }

        private void DeleteShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowsGrid.SelectedItem == null) { MessageBox.Show("Выберите представление."); return; }
            if (MessageBox.Show("Удалить выбранное представление?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
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

        // ---- Работники ----
        private void AddWorkerBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Форма добавления работника (TODO)");

        private void EditWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersGrid.SelectedItem == null) return;
            MessageBox.Show("Форма редактирования работника (TODO)");
        }

        private void DeleteWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersGrid.SelectedItem == null) return;
            if (MessageBox.Show("Удалить работника?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
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

        // ---- Гастроли ----
        private void AddTourBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Форма добавления гастроли/сеанса (TODO)");

        private void EditTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToursGrid.SelectedItem == null) return;
            MessageBox.Show("Форма редактирования гастроли/сеанса (TODO)");
        }

        // ---- Заказы ----
        private void CancelOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) { MessageBox.Show("Выберите заказ."); return; }
            if (MessageBox.Show("Отменить выбранный заказ?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
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