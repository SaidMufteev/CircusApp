using System;
using System.Linq;
using System.Windows;

namespace CircusApp
{
    public partial class ShowEditWindow : Window
    {
        private int? _editId = null; // Если null - добавляем, если есть ID - редактируем

        public ShowEditWindow() // Вызов для ДОБАВЛЕНИЯ
        {
            InitializeComponent();
            Title = "Добавление представления";
        }

        public ShowEditWindow(Представления show) // Вызов для РЕДАКТИРОВАНИЯ
        {
            InitializeComponent();
            Title = "Редактирование представления";
            _editId = show.ID_Представления;

            // Заполняем поля текущими данными
            NameTB.Text = show.Название;
            AuthorTB.Text = show.Автор;
            GenreTB.Text = show.Жанр;
            TypeTB.Text = show.Тип;
            ImageTB.Text = show.ImagePath;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                MessageBox.Show("Введите название представления!");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    Представления show;

                    if (_editId == null) // ДОБАВЛЕНИЕ
                    {
                        show = new Представления();
                        context.Представления.Add(show);
                    }
                    else // РЕДАКТИРОВАНИЕ
                    {
                        show = context.Представления.Find(_editId);
                        if (show == null) return;
                    }

                    // Записываем данные из полей
                    show.Название = NameTB.Text.Trim();
                    show.Автор = string.IsNullOrWhiteSpace(AuthorTB.Text) ? null : AuthorTB.Text.Trim();
                    show.Жанр = string.IsNullOrWhiteSpace(GenreTB.Text) ? null : GenreTB.Text.Trim();
                    show.Тип = string.IsNullOrWhiteSpace(TypeTB.Text) ? null : TypeTB.Text.Trim();
                    show.ImagePath = string.IsNullOrWhiteSpace(ImageTB.Text) ? null : ImageTB.Text.Trim();

                    context.SaveChanges();
                }
                DialogResult = true; // Сообщаем главному окну, что всё ок
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