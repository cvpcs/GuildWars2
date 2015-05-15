using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Colors
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new ColorsRequest().Execute();

            // assert
            Assert.IsTrue(response.Colors.Count > 0);

            foreach (var dye in response.Colors.Values)
            {
                Assert.IsNotNull(dye.Name);

                AssertRgbArray(dye.BaseRgb);

                AssertMaterialColor(dye.Cloth);
                AssertMaterialColor(dye.Leather);
                AssertMaterialColor(dye.Metal);
            }
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var shadowBlueId = 1357;
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
                Assert.IsTrue(response.Colors.ContainsKey(shadowBlueId));

                var dye = response.Colors[shadowBlueId];

                Assert.AreEqual(pair.Value, dye.Name);
            }
        }

        private void AssertMaterialColor(MaterialColor m)
        {
            Assert.IsNotNull(m);

            Assert.IsTrue(m.Brightness >= -128 && m.Brightness <= 128); // [-128, 128]
            Assert.IsTrue(m.Contrast >= 0 && m.Contrast <= 2);          // [0, 2]
            Assert.IsTrue(m.Hue >= 0 && m.Hue < 360);                   // [0, 360)
            Assert.IsTrue(m.Saturation >= 0 && m.Saturation <= 2);      // [0, 2]
            Assert.IsTrue(m.Lightness >= 0 && m.Lightness <= 2);        // [0, 2]

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
