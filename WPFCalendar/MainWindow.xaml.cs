using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtp.SelectedDate = DateTime.Now;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            lblMonth.Content = dtp.SelectedDate.Value.ToString("yyyy年MM月");
            RenderDates(dtp.SelectedDate);
        }

        private void RenderDates(DateTime? date = null)
        {
            this.gridCalendar.Children.RemoveRange(7, this.gridCalendar.Children.Count - 7);
            //this.gridCalendar.Children.Clear();
            foreach (var day in GetDates(date ?? DateTime.Now))
            {
                this.gridCalendar.Children.Add(day);
            }

#if DEBUG
            if (this.gridCalendar.Children.Count != 49)
            {
                MessageBox.Show("something wrong!");
            }
#endif
        }

        private IEnumerable<FrameworkElement> GetDates(DateTime date)
        {
            var month = new DateTime(date.Year, date.Month, 1);
            var currentMonthDays = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(date.Year, date.Month);

            // grid has 6 rows for date, so all the date elements count should be 42.
            var list = new List<FrameworkElement>(42);

            // row 0 is for title. date starts with row 1.
            var row = 1;

            // fill last month
            var lastMonthDays = (int)month.DayOfWeek;
            list.AddRange(FillMonth(month.AddDays(-lastMonthDays), lastMonthDays, ref row, false));

            // fill current month
            list.AddRange(FillMonth(month, currentMonthDays, ref row));

            // fill next month
            var nextMonth = month.AddMonths(1);
            list.AddRange(FillMonth(nextMonth, 42 - lastMonthDays - currentMonthDays, ref row, false));

            return list;
        }

        private FrameworkElement[] FillMonth(DateTime baseTime, int days, ref int row, bool active = true)
        {
            var els = new FrameworkElement[days];
            for (int i = 0; i < days; i++)
            {
                var day = baseTime.AddDays(i);
                var dow = day.DayOfWeek;
                els[i] = CreateElement((int)dow, row, day, active);
                if (dow == DayOfWeek.Saturday)
                {
                    row++;
                }
            }

            return els;
        }

        private FrameworkElement CreateElement(int column, int row, DateTime day, bool active = true)
        {
            void El_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                dtp.SelectedDate = (DateTime?)((FrameworkElement)sender).Tag;
            }

            var el = new Label
            {
                Content = day.Day.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Tag = day
            };
            el.SetValue(Grid.ColumnProperty, column);
            el.SetValue(Grid.RowProperty, row);
            if (!active)
            {
                el.Foreground = Brushes.Gray;
            }

            el.MouseDoubleClick += El_MouseDoubleClick;

            return el;
        }
    }
}
