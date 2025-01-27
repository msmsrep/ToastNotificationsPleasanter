using Microsoft.Toolkit.Uwp.Notifications;

namespace ToastNotificationsPleasanter;

public class ToastNotifications
{
    public static System.Timers.Timer? ToastTimer { get; set; }
    public ToastNotifications(int timerInterval)
    {
        ToastTimer = new(timerInterval);
        ToastTimer.Elapsed += async (sender, e) => await SetToastAsync();
    }
    private static async Task SetToastAsync()
    {
        var scheduleList = await ScheduleAPI.GetScheduleAsync(
                            ScheduleAPI.CreateContent(100, 15));
        if (scheduleList is null) return;
        foreach (var li in scheduleList)
        {
            new ToastContentBuilder()
            .AddText(li.Title)
            .AddText(li.StartTime)
            .AddButton(new ToastButton()
                .SetContent("ページを開く")
                .AddArgument("action", li.IssueId.ToString())
            )
            .SetToastScenario(ToastScenario.Reminder)
            .AddToastInput(new ToastSelectionBox("snoozeTime")
            {
                DefaultSelectionBoxItemId = "1",
                Items =
                {
                    new ToastSelectionBoxItem("1", "1分後"),
                    new ToastSelectionBoxItem("5", "5分後"),
                    new ToastSelectionBoxItem("10", "10分後"),
                    new ToastSelectionBoxItem("15", "15分後"),
                }
            })
            .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
            .Show();
        }
    }
    public static bool ToastStateChange()
    {
        if (ToastTimer is null) return false;
        Action action = ToastTimer.Enabled
            ? new Action(ToastTimer.Stop)
            : new Action(async () =>
            {
                ToastTimer.Start();
                await SetToastAsync();
            });
        action();
        return ToastTimer.Enabled;
    }
}
