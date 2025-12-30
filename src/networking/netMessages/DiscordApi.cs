using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostHuntingSchmutz
{

    public static class DiscordApi
    {
        private static readonly string GuildId = Environment.GetEnvironmentVariable("GUILD_ID") ?? "";
        private static readonly string BotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";

        private static readonly HttpClient Http = new HttpClient();

        public static List<Channel> Channels = new List<Channel>
    {
        new Channel("420874075351810060", "Text-Kanäle"),
        new Channel("420874075351810061", "geschriebenes"),
        new Channel("420874075351810062", "Sprach-Kanäle"),
        new Channel("420874075351810063", "Linke obere Schublade"),
        new Channel("881304679638781992", "Fridolins Sitzkissenstube"),
        new Channel("881304843065655316", "hoffentlich-nicht-geschriebenes"),
        new Channel("1316497895733596212", "Browser-VdW"),
        new Channel("1316498012352155908", "lobby"),
        new Channel("1316498053418582047", "1"),
        new Channel("1316498107797733376", "2"),
        new Channel("1316498119697104987", "3"),
        new Channel("1316498132749520937", "4")
    };

        public static List<Member> Members = new List<Member>();

        public static async Task MoveUserToChannel(string userId, string channelName)
        {
            Console.WriteLine("[MoveUserToChannel] Start");
            Console.WriteLine($"[MoveUserToChannel] userId = {userId}");
            Console.WriteLine($"[MoveUserToChannel] channelName = {channelName}");

            var channel = Channels.FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                Console.WriteLine($"[MoveUserToChannel] Channel '{channelName}' nicht gefunden.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine("[MoveUserToChannel] userId ist leer oder null");
                return;
            }

            Console.WriteLine($"[MoveUserToChannel] channel.Id = {channel.Id}");

            var url = $"https://discord.com/api/v10/guilds/{GuildId}/members/{userId}";
            Console.WriteLine($"[MoveUserToChannel] URL: {url}");

            var payloadObj = new
            {
                channel_id = channel.Id
            };

            var payload = JsonSerializer.Serialize(payloadObj);
            Console.WriteLine($"[MoveUserToChannel] Payload: {payload}");

            var req = new HttpRequestMessage(HttpMethod.Patch, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bot", BotToken);
            req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try
            {
                Console.WriteLine("[MoveUserToChannel] Sending request...");
                resp = await Http.SendAsync(req);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoveUserToChannel] Request Exception: {ex}");
                return;
            }

            Console.WriteLine($"[MoveUserToChannel] Response Status: {(int)resp.StatusCode} {resp.StatusCode}");

            string responseBody = "";
            try
            {
                responseBody = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"[MoveUserToChannel] Response Body: {responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoveUserToChannel] Error reading body: {ex}");
            }

            if (resp.IsSuccessStatusCode)
                Console.WriteLine($"[MoveUserToChannel] SUCCESS: User {userId} -> Channel {channel.Id}");
            else
                Console.WriteLine($"[MoveUserToChannel] FAILURE: Discord returned {resp.StatusCode}");

            Console.WriteLine("[MoveUserToChannel] Finished");
        }


        public static async Task<List<Member>> GetMembers()
        {
            Console.WriteLine("[GetMembers] Start");

            var url = $"https://discord.com/api/v10/guilds/{GuildId}/members?limit=100";
            Console.WriteLine($"[GetMembers] URL: {url}");
            Console.WriteLine($"[GetMembers] GuildId = {GuildId}");
            Console.WriteLine($"[GetMembers] BotToken (gekürzt) = {BotToken?.Substring(0, Math.Min(10, BotToken.Length))}...");

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bot", BotToken);

            HttpResponseMessage resp;
            try
            {
                Console.WriteLine("[GetMembers] Sending request...");
                resp = await Http.SendAsync(req);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetMembers] Request Exception: {ex}");
                return new List<Member>();
            }

            Console.WriteLine($"[GetMembers] Response Status: {(int)resp.StatusCode} {resp.StatusCode}");

            string json;
            try
            {
                json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetMembers] Raw JSON length: {json.Length}");
                // Optional: komplettes JSON sehen (falls nötig)
                // Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetMembers] Error reading response JSON: {ex}");
                return new List<Member>();
            }

            List<DiscordMember> raw;
            try
            {
                raw = JsonSerializer.Deserialize<List<DiscordMember>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? [];

                if (raw == null)
                {
                    Console.WriteLine("[GetMembers] Deserialization returned null");
                    return new List<Member>();
                }

                Console.WriteLine($"[GetMembers] Raw member count: {raw.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetMembers] JSON Deserialization Error: {ex}");
                return new List<Member>();
            }

            var mapped = raw
                .Where(m => m.User != null && m.User.Bot == false)
                .Select(m => new Member(
                    m.User.Id,
                    m.User.Username,
                    m.Nick,
                    m.User.GlobalName
                ))
                .ToList();

            Console.WriteLine($"[GetMembers] Non-bot members: {mapped.Count}");

            Members.Clear();
            Members.AddRange(mapped);

            Console.WriteLine("[GetMembers] Finished");
            return mapped;
        }
    }

    public record Channel(string Id, string Name);

    public record Member(string Id, string Username, string Nick, string GlobalName);

    public class DiscordMember
    {
        public DiscordUser User { get; set; }
        public string Nick { get; set; }
    }

    public class DiscordUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string GlobalName { get; set; }
        public bool Bot { get; set; }
    }

}