using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text.Trim();
            string password = PassPB.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (var context = new CircusEntities())
            {
                var user = context.Пользователи
                    .FirstOrDefault(u => u.Логин == login
                                      && u.Пароль == password
                                      && u.Активен == true);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }

                CurrentUser.ID_Пользователя = user.ID_Пользователя;
                CurrentUser.Логин = user.Логин;
                CurrentUser.ID_Роли = user.ID_Роли;
                CurrentUser.ID_Работника = user.ID_Работника;
                CurrentUser.ID_Клиента = user.ID_Клиента;

                switch (user.ID_Роли)
                {
                    case 1: Manager.MainFrame.Navigate(new AdminPage()); break;
                    case 2: Manager.MainFrame.Navigate(new ManagerPage()); break;
                    case 3: Manager.MainFrame.Navigate(new CashierPage()); break;
                    case 4: Manager.MainFrame.Navigate(new ArtistPage()); break;
                    case 5: Manager.MainFrame.Navigate(new ClientPage()); break;
                    default: MessageBox.Show("Неизвестная роль"); break;
                }
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new RegisterPage());
        }
    }
}