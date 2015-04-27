using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Colors
    {
        private const int ShadowBlueId = 1357;

        [TestMethod]
        public void Execute()
        {
            // act
            var response = new ColorsRequest().Execute();

            // assert
            Assert.IsTrue(response.Colors.Count > 0);
            Assert.IsTrue(response.Colors.ContainsKey(ShadowBlueId));

            var dye = response.Colors[ShadowBlueId];

            Assert.AreEqual("Shadow Blue", dye.Name);

            AssertRgbArray(dye.BaseRgb);

            AssertMaterialColor(dye.Cloth);
            AssertMaterialColor(dye.Leather);
            AssertMaterialColor(dye.Metal);
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Schatten-Blau" },
                { LanguageCode.EN, "Shadow Blue" },
                { LanguageCode.ES, "Azul sombrío" },
                { LanguageCode.FR, "Bleu de l'ombre" },
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new ColorsRequest(pair.Key).Execute();

                // assert
                Assert.IsTrue(response.Colors.Count > 0);
                Assert.IsTrue(response.Colors.ContainsKey(ShadowBlueId));

                var dye = response.Colors[ShadowBlueId];

                Assert.AreEqual(pair.Value, dye.Name);
            }
        }

        private void AssertMaterialColor(MaterialColor m)
        {
            Assert.IsNotNull(m);

            Assert.IsTrue(m.Brightness >= -100 && m.Brightness <= 100); // [-100, 100]
            Assert.IsTrue(m.Contrast >= 0 && m.Contrast <= 1);          // [0, 1]
            Assert.IsTrue(m.Hue >= 0 && m.Hue < 360);                   // [0, 360)
            Assert.IsTrue(m.Saturation >= 0 && m.Saturation <= 1);      // [0, 1]
            Assert.IsTrue(m.Lightness >= 0 && m.Lightness <= 1);        // [0, 1]

            AssertRgbArray(m.Rgb);
        }

        private void AssertRgbArray(List<int> rgb)
        {
            Assert.AreEqual(3, rgb.Count);
            Assert.IsTrue(rgb[0] >= 0 && rgb[0] <= 255); // [0, 255]
            Assert.IsTrue(rgb[1] >= 0 && rgb[1] <= 255); // [0, 255]
            Assert.IsTrue(rgb[2] >= 0 && rgb[2] <= 255); // [0, 255]
        }
    }
}
