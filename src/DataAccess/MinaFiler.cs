using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataAccess
{
    /// <summary>
    /// Hanterar alla filoperationer inklusive JSON serialisering/deserialisering och textfilläsning
    /// </summary>
    public static class MinaFiler
    {
        /// <summary>
        /// Konfigurerar JSON serialiseringsinställningar för korrekt hantering av arv och namngivning
        /// </summary>
        private static JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings
            {
                // Bevara typinformation för arv under serialisering
                TypeNameHandling = TypeNameHandling.Auto,
                // Gör JSON-utdata formaterad och läsbar
                Formatting = Formatting.Indented,
                // Använd camelCase för egenskapsnamn i JSON
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        /// <summary>
        /// Läser och deserialiserar en JSON-fil till angiven typ
        /// </summary>
        public static T ReadJson<T>(string filePath)
        {
            try
            {
                // Kontrollera om filen finns innan läsning
                if (!File.Exists(filePath))
                    return default(T);

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json, GetJsonSettings());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Serialiserar ett objekt till JSON och sparar till fil
        /// </summary>
        public static void SaveJson<T>(string filePath, T data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, GetJsonSettings());
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Läser hela innehållet i en textfil
        /// </summary>
        public static string ReadTextFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return string.Empty;

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading text file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Laddar om en textfil, användbart när prislistan uppdateras
        /// </summary>
        public static string ReloadTextFile(string filePath)
        {
            return ReadTextFile(filePath);
        }
    }
}
