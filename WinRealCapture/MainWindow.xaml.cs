using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinRealCapture.Models;

namespace WinRealCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // エラーラベルは非表示にしておく
            HideError();

            // ユーザデータから前回のディレクトリ読み出し
            try
            {
                SavingDirectoryTextBox.Text = Properties.Settings.Default["SavingDirectory"].ToString();
            }
            catch (Exception)
            {
            }

            // キーボードフック開始
            KeyboardHook.StartHook(OnCtrlF2);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // キーボードフック終了
            KeyboardHook.EndHook();
        }

        // エラー表示
        private void ShowError(string error)
        {
            ErrorLabel.Visibility = Visibility.Visible;
            ErrorLabel.Content = error;
        }
        private void HideError()
        {
            ErrorLabel.Visibility = Visibility.Collapsed;
            ErrorLabel.Content = "";
        }

        // Ctrl + F2 が押されたときに呼ばれるところ
        private void OnCtrlF2()
        {
            // DoCapture();
            Debug.WriteLine("Ctrl+F2");
            HideError();
            try
            {
                string savingDirectory = SavingDirectoryTextBox.Text;

                // ディレクトリ有無チェック
                if (!Directory.Exists(savingDirectory))
                {
                    throw new Exception(string.Format("SavingDirectory \"{0}\" not found", savingDirectory));
                }

                // キャプチャ実施
                Capture.CaptureActiveWindow(savingDirectory);
            }
            catch(Exception ex)
            {
                ShowError("CaptureError: " + ex.Message);
            }
        }



        private void SavingDirectorySelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select saving directory";
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.SelectedPath = SavingDirectoryTextBox.Text;
            dialog.ShowNewFolderButton = true;
            var ret = dialog.ShowDialog();
            if(ret== System.Windows.Forms.DialogResult.OK)
            {
                SavingDirectoryTextBox.Text = dialog.SelectedPath;

                // ユーザデータとして保存しておく
                Properties.Settings.Default["SavingDirectory"] = dialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
