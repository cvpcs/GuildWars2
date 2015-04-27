using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class MapNames
    {
        private const int CursedShoreMapId = 62;

        [TestMethod]
        public void Execute()
        {
            // act
            var response = new MapNamesRequest().Execute();

            // assert
            var map = response.Where(m => m.Id == CursedShoreMapId).First();
            Assert.AreEqual("Cursed Shore", map.Name);
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Fluchküste" },
                { LanguageCode.EN, "Cursed Shore" },
                { LanguageCode.ES, "Ribera Maldita" },
                { LanguageCode.FR, "Rivage maudit" },
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new MapNamesRequest(pair.Key).Execute();

                // assert
                var map = response.Where(m => m.Id == CursedShoreMapId).First();
                Assert.AreEqual(pair.Value, map.Name);
            }
        }
    }
}
