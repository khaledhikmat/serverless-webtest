#r "Newtonsoft.Json"

using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task Run(TimerInfo cacheTimer, TraceWriter log)
{
    var location = GetEnvironmentVariable("location");
    log.Info($"Web Test trigger executed at {DateTime.Now} from {location}");    

    try 
    {
        var testUrl = GetEnvironmentVariable("testUrl");
        if (!string.IsNullOrEmpty(testUrl))
        {
            string [] results = await TestUrl(testUrl, log);                
            if (results != null && results.Length == 2)
            {
                // Condition the event to meet the Real-Time PowerBI expectation
                var realTimeEvent = new {
                    time = DateTime.Now,
                    source = GetEnvironmentVariable("location"),
                    url  = testUrl,
                    duration  = Double.Parse(results[1]),
                    result = results[0]
                };

                var events = new List<dynamic>();
                events.Add(realTimeEvent);
                await PostToPowerBI(events, log);
            }
            else
            {
                log.Info($"Bad results from testing url!");
            }
        }
        else
            log.Info($"No Test URL!");
    }
    catch (Exception e)
    {
        log.Info($"Encountered a failure: {e.Message}");
    }
}

private async static Task<string []> TestUrl(string url, TraceWriter log)
{
    var results = new string[2];
    var statusCode = "";
    HttpClient client = null;
    DateTime startTime = DateTime.Now;
    DateTime endTime = DateTime.Now;

    try
    {
        client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync(url);
        statusCode = response.StatusCode.ToString();    
    }
    catch (Exception ex)
    {
        log.Info($"TestUrl failed: {ex.Message}");
        statusCode = "500";
    }
    finally
    {
        if (client != null)
            client.Dispose();
    }

    endTime = DateTime.Now;
    results[0] = statusCode;
    results[1] = (endTime - startTime).TotalSeconds + "";
    return results;
}

private async static Task PostToPowerBI(object realTimeEvents, TraceWriter log)
{
    HttpClient client = null;
    // The URL for PowerBI Real Time Dataset
    var url = "https://api.powerbi.com/beta/your-own";

    try
    {
        client = new HttpClient();

        var postData = Newtonsoft.Json.JsonConvert.SerializeObject(realTimeEvents);
        HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(url , httpContent);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Bad return code: " + response.StatusCode);
        }
    }
    catch (Exception ex)
    {
        log.Info($"PostToPowerBI failed: {ex.Message}");
    }
    finally
    {
        if (client != null)
            client.Dispose();
    }
}

public static string GetEnvironmentVariable(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
