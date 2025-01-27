using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ToastNotificationsPleasanter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        _ = new ToastNotifications(300000);
        _ = new ScheduleAPI();
        Loaded += new RoutedEventHandler(Button_Click);
        AppButton.Click += Button_Click;
        Closing += new CancelEventHandler(MainWindow_Closing);
        ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        AppTextBlock.Text = ToastNotifications.ToastStateChange()
                            ? "リマインダーを起動しています"
                            : "リマインダーを停止しています";
    }
    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        ToastNotificationManagerCompat.Uninstall();
    }
    public static void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        var args = ToastArguments.Parse(e.Argument);
        if (args.Count is 0) return;
        Console.WriteLine("通知をクリック" + args["action"]);
        Process.Start(
            new ProcessStartInfo(ConfigurationManager.AppSettings["PageUrl"] + args["action"])
            { UseShellExecute = true }
        );
    }
}
