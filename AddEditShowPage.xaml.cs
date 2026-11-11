using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CircusApp
{
    public partial class AddEditShowPage : Page
    {
        private Представления _currentShow;
        private string _selectedImagePath = null; // временное хранение пути к выбранному файлу

        public AddEditShowPage(Представления selectedShow)
        {
            InitializeComponent();

            if (selectedShow != null)
                _currentShow = selectedShow;
            else
                _currentShow = new Представления();

            DataContext = _currentShow;

            // Отображаем текущее изображение, если оно есть
            if (!string.IsNullOrEmpty(_currentShow.ImagePath))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentShow.ImagePath);
                if (File.Exists(fullPath))
                {
                    ImgPreview.Source = new BitmapImage(new Uri(fullPath));
                }
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Выберите изображение для представления"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;
                // Показываем превью
                ImgPreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            _selectedImagePath = null;
            _currentShow.ImagePath = null;
            ImgPreview.Source = new BitmapImage(new Uri("pack://application:,,,/res/image/picture.png"));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверка обязательных полей (все, кроме изображения)
            if (string.IsNullOrWhiteSpace(_currentShow.Название))
                errors.AppendLine("Укажите название представления");

            if (string.IsNullOrWhiteSpace(_currentShow.Жанр))
                errors.AppendLine("Укажите жанр представления");

            if (string.IsNullOrWhiteSpace(_currentShow.Тип))
                errors.AppendLine("Укажите тип представления");

            if (string.IsNullOrWhiteSpace(_currentShow.Автор))
                errors.AppendLine("Укажите автора представления");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка уникальности названия
            using (var context = new CircusEntities())
            {
                var existing = context.Представления
                    .FirstOrDefault(s => s.Название == _currentShow.Название
                                      && s.ID_Представления != _currentShow.ID_Представления);

                if (existing != null)
                {
                    MessageBox.Show("Представление с таким названием уже существует.");
                    return;
                }
            }

            try
            {
                // Если пользователь выбрал новое фото, копируем его в папку res/image
                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(_selectedImagePath);
                    string destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "image");

                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    string destPath = Path.Combine(destFolder, fileName);
                    File.Copy(_selectedImagePath, destPath, true);

                    // Сохраняем относительный путь в БД
                    _currentShow.ImagePath = "res/image/" + fileName;
                }

                using (var context = new CircusEntities())
                {
                    if (_currentShow.ID_Представления == 0)
                        context.Представления.Add(_currentShow);
                    else
                        context.Entry(_currentShow).State = System.Data.Entity.EntityState.Modified;

                    context.SaveChanges();
                }

                MessageBox.Show("Информация сохранена.");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}