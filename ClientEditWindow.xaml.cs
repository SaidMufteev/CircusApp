using System;
using System.Windows;

namespace CircusApp
{
    public partial class ClientEditWindow : Window
    {
        private int? _editId = null;
        public int NewClientId { get; private set; }

        public ClientEditWindow()
        {
            InitializeComponent();
            Title = "Добавление клиента";
        }

        public ClientEditWindow(Клиенты client)
        {
            InitializeComponent();
            Title = "Редактирование клиента";
            _editId = client.ID_Клиента;
            NewClientId = client.ID_Клиента;

            LastNameTB.Text = client.Фамилия;
            FirstNameTB.Text = client.Имя;
            MiddleNameTB.Text = client.Отчество;
            PhoneTB.Text = client.Телефон;
            EmailTB.Text = client.Email;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTB.Text) || string.IsNullOrWhiteSpace(FirstNameTB.Text))
            {
                MessageBox.Show("Заполните фамилию и имя!");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    Клиенты client;
                    if (_editId == null)
                    {
                        client = new Клиенты();
                        context.Клиенты.Add(client);
                    }
                    else
                    {
                        client = context.Клиенты.Find(_editId);
                        if (client == null) return;
                    }

                    client.Фамилия = LastNameTB.Text.Trim();
                    client.Имя = FirstNameTB.Text.Trim();
                    client.Отчество = string.IsNullOrWhiteSpace(MiddleNameTB.Text) ? null : MiddleNameTB.Text.Trim();
                    client.Телефон = string.IsNullOrWhiteSpace(PhoneTB.Text) ? null : PhoneTB.Text.Trim();
                    client.Email = string.IsNullOrWhiteSpace(EmailTB.Text) ? null : EmailTB.Text.Trim();

                    context.SaveChanges();
                    NewClientId = client.ID_Клиента;
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}