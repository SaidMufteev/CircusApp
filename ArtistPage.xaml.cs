using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircusApp
{
    public partial class ArtistPage : Page
    {
        public ArtistPage()
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
                    int workerId = CurrentUser.ID_Работника ?? 0;

                    var showIds = context.Группа_Представления
                        .Where(g => g.ID_Работника == workerId)
                        .Select(g => g.ID_Представления)
                        .ToList();

                    MyShowsGrid.ItemsSource = context.Представления
                        .Where(p => showIds.Contains(p.ID_Представления))
                        .ToList();

                    MyScheduleGrid.ItemsSource = context.Расписание_Гастролей
                        .Where(s => showIds.Contains(s.ID_Представления))
                        .ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
}