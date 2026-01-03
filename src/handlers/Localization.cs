using System.Text.Json;
using System.Text.Json.Serialization;

// Ist eine Schnittstelle zwischen Textdateien ( mit allen angezeigten Texten ) und dem Code.
// Code sollte niemals hard-gecodeten Text haben, sondern diese Klasse zum laden der Texte benutzen.
// Es können "Tokens" verwendet werden, um Text automatisch dynamisch anzupassen.
// Es können auch eigene Tokens verwendet werden durch /test123/, und später manuell ersetzt werden.
// Ebenso kann man zufällige Texte aus einer Auswahl laden

namespace fiveSeconds
{

    public class WeightedString
    {
        public string String { get; set; } = "";
        public int Weight { get; set; }
    }

    public class TranslationValue
    {
        public string? String { get; set; }
        public List<WeightedString>? Options { get; set; }

        public bool IsString => String != null;
        public bool IsList => Options != null;

        public string GetRandomString()
        {
            string raw = IsString ? String! : PickWeightedRandom(Options!);
            return ReplaceTokens(raw);
        }

        public string[] GetAllStrings()
        {
            if (Options == null) return [];
            string[] strings = [.. Options.Select((s) => ReplaceTokens(s.String))];
            return strings;
        }

        private static string PickWeightedRandom(List<WeightedString> options)
        {
            int totalWeight = options.Sum(o => o.Weight);
            int choice = Client.Game.Random.Next(totalWeight);
            // Console.WriteLine($"Total weight {totalWeight} {choice}");
            foreach (var o in options)
            {
                if (choice < o.Weight) return o.String;
                choice -= o.Weight;
            }
            return options.First().String;
        }

        public static string ReplaceTokens(string input)
        {
            return input;
        }
    }

    public class TranslationValueConverter : JsonConverter<TranslationValue>
    {
        public override TranslationValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // Einzelner String
                return new TranslationValue { String = reader.GetString() };
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = new List<WeightedString>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        // Reiner String → Gewicht 1
                        list.Add(new WeightedString
                        {
                            String = reader.GetString()!,
                            Weight = 1
                        });
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        // Objekt → normal deserialisieren
                        var obj = JsonSerializer.Deserialize<WeightedString>(ref reader, options);
                        list.Add(obj!);
                    }
                    else
                    {
                        throw new JsonException("Invalid array item type in translation value.");
                    }
                }

                return new TranslationValue { Options = list };
            }

            throw new JsonException("Invalid translation value type.");
        }

        public override void Write(Utf8JsonWriter writer, TranslationValue value, JsonSerializerOptions options)
        {
            if (value.IsString)
            {
                writer.WriteStringValue(value.String);
            }
            else if (value.IsList)
            {
                JsonSerializer.Serialize(writer, value.Options, options);
            }
        }
    }


    public static class Localization
    {
        private static Dictionary<string, TranslationValue> _translations;

        private static bool loaded = false;

        public static void LoadLanguage(string langCode)
        {
            string filePath = Path.Combine("lang", langCode + ".json");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Language file not found: {filePath}");

            string json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new TranslationValueConverter() }
            };

            _translations = JsonSerializer.Deserialize<Dictionary<string, TranslationValue>>(json, options)!;
        }

        // Übersetzung holen
        public static string Get(string key, params object[] args)
        {
            if (!loaded)
            {
                LoadLanguage("de");
                loaded = true;
            }

            if (_translations.TryGetValue(key, out var value))
                return value.GetRandomString();

            return key; // Fallback, falls Key fehlt
        }

        public static string[] GetAll(string key, params object[] args)
        {
            if (!loaded)
            {
                LoadLanguage("de");
                loaded = true;
            }

            if (_translations.TryGetValue(key, out var value))
                return value.GetAllStrings();

            return [key]; // Fallback, falls Key fehlt
        }

        public static string GetTileInfo(string key, params object[] args)
        {
            return Get($"tile.{key}.info", args);
        }

        public static string GetPropInfo(string key, params object[] args)
        {
            return Get($"prop.{key}.info", args);
        }

        public static string GetModeString(string key, params object[] args)
        {
            return Get($"mode.{key}");
        }

        public static string GetDialogue(string key, params object[] args)
        {
            return Get($"actor.{key}");
        }

        public static string GetEquipment(string equipment, string key)
        {
            return Get($"equipment.{equipment}.{key}");
        }

        public static string[] GetQuestionTriggerStrings(string type)
        {
            return GetAll($"question.{type}.triggerStrings");
        }

        public static string[] GetOrderTriggerStrings(string type)
        {
            return GetAll($"order.{type}.triggerStrings");
        }
    }
}