using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class WeaponsJsonParser
{
    public static WeaponsDataset ParseJson(TextAsset jsonFile)
    {
        if (jsonFile != null)
        {
            string json = jsonFile.text;

            // Register the custom converter
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ProjectileModifiersConverter() }
            };

            WeaponsDataset weaponDataset = JsonConvert.DeserializeObject<WeaponsDataset>(json);

            // Access the parsed data
            List<WeaponData> commonWeapons = weaponDataset.Common;
            List<WeaponData> uncommonWeapons = weaponDataset.Uncommon;
            List<WeaponData> rareWeapons = weaponDataset.Rare;
            List<WeaponData> legendaryWeapons = weaponDataset.Legendary;

            return weaponDataset;
        }
        else
        {
            Debug.LogError("No JSON file assigned!");
            return null;
        }
    }
}

public class ProjectileModifiersConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(List<string>);
    }

    public override object ReadJson(
        JsonReader reader,
        System.Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            List<string> modifiers = token.ToObject<List<string>>();
            return modifiers.FindAll(modifier => modifier != "None");
        }
        return new List<string>();
    }

    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }
}
