using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TestCountDown {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        DispatcherTimer tmr = new DispatcherTimer();
        DateTime endTimer;
        DateTime startTimer;
        bool startTimeWasSet = false;
        bool initializingValues = true;
        bool showMinutesInCountdown = true; // not percents of time.
        int showSecondsFrom;
        public MainWindow() {
            InitializeComponent();

            initializingValues = true;
            Tools.fillTimeDDLs(ddlStartHour, ddlStartMinutes, DateTime.Now);
            Tools.fillTimeDDLs(ddlEndHour, ddlEndMinutes, DateTime.Now.AddMinutes(50));
            initializingValues = false;
            tmr.Interval = new TimeSpan(5000000);

            //btnBegin.Content = "Start Count Down";

            // hide seconds until counter started
            grdShowSeconds.Visibility = Visibility.Collapsed;
            // start seconds hidden
            tbSecondsValue.Visibility = tbSecondsTitle.Visibility
            = Visibility.Collapsed;

            tmr.Tick += Tmr_Tick;
        }

        private void Tmr_Tick(object sender, EventArgs e) {
            TimeSpan tsTimeLeft = endTimer - DateTime.Now;
            TimeSpan tsWholeTime = endTimer - startTimer;
            double perLeft = tsTimeLeft.TotalSeconds / tsWholeTime.TotalSeconds;
            if (showMinutesInCountdown) {
                ShowMinutes(tsTimeLeft);
                grdShowSeconds.Visibility = Visibility.Visible;
            } else { // use Percentages
                ShowPercents(tsTimeLeft, tsWholeTime);
                grdShowSeconds.Visibility = Visibility.Hidden;
            }
            tbCounterValue.Foreground =
                tbSecondsValue.Foreground =
                    pbTime.Foreground = GetPercentageBrush(perLeft);

            pbTime.Value = perLeft *100;
        }

        private Brush GetPercentageBrush(double v) {
            Brush bshLateness;

            if (v > .75)
                bshLateness = Brushes.Black;
            else if (v > .50)
                bshLateness = Brushes.DarkGreen;
            else if (v > .25)
                bshLateness = Brushes.Blue;
            else
                bshLateness = Brushes.Red;

            return bshLateness;
        }

        private void ShowMinutes(TimeSpan tsTimeLeft) {
            int minutesLeft = (int)tsTimeLeft.TotalMinutes;


            if (chkShowSeconds.IsChecked == true || minutesLeft < showSecondsFrom) {
                tbSecondsValue.Visibility = tbSecondsTitle.Visibility
                    = Visibility.Visible;
                tbSecondsValue.Text = ":" + tsTimeLeft.Seconds.ToString("00");
            } else {
                tbSecondsValue.Visibility = tbSecondsTitle.Visibility
            = Visibility.Collapsed;
                if (minutesLeft > 0)
                    minutesLeft++;
            }
            tbCounterValue.Text = minutesLeft.ToString();
        }
        private void ShowPercents(TimeSpan tsTimeLeft, TimeSpan tsCompleteTime) {
            tbCounterValue.Text = String.Format("{0:0}%", tsTimeLeft.TotalSeconds / tsCompleteTime.TotalSeconds * 100);
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e) {
            btnBegin.Content = "Running";
            btnBegin.IsEnabled = false;
            DateTime currentTime = DateTime.Now;

            int endHours, endMinutes, startHours, startMinutes;
            startHours = int.Parse(ddlStartHour.SelectedValue.ToString());
            startMinutes = int.Parse(ddlStartMinutes.SelectedValue.ToString());

            endHours = int.Parse(ddlEndHour.SelectedValue.ToString());
            endMinutes = int.Parse(ddlEndMinutes.SelectedValue.ToString());

            if (startTimeWasSet) {
                startTimer = new DateTime(currentTime.Year, currentTime.Month,
                     currentTime.Day, startHours, startMinutes, 0);
            } else {
                startTimer = currentTime;
            }

            // create time to end
            endTimer = new DateTime(currentTime.Year, currentTime.Month,
                currentTime.Day, endHours, endMinutes, 0);

            // MessageBox.Show(String.Format("{0} to {1}\r\n {2}:{3}\t{4}:{5}", startTimer, endTimer, startHours, startMinutes, endHours, endMinutes));
            SetupSecondsForm(endTimer - startTimer);

            if (!tmr.IsEnabled) {
                tmr.Interval = new TimeSpan(500);
                tmr.Start();
            }

        }

        private void SetupSecondsForm(TimeSpan tsTotalCount) {
            int selectedIndex = -1;
            if (ddlMinutesToShowSeconds.SelectedIndex > -1)
                selectedIndex = ddlMinutesToShowSeconds.SelectedIndex;
            ddlMinutesToShowSeconds.Items.Clear();

            for (int m = 1; m <= tsTotalCount.TotalMinutes; m++) {
                ddlMinutesToShowSeconds.Items.Add(m);
            }
            if (selectedIndex < 0 || selectedIndex > tsTotalCount.TotalMinutes) {
                // reevaluate selected index
                if (tsTotalCount.TotalMinutes > 3) {
                    selectedIndex = 2;
                } else if (tsTotalCount.TotalMinutes > 1) {
                    selectedIndex = (int)tsTotalCount.TotalMinutes - 1;
                } else {
                    // do nothing
                    selectedIndex = -1;
                }
            }
            ddlMinutesToShowSeconds.SelectedIndex = selectedIndex;
            if (selectedIndex >= 0)
                showSecondsFrom = (int)ddlMinutesToShowSeconds.SelectedValue;
            else
                showSecondsFrom = 1;
            // show seconds options now that we have started
            grdShowSeconds.Visibility = Visibility.Visible;
        }

        private void ddlMinutesToShowSeconds_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox ddl = (ComboBox)sender;
            if (ddl.SelectedIndex >= 0)
                showSecondsFrom = (int)ddl.SelectedValue;
        }

        private void ddlTime_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox ddl = (ComboBox)sender;
            if (!initializingValues && ddl.Name.Contains("Start")) startTimeWasSet = true;
            btnBegin.IsEnabled = true;
            if (tmr.IsEnabled)
                btnBegin.Content = "Apply Changes";

        }



        private void txtNotes_GotFocus(object sender, RoutedEventArgs e) {
            TextBox owner = (TextBox)sender;
            owner.Foreground = Brushes.Black;
            if (owner.Tag == null) {
                owner.Tag = true;
                txtNotes.Text = "";
            }
        }

        private void txtNotes_LostFocus(object sender, RoutedEventArgs e) {
            TextBox owner = (TextBox)sender;
            if (string.IsNullOrEmpty(owner.Text)
                || string.IsNullOrWhiteSpace(owner.Text)) {
                owner.Tag = null;
                txtNotes.Text = "Notes go here.";
                owner.Foreground = Brushes.Gray;
            }
        }

        private void miTimerFormat_Click(object sender, RoutedEventArgs e) {
            MenuItem mi = (MenuItem)sender;
            if (mi.Tag.ToString() == "minutes") {
                showMinutesInCountdown = true;
            } else if (mi.Tag.ToString() == "percent") {
                showMinutesInCountdown = false;
            } else {
                // should not hit this.
            }
            miTimerInMinutes.IsChecked = showMinutesInCountdown;
            miTimerInPercent.IsChecked = !showMinutesInCountdown;
        }

    }
}
