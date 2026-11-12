using System;
using System.Linq;
using System.Windows;

namespace CircusApp
{
    public partial class ScheduleEditWindow : Window
    {
        private int? _editId = null;

        public ScheduleEditWindow()
        {
            InitializeComponent();
            Title = "Добавление в расписание";
            LoadComboBoxes();
        }

        public ScheduleEditWindow(Расписание_Гастролей schedule)
        {
            InitializeComponent();
            Title = "Редактирование расписания";
            _editId = schedule.ID_Гастроли;

            LoadComboBoxes();

            // Выделяем текущие значения в списках
            ShowCB.SelectedValue = schedule.ID_Представления;
            PlaceCB.SelectedValue = schedule.ID_Места;

            // Заполняем даты
            StartDateDP.SelectedDate = schedule.Дата_начала;
            EndDateDP.SelectedDate = schedule.Дата_окончания;
        }

        private void LoadComboBoxes()
        {
            using (var context = new CircusEntities())
            {
                // Загружаем представления
                ShowCB.ItemsSource = context.Представления.ToList();
                ShowCB.SelectedValuePath = "ID_Представления";

                // Загружаем места проведения
                PlaceCB.ItemsSource = context.Места_Гастролей.ToList();
                PlaceCB.SelectedValuePath = "ID_Места"; // Убедись, что ID называется именно так
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowCB.SelectedValue == null || PlaceCB.SelectedValue == null ||
                StartDateDP.SelectedDate == null || EndDateDP.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (StartDateDP.SelectedDate > EndDateDP.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала!");
                return;
            }

            try
            {
                using (var context = new CircusEntities())
                {
                    Расписание_Гастролей schedule;
                    if (_editId == null)
                    {
                        schedule = new Расписание_Гастролей();
                        context.Расписание_Гастролей.Add(schedule);
                    }
                    else
                    {
                        schedule = context.Расписание_Гастролей.Find(_editId);
                        if (schedule == null) return;
                    }

                    schedule.ID_Представления = (int)ShowCB.SelectedValue;
                    schedule.ID_Места = (int)PlaceCB.SelectedValue;
                    schedule.Дата_начала = StartDateDP.SelectedDate.Value;
                    schedule.Дата_окончания = EndDateDP.SelectedDate.Value;

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