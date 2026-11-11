using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class OrderPage : Page
    {
        private Представления _currentShow;
        private List<Программа> _sessions;

        public OrderPage(Представления selectedShow)
        {
            InitializeComponent();
            _currentShow = selectedShow;
            TbShowName.Text = _currentShow.Название;

            LoadSessions();
            LoadClients();
        }

        private void LoadSessions()
        {
            using (var context = new CircusEntities())
            {
                _sessions = context.Программа
                    .Where(p => p.ID_Представления == _currentShow.ID_Представления)
                    .OrderBy(p => p.Дата_время)
                    .ToList();

                ComboSession.Items.Clear();
                foreach (var session in _sessions)
                {
                    ComboSession.Items.Add(new ComboBoxItem
                    {
                        Content = $"{session.Дата_время:dd.MM.yyyy HH:mm} — {session.Цена_билета:C}",
                        Tag = session
                    });
                }
            }
        }

        private void LoadClients()
        {
            using (var context = new CircusEntities())
            {
                var clients = context.Клиенты
                    .Select(c => new { c.ID_Клиента, ФИО = c.Фамилия + " " + c.Имя + " " + c.Отчество })
                    .ToList();

                ComboClient.ItemsSource = clients;
                ComboClient.DisplayMemberPath = "ФИО";
                ComboClient.SelectedValuePath = "ID_Клиента";
                ComboClient.SelectedIndex = 0;
            }
        }

        private void ComboSession_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void TbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            if (ComboSession.SelectedItem == null) return;

            var selectedItem = ComboSession.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            var session = selectedItem.Tag as Программа;
            if (session == null) return;

            if (int.TryParse(TbQuantity.Text, out int quantity) && quantity > 0)
            {
                decimal total = session.Цена_билета * quantity;
                TbTotal.Text = total.ToString("C");
            }
            else
            {
                TbTotal.Text = "0,00 ₽";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboSession.SelectedItem == null)
            {
                MessageBox.Show("Выберите сеанс.");
                return;
            }

            if (ComboClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }

            if (!int.TryParse(TbQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Укажите корректное количество билетов (целое число больше 0).");
                return;
            }

            var selectedItem = ComboSession.SelectedItem as ComboBoxItem;
            var session = selectedItem.Tag as Программа;
            if (session == null)
            {
                MessageBox.Show("Ошибка выбора сеанса.");
                return;
            }

            int clientId = (int)ComboClient.SelectedValue;

            try
            {
                using (var context = new CircusEntities())
                {
                    var order = new Заказы
                    {
                        ID_Клиента = clientId,
                        Дата_заказа = DateTime.Now,
                        Сумма = session.Цена_билета * quantity,
                        Статус = "Новый"
                    };
                    context.Заказы.Add(order);
                    context.SaveChanges();

                    var ticket = new Билеты
                    {
                        ID_Заказа = order.ID_Заказа,
                        ID_Сеанса = session.ID_Сеанса,
                        Количество = quantity,
                        Цена = session.Цена_билета
                    };
                    context.Билеты.Add(ticket);
                    context.SaveChanges();
                }

                MessageBox.Show($"Заказ оформлен! Сумма: {session.Цена_билета * quantity:C}");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}