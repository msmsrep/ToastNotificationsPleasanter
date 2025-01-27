using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Configuration;

namespace ToastNotificationsPleasanter;

public class Schedule
{
    public int IssueId { get; set; }
    public string? Title { get; set; }
    public string? StartTime { get; set; }
}

public class ScheduleAPI
{
    public static List<Schedule>? ScheduleList { get; set; }
    public ScheduleAPI()
    {
        ScheduleList = new();
    }
    public static StringContent CreateContent(int targetStatus, int afterMinute)
    {
        var requestBody = new
        {
            ApiVersion = 1.1,
            ApiKey = ConfigurationManager.AppSettings["ApiKey"],
            View = new
            {
                ColumnFilterHash = new
                {
                    Status = targetStatus,
                    StartTime = $"[\"{DateTime.Now},{DateTime.Now.AddMinutes(afterMinute)}\"]"
                },
                ColumnSorterHash = new { StartTime = "asc" },
            }
        };
        return new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8, @"application/json"); ;
    }
    public static async Task<List<Schedule>?> GetScheduleAsync(StringContent content)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(
                        ConfigurationManager.AppSettings["ApiUrl"], content);
        if (!response.IsSuccessStatusCode) return null;
        var responseBody = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        JsonElement root = doc.RootElement;
        var elements = root.GetProperty("Response").GetProperty("Data");
        var NewScheduleList = new List<Schedule>();
        for (int i = 0; i < elements.GetArrayLength(); i++)
        {
            if (ScheduleList is null) continue;
            if (ScheduleList.Any(x => x.IssueId ==
                elements[i].GetProperty("IssueId").GetInt32())) continue;
            ScheduleList.Add(new Schedule()
            {
                IssueId = elements[i].GetProperty("IssueId").GetInt32(),
            });
            NewScheduleList.Add(new Schedule()
            {
                IssueId = elements[i].GetProperty("IssueId").GetInt32(),
                Title = elements[i].GetProperty("Title").GetString(),
                StartTime = elements[i].GetProperty("StartTime").GetDateTime().ToString()
            });
        }
        return NewScheduleList;
    }
}
