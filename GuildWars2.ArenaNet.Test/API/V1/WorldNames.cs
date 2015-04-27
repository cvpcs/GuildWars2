using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class WorldNames
    {
        private const int GateOfMadnessWorldId = 1007;

        [TestMethod]
        public void Execute()
        {
            // act
            var response = new WorldNamesRequest().Execute();

            // assert
            var world = response.Where(w => w.Id == GateOfMadnessWorldId).First();
            Assert.AreEqual("Gate of Madness", world.Name);
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Tor des Irrsinns" },
                { LanguageCode.EN, "Gate of Madness" },
                { LanguageCode.ES, "Puerta de la Locura" },
                { LanguageCode.FR, "Porte de la folie" },
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new WorldNamesRequest(pair.Key).Execute();

                // assert
                var world = response.Where(w => w.Id == GateOfMadnessWorldId).First();
                Assert.AreEqual(pair.Value, world.Name);
            }
        }
    }
}
