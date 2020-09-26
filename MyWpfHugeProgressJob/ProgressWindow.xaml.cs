using System;
using System.Windows;

namespace MyWpfHugeProgressJob
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public event EventHandler Cancel;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Button_Cancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(null, null);
        }

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Window_IsVisibleChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Top = Owner.Top + (Owner.Height - Height) / 2;
            Left = Owner.Left + (Owner.Width - Width) / 2;
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// UpDate
        /// </summary>
        public void ProgressBarUpDate()
        {
            TheProgressBar.Dispatcher.Invoke((Action)(() => TheProgressBar.Value++));
        }

        /// <summary>
        /// ProgressBarReset
        /// </summary>
        public void ProgressBarReset()
        {
            TheProgressBar.Value = TheProgressBar.Minimum;
        }

        /// <summary>
        /// ProgressSet
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public void ProgressSet(int max, int min = 0)
        {
            TheProgressBar.Maximum = max;
            TheProgressBar.Minimum = min;
        }
    }
}
