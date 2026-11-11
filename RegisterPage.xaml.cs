using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string lastName = TbLastName.Text.Trim();
            string firstName = TbFirstName.Text.Trim();
            string middleName = TbMiddleName.Text.Trim();
            string phone = TbPhone.Text.Trim();
            string email = TbEmail.Text.Trim();
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password.Trim();

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните обязательные поля: фамилия, имя, логин, пароль.");
                return;
            }

            using (var context = new CircusEntities())
            {
                if (context.Пользователи.Any(u => u.Логин == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }

                if (!string.IsNullOrEmpty(phone) &&
                    context.Клиенты.Any(c => c.Телефон == phone))
                {
                    MessageBox.Show("Клиент с таким телефоном уже существует.");
                    return;
                }

                var client = new Клиенты
                {
                    Фамилия = lastName,
                    Имя = firstName,
                    Отчество = string.IsNullOrEmpty(middleName) ? null : middleName,
                    Телефон = string.IsNullOrEmpty(phone) ? null : phone,
                    Email = string.IsNullOrEmpty(email) ? null : email
                };
                context.Клиенты.Add(client);
                context.SaveChanges();

                var user = new Пользователи
                {
                    Логин = login,
                    Пароль = password,
                    ID_Роли = 5,
                    ID_Клиента = client.ID_Клиента,
                    Активен = true
                };
                context.Пользователи.Add(user);
                context.SaveChanges();
            }

            MessageBox.Show("Регистрация успешна! Теперь вы можете войти.");
            Manager.MainFrame.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}