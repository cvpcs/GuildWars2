using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class EventNames
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new EventNamesRequest().Execute();

            // assert
            Assert.IsTrue(response.Count > 0);

            foreach (var ev in response)
            {
                Assert.IsNotNull(ev.Id);
                Assert.IsNotNull(ev.Name);
            }
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var eventId = new Guid("F479B4CF-2E11-457A-B279-90822511B53B"); // Defeat the Karka Queen threatening the settlements.
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Besiegt die Karka-Königin, die die Siedlungen bedroht." },
                { LanguageCode.EN, "Defeat the Karka Queen threatening the settlements." },
                { LanguageCode.ES, "Derrota a la reina karka que amenaza a los asentamientos." },
                { LanguageCode.FR, "Vaincre la reine karka qui menace les colonies." },
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new EventNamesRequest(pair.Key).Execute();

                // assert
                var ev = response.Where(e => e.Id == eventId).First();
                Assert.AreEqual(pair.Value, ev.Name);
            }
        }
    }
}
