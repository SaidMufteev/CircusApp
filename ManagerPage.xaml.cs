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
                    WorkersGrid.ItemsSource = context.Работники.ToList();  // только чтение
                    OrdersGrid.ItemsSource = context.Заказы.ToList();      // только чтение

                    TotalOrdersTB.Text = $"Заказов: {context.Заказы.Count()}";
                    TotalRevenueTB.Text = $"Выручка: {(context.Заказы.Select(o => (decimal?)o.Сумма).Sum() ?? 0m):C}";
                    TotalClientsTB.Text = $"Клиентов: {context.Клиенты.Count()}";
                    TotalShowsTB.Text = $"Представлений: {context.Представления.Count()}";
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
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
            if (MessageBox.Show("Удалить представление?", "Подтверждение",
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

        // ---- Гастроли ----
        private void AddTourBtn_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Форма добавления гастроли (TODO)");

        private void EditTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToursGrid.SelectedItem == null) return;
            MessageBox.Show("Форма редактирования гастроли (TODO)");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
}