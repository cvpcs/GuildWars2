using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GuildWars2.PvPCasterToolbox.Controls;
using LibHotKeys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace GuildWars2.PvPCasterToolbox.Configuration
{
    // TODO: can this be done with IConfiguration or IOptions?
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppConfig : INotifyPropertyChanged
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new HotKeyConverter(), new StringEnumConverter() }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        #region Backing properties / defaults
        private bool useOverlayMode = true;
        private bool useLiveSetup = true;
        private Rectangle redSection = new Rectangle((int)(0.42708 * SystemParameters.PrimaryScreenWidth),
                                                     (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                                     (int)(0.04167 * SystemParameters.PrimaryScreenWidth),
                                                     (int)(0.03704 * SystemParameters.PrimaryScreenHeight));
        private Rectangle blueSection = new Rectangle((int)(0.53125 * SystemParameters.PrimaryScreenWidth),
                                                      (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                                      (int)(0.04167 * SystemParameters.PrimaryScreenWidth),
                                                      (int)(0.03704 * SystemParameters.PrimaryScreenHeight));
        private RectangleF redScoreBarPosition;
        private RectangleF blueScoreBarPosition;
        private ImageModulationParameters redScoreBarModulation;
        private ImageModulationParameters blueScoreBarModulation;
        private HotKey screenshotHotKey;
        #endregion

        #region Serialized properties
        [JsonProperty]
        public bool UseOverlayMode
        {
            get => this.useOverlayMode;
            set => this.SetValue(ref this.useOverlayMode, value);
        }

        [JsonProperty]
        public bool UseLiveSetup
        {
            get => this.useLiveSetup;
            set => this.SetValue(ref this.useLiveSetup, value);
        }

        [JsonProperty]
        public Rectangle RedSection
        {
            get => this.redSection;
            set => this.SetValue(ref this.redSection, value);
        }

        [JsonProperty]
        public Rectangle BlueSection
        {
            get => this.blueSection;
            set => this.SetValue(ref this.blueSection, value);
        }

        [JsonProperty]
        public RectangleF RedScoreBarPosition
        {
            get => this.redScoreBarPosition;
            set => this.SetValue(ref this.redScoreBarPosition, value);
        }

        [JsonProperty]
        public RectangleF BlueScoreBarPosition
        {
            get => this.blueScoreBarPosition;
            set => this.SetValue(ref this.blueScoreBarPosition, value);
        }

        [JsonProperty]
        public ImageModulationParameters RedScoreBarModulation
        {
            get => this.redScoreBarModulation;
            set => this.SetValue(ref this.redScoreBarModulation, value);
        }

        [JsonProperty]
        public ImageModulationParameters BlueScoreBarModulation
        {
            get => this.blueScoreBarModulation;
            set => this.SetValue(ref this.blueScoreBarModulation, value);
        }

        [JsonProperty]
        public HotKey ScreenshotHotKey
        {
            get => this.screenshotHotKey;
            set => this.SetValue(ref this.screenshotHotKey, value);
        }
        #endregion

        public void Load(string path)
        {
            JsonConvert.PopulateObject(File.ReadAllText(path), this, JsonSerializerSettings);

            // TODO: the populateobject section only performs sets on leaf members, so the base objects don't have their setters
            //       hit. as such we need to trigger the property change notifications on all of them manually on load. investigate
            //       a better way to handle this
            var jsonProperties = this.GetType()
                                     .GetProperties()
                                     .Where(propertyInfo => Attribute.IsDefined(propertyInfo, typeof(JsonPropertyAttribute)));
            foreach (var jsonProperty in jsonProperties)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(jsonProperty.Name));
            }
        }

        public void Save(string path)
            => File.WriteAllText(path, JsonConvert.SerializeObject(this, JsonSerializerSettings));

        private void SetValue<TValue>(ref TValue property, TValue value, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<TValue>.Default.Equals(property, value))
            {
                property = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class HotKeyConverter : JsonConverter<HotKey>
        {
            public override bool CanWrite => false;

            public override HotKey ReadJson(JsonReader reader, Type objectType, HotKey existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                return new HotKey(jo[nameof(HotKey.Key)].ToObject<Key>(),
                                  jo[nameof(HotKey.Modifiers)].ToObject<ModifierKeys>());
            }

            public override void WriteJson(JsonWriter writer, HotKey value, JsonSerializer serializer)
                => throw new NotImplementedException();
        }
    }
}
