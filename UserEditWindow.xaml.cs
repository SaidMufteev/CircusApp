using System;
using System.Linq;
using System.Windows;

namespace CircusApp
{
    public partial class UserEditWindow : Window
    {
        private int? _editId = null;

        public UserEditWindow()
        {
            InitializeComponent();
            Title = "Добавление пользователя";
            LoadComboBoxes();
        }

        public UserEditWindow(Пользователи user)
        {
            InitializeComponent();
            Title = "Редактирование пользователя";
            _editId = user.ID_Пользователя;

            LoadComboBoxes();

            // Заполняем поля существующими данными
            LoginTB.Text = user.Логин;
            PasswordTB.Text = user.Пароль;
            ActiveCB.IsChecked = user.Активен;

            RoleCB.SelectedValue = user.ID_Роли;
            WorkerCB.SelectedValue = user.ID_Работника;
            ClientCB.SelectedValue = user.ID_Клиента;
        }

        private void LoadComboBoxes()
        {
            using (var context = new CircusEntities())
            {
                // Загружаем роли
                RoleCB.ItemsSource = context.Роли_Пользователей.ToList();
                RoleCB.SelectedValuePath = "ID_Роли";

                // Загружаем работников
                WorkerCB.ItemsSource = context.Работники.ToList();
                WorkerCB.SelectedValuePath = "ID_Работника";

                // Загружаем клиентов
                ClientCB.ItemsSource = context.Клиенты.ToList();
                ClientCB.SelectedValuePath = "ID_Клиента";
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordTB.Text) || RoleCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните логин, пароль и выберите роль!");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    // Проверка на уникальность логина
                    string login = LoginTB.Text.Trim();
                    bool loginExists = context.Пользователи.Any(u => u.Логин == login && u.ID_Пользователя != _editId);
                    if (loginExists)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                        return;
                    }

                    Пользователи user;
                    if (_editId == null)
                    {
                        user = new Пользователи();
                        context.Пользователи.Add(user);
                    }
                    else
                    {
                        user = context.Пользователи.Find(_editId);
                        if (user == null) return;
                    }

                    user.Логин = login;
                    user.Пароль = PasswordTB.Text.Trim();
                    user.ID_Роли = (int)RoleCB.SelectedValue;
                    user.Активен = ActiveCB.IsChecked ?? false;

                    // Привязка к работнику или клиенту (если выбраны)
                    user.ID_Работника = WorkerCB.SelectedValue as int?;
                    user.ID_Клиента = ClientCB.SelectedValue as int?;

                    context.SaveChanges();
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