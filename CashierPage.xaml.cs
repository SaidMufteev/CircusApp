using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class CashierPage : Page
    {
        public CashierPage()
        {
            InitializeComponent();
            LoadClients();
            LoadSessions();
            LoadMyOrders();
        }

        private void LoadClients()
        {
            using (var context = new CircusEntities())
                ClientCB.ItemsSource = context.Клиенты.ToList();
        }

        private void LoadSessions()
        {
            using (var context = new CircusEntities())
                SessionCB.ItemsSource = context.Расписание_Гастролей.ToList();
        }

        private void LoadMyOrders()
        {
            try
            {
                using (var context = new CircusEntities())
                {
                    var today = DateTime.Today;
                    MyOrdersGrid.ItemsSource = context.Заказы
                        .Where(o => o.Дата_заказа >= today)
                        .ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void NewClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new ClientEditWindow();
            if (win.ShowDialog() == true)
            {
                LoadClients();
                ClientCB.SelectedValue = win.NewClientId;
            }
        }

        private void SaveOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientCB.SelectedItem == null || SessionCB.SelectedItem == null ||
                !int.TryParse(TicketsTB.Text, out int tickets) ||
                !decimal.TryParse(PriceTB.Text, out decimal price) || tickets <= 0 || price <= 0)
            {
                MessageBox.Show("Заполните клиента, сеанс, количество и цену.");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    var client = (Клиенты)ClientCB.SelectedItem;
                    var session = (Расписание_Гастролей)SessionCB.SelectedItem;

                    var order = new Заказы
                    {
                        ID_Клиента = client.ID_Клиента,
                        Дата_заказа = DateTime.Now,
                        Сумма = price * tickets,
                        Статус = "Оформлен"
                    };
                    context.Заказы.Add(order);
                    context.SaveChanges();

                    var ticket = new Билеты
                    {
                        ID_Заказа = order.ID_Заказа,
                        ID_Сеанса = session.ID_Гастроли,
                        Количество = tickets,
                        Цена = price
                    };
                    context.Билеты.Add(ticket);
                    context.SaveChanges();
                }
                MessageBox.Show("Заказ успешно создан.");
                TicketsTB.Text = "";
                LoadMyOrders();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка сохранения: " + ex.Message); }
        }

        private void CancelOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MyOrdersGrid.SelectedItem == null) { MessageBox.Show("Выберите заказ."); return; }
            if (MessageBox.Show("Отменить заказ?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try
            {
                var sel = (Заказы)MyOrdersGrid.SelectedItem;
                using (var context = new CircusEntities())
                {
                    var db = context.Заказы.Find(sel.ID_Заказа);
                    if (db != null) { db.Статус = "Отменён"; context.SaveChanges(); }
                }
                LoadMyOrders();
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