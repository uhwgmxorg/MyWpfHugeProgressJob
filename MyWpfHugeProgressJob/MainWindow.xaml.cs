using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyWpfHugeProgressJob
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Task _task;
        CancellationTokenSource _ts;
        CancellationToken _ct;

        public virtual System.Windows.Threading.Dispatcher DispatcherObjectMainWin { get; protected set; }

        #region INotify Changed Properties  

        // Template for a new INotify Changed Property
        // for using CTRL-R-R
        private string xxx;
        public string Xxx
        {
            get { return xxx; }
            set { SetField(ref xxx, value, nameof(Xxx)); }
        }

        #endregion

        const int MAX_ITERATIONS = 20;

        ProgressWindow _progressWindow;

        /// <summary>
        /// Construtor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DispatcherObjectMainWin = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            EnableDisableButtons(false);
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_1_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            _progressWindow.Show();

            EnableDisableButtons(true);

            _progressWindow.ProgressSet(MAX_ITERATIONS * MAX_ITERATIONS * MAX_ITERATIONS);
            _progressWindow.ProgressBarReset();

            _ts = new CancellationTokenSource();
            _ct = _ts.Token;
            _task = Task.Factory.StartNew(() => JobManager(), _ct);
        }

        /// <summary>
        /// Button_2_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _ts.Cancel();
            EnableDisableButtons(false);
           _progressWindow.Hide();
        }

        /// <summary>
        /// Button_3_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (_progressWindow.IsVisible)
                _progressWindow.Hide();
            else
                _progressWindow.Show();
        }

        /// <summary>
        /// Button_Close_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _progressWindow = new ProgressWindow();
            _progressWindow.Owner = Window.GetWindow(this);
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Cancel if a TheHugeJob is still running
            if (_ts != null) _ts.Cancel();
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// JobManager
        /// </summary>
        public void JobManager()
        {

            // Call the TheHugeJob
            TheHugeJob();

            // Check in which threat we are in
            if (DispatcherObjectMainWin.Thread == Thread.CurrentThread)
                _progressWindow.Hide();
            else
                _progressWindow.Dispatcher.Invoke((Action)(() => _progressWindow.Hide()));
            // Mange button state
            EnableDisableButtons(false);
            // give a signal
            Console.Beep();
        }

        /// <summary>
        /// TheHugeJob
        /// </summary>
        public void TheHugeJob()
        {
            int i, j, k;
            Double d = 0.0;

            for(i=0; i < MAX_ITERATIONS; i++)
                for (j = 0; j < MAX_ITERATIONS; j++)
                    for (k = 0; k < MAX_ITERATIONS; k++)
                    {
                        if (_ct.IsCancellationRequested)
                        {
                            // UI thread decided to cancel
                            Debug.WriteLine(String.Format("We have a Cancellation Requested "));
                            return;
                        }
                        d++;
                        Debug.WriteLine(String.Format("i={0} j={1} k={2} d={3}",i,j,k,d));
                        _progressWindow.ProgressBarUpDate();
                    }
        }

        /// <summary>
        /// EnableDisableButtons
        /// </summary>
        /// <param name="running"></param>
        private void EnableDisableButtons(bool running)
        {
            // Check in which threat we are in
            if (DispatcherObjectMainWin.Thread == Thread.CurrentThread)
            {
                button_Start_Task.IsEnabled = !running;
                button_Cancel_Task.IsEnabled = running;
            }
            else
            {
                button_Start_Task.Dispatcher.Invoke((Action)(() => button_Start_Task.IsEnabled = !running));
                button_Start_Task.Dispatcher.Invoke((Action)(() => button_Cancel_Task.IsEnabled = running));
            }
        }

        /// <summary>
        /// SetField
        /// for INotify Changed Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
