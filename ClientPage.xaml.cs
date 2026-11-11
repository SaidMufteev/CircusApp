using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
            LoadShows();
            LoadMyOrders();
            LoadProfile();
        }

        private void LoadShows()
        {
            using (var context = new CircusEntities())
                ShowsGrid.ItemsSource = context.Представления.ToList();
        }

        private void ShowsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShowsGrid.SelectedItem is Представления show)
            {
                using (var context = new CircusEntities())
                {
                    SessionCB.ItemsSource = context.Расписание_Гастролей
                        .Where(s => s.ID_Представления == show.ID_Представления)
                        .ToList();
                }
            }
        }

        private void LoadMyOrders()
        {
            try
            {
                using (var context = new CircusEntities())
                {
                    int clientId = CurrentUser.ID_Клиента ?? 0;
                    MyOrdersGrid.ItemsSource = context.Заказы
                        .Where(o => o.ID_Клиента == clientId)
                        .ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void LoadProfile()
        {
            using (var context = new CircusEntities())
            {
                int clientId = CurrentUser.ID_Клиента ?? 0;
                var c = context.Клиенты.FirstOrDefault(x => x.ID_Клиента == clientId);
                if (c == null) return;
                LastNameTB.Text = c.Фамилия;
                FirstNameTB.Text = c.Имя;
                MiddleNameTB.Text = c.Отчество;
                PhoneTB.Text = c.Телефон;
                EmailTB.Text = c.Email;
            }
        }

        private void TicketsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Можно добавить авто-расчет, если нужно
        }

        private void BuyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SessionCB.SelectedItem == null || !int.TryParse(TicketsTB.Text, out int tickets) ||
                !decimal.TryParse(PriceTB.Text, out decimal price) || tickets <= 0 || price <= 0)
            {
                MessageBox.Show("Выберите сеанс, укажите количество билетов и цену.");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    var session = (Расписание_Гастролей)SessionCB.SelectedItem;

                    // 1. Создаем заказ
                    var order = new Заказы
                    {
                        ID_Клиента = CurrentUser.ID_Клиента.Value,
                        Дата_заказа = DateTime.Now,
                        Сумма = price * tickets,
                        Статус = "Оплачен"
                    };
                    context.Заказы.Add(order);
                    context.SaveChanges(); // Сохраняем, чтобы получить ID_Заказа

                    // 2. Создаем билет, привязанный к заказу
                    var ticket = new Билеты
                    {
                        ID_Заказа = order.ID_Заказа,
                        ID_Сеанса = session.ID_Гастроли, // Связываем с расписанием
                        Количество = tickets,
                        Цена = price
                    };
                    context.Билеты.Add(ticket);
                    context.SaveChanges();
                }
                MessageBox.Show("Билеты куплены!");
                TicketsTB.Text = "";
                LoadMyOrders();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка покупки: " + ex.Message); }
        }

        private void SaveProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new CircusEntities())
            {
                int clientId = CurrentUser.ID_Клиента ?? 0;
                var c = context.Клиенты.FirstOrDefault(x => x.ID_Клиента == clientId);
                if (c == null) return;
                c.Фамилия = LastNameTB.Text.Trim();
                c.Имя = FirstNameTB.Text.Trim();
                c.Отчество = string.IsNullOrWhiteSpace(MiddleNameTB.Text) ? null : MiddleNameTB.Text.Trim();
                c.Телефон = string.IsNullOrWhiteSpace(PhoneTB.Text) ? null : PhoneTB.Text.Trim();
                c.Email = string.IsNullOrWhiteSpace(EmailTB.Text) ? null : EmailTB.Text.Trim();
                context.SaveChanges();
            }
            MessageBox.Show("Профиль обновлён.");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
}