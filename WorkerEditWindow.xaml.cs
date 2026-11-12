using System;
using System.Windows;

namespace CircusApp
{
    public partial class WorkerEditWindow : Window
    {
        private int? _editId = null;

        public WorkerEditWindow()
        {
            InitializeComponent();
            Title = "Добавление работника";
        }

        public WorkerEditWindow(Работники worker)
        {
            InitializeComponent();
            Title = "Редактирование работника";
            _editId = worker.ID_Работника;

            LastNameTB.Text = worker.Фамилия;
            FirstNameTB.Text = worker.Имя;
            MiddleNameTB.Text = worker.Отчество;
            BirthYearTB.Text = worker.Год_рождения.ToString();
            HireYearTB.Text = worker.Год_поступления.ToString();
            ExperienceTB.Text = worker.Стаж?.ToString() ?? "0";
            PositionTB.Text = worker.Должность;
            GenderTB.Text = worker.Пол;
            PhoneTB.Text = worker.Телефон;
            CityTB.Text = worker.Город;
            AddressTB.Text = worker.Адрес;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTB.Text) || string.IsNullOrWhiteSpace(FirstNameTB.Text))
            {
                MessageBox.Show("Заполните фамилию и имя!");
                return;
            }

            if (!int.TryParse(BirthYearTB.Text, out int birthYear) || !int.TryParse(HireYearTB.Text, out int hireYear))
            {
                MessageBox.Show("Год рождения и год поступления должны быть числами!");
                return;
            }

            int.TryParse(ExperienceTB.Text, out int experience);

            try
            {
                using (var context = new CircusEntities())
                {
                    Работники worker;
                    if (_editId == null)
                    {
                        worker = new Работники();
                        context.Работники.Add(worker);
                    }
                    else
                    {
                        worker = context.Работники.Find(_editId);
                        if (worker == null) return;
                    }

                    worker.Фамилия = LastNameTB.Text.Trim();
                    worker.Имя = FirstNameTB.Text.Trim();
                    worker.Отчество = string.IsNullOrWhiteSpace(MiddleNameTB.Text) ? null : MiddleNameTB.Text.Trim();
                    worker.Год_рождения = birthYear;
                    worker.Год_поступления = hireYear;
                    worker.Стаж = experience;
                    worker.Должность = string.IsNullOrWhiteSpace(PositionTB.Text) ? null : PositionTB.Text.Trim();
                    worker.Пол = string.IsNullOrWhiteSpace(GenderTB.Text) ? null : GenderTB.Text.Trim().ToUpper();
                    worker.Телефон = string.IsNullOrWhiteSpace(PhoneTB.Text) ? null : PhoneTB.Text.Trim();
                    worker.Город = string.IsNullOrWhiteSpace(CityTB.Text) ? null : CityTB.Text.Trim();
                    worker.Адрес = string.IsNullOrWhiteSpace(AddressTB.Text) ? null : AddressTB.Text.Trim();

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