using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterComCore.Entities
{
    /// <summary>
    /// Представляє договір, укладений користувачем за певним шаблоном.
    /// Зберігає посилання на Template, ідентифікатор користувача,
    /// час створення та значення плейсхолдерів у JSON-форматі.
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Унікальний ідентифікатор договору.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// FK на шаблон договору.
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// Навігаційна властивість до шаблону.
        /// </summary>
        public Template Template { get; set; } = null!;

        /// <summary>
        /// FK на користувача (IdentityUser.Id).
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Дата та час створення договору.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// JSON-рядок, який містить серіалізований словник 
        /// { "PlaceholderKey" : "Value", ... }.
        /// </summary>
        public string PlaceholderValuesJson { get; set; } = "{}";

        /// <summary>
        /// Десеріалізує PlaceholderValuesJson у словник.
        /// </summary>
        /// <returns>Словник {ключ: значення} плейсхолдерів.</returns>
        public Dictionary<string, string> GetValues()
        {
            if (string.IsNullOrWhiteSpace(PlaceholderValuesJson))
                return new Dictionary<string, string>();

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(PlaceholderValuesJson)
                       ?? new Dictionary<string, string>();
            }
            catch (JsonException)
            {
                // Якщо JSON некоректний, повертаємо порожній словник
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Серіалізує словник у JSON та зберігає у PlaceholderValuesJson.
        /// </summary>
        /// <param name="values">Словник {ключ: значення}.</param>
        public void SetValues(Dictionary<string, string> values)
        {
            PlaceholderValuesJson = JsonSerializer.Serialize(values);
        }
    }
}
