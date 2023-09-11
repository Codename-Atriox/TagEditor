using System;
using System.Collections.Generic;
using System.IO;
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
using TagEditor.UI.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Interop;
using System.Timers;
using System.Windows.Threading;
using static Infinite_module_test.tag_structs;

namespace TagEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            init_strings(); // optional, but loads all tagnames & unhashed stringID's, to make the system easier to navigate

            StateChanged += MainWindowStateChangeRaised;

            Active_TagExplorer = new(this); explorer_socket.Children.Add(Active_TagExplorer);
            Active_TagViewer = new(this); tag_socket.Children.Add(Active_TagViewer);

            // setup status indicator
            var heart_monitor = new System.Windows.Threading.DispatcherTimer();
            heart_monitor.Tick += new EventHandler((sender, e) => heartbeat());
            heart_monitor.Interval = new TimeSpan(0, 0, 1);
            heart_monitor.Start();
        }
        #region WindowBar_funcs
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e) { SystemCommands.MinimizeWindow(this); }
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e) { SystemCommands.MaximizeWindow(this); }
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e) { SystemCommands.RestoreWindow(this); }
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e) { SystemCommands.CloseWindow(this); }

        // State change
        private void MainWindowStateChangeRaised(object? sender, EventArgs e){
            if (WindowState == WindowState.Maximized){
                main_grid.Margin = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }else{
                main_grid.Margin = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
        }}
        #endregion

        private HexViewer Active_HexViewer;
        private TagExplorer Active_TagExplorer;
        private TagViewer Active_TagViewer;

        public string plugins_path = "C:\\Users\\Joe bingle\\Downloads\\plugins";

        #region ERROR_HANDLING
        short Error_status = (short)error_level.NONE;
        short Seconds_displayed = 0;
        byte heartbeat_tick = 0;
        private SolidColorBrush[] e_colors = new SolidColorBrush[] { new SolidColorBrush(Color.FromArgb(0xFF, 0x42, 0x42, 0x42)), 
            new SolidColorBrush(Color.FromArgb(0xFF, 0x7B, 0x60, 0x00)), new SolidColorBrush(Color.FromArgb(0xFF, 0x7B, 0x00, 0x00))}; 
        public enum error_level{NONE=-1, NOTE=0, WARNING=1, ERROR=2}
        public void DisplayNote(string note, Exception? ex, error_level Note_type){ // potential error, display on debug'o'meter
            DebugText.Text = note;
            DebugPanel.Background =  e_colors[(int)Note_type];
            Error_status = (short)Note_type;
            if (ex != null) Error(note, ex);
        }
        public void Error(string error, Exception ex){ // actual error, calls popup window
            ErrorWind error_display = new(error, ex);
            error_display.Show();
            error_display.Focus();
        }
        private void DebugPanel_MouseDown(object sender, MouseButtonEventArgs e){ // double click to clear status
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) ClearStatus();
        }
        private void ClearStatus(){
            DebugText.Text = "Running...";
            DebugPanel.Background = e_colors[(int)error_level.NOTE];
            Error_status = (short)error_level.NONE;
            Seconds_displayed = 0;
        }
        private void heartbeat(){
            if (Error_status > -1){
                Seconds_displayed++;
                if (Seconds_displayed > 3) ClearStatus();
                else return;
            }
            heartbeat_tick++;
            if (heartbeat_tick == 4) heartbeat_tick = 0;
            DebugText.Text = "Running" + new string('.', heartbeat_tick);
        }
        #endregion


        public void TagExplorer_OpenDirectory(object sender, RoutedEventArgs e){
            // TODO: if active explorer is null, open a new one
            Active_TagExplorer.OpenDirectory(true);
        }
        private void Button_OpenTag(object sender, RoutedEventArgs e){
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true) TagViewer_OpenTag(dlg.FileName);
        }
        public void TagViewer_OpenTag(string tag_path){
            // TODO: if active tag viewer is null, open a new one
            Active_TagViewer.OpenTag(tag_path, plugins_path);
        }
        public void TagViewer_OpenModuleTag(directory_item tag_item)
        {
            // TODO: if active tag viewer is null, open a new one
            Active_TagViewer.OpenModuleTag(tag_item, plugins_path);
        }

    }
}
