using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class EventDetails
    {
        [TestMethod]
        public void Execute()
        {
            //act
            var response = new EventDetailsRequest().Execute();

            // assert
            Assert.IsTrue(response.Events.Count > 1);

            var ev = response.Events.First();

            Assert.IsNotNull(ev.Key);
            Assert.IsNotNull(ev.Value.Name);
            Assert.IsNotNull(ev.Value.Location);
            Assert.IsTrue(ev.Value.Level > 0);
            Assert.IsTrue(ev.Value.MapId > 0);
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
                var response = new EventDetailsRequest(eventId, pair.Key).Execute();

                // assert
                Assert.AreEqual(1, response.Events.Count);
                Assert.IsTrue(response.Events.ContainsKey(eventId));

                var ev = response.Events[eventId];

                Assert.AreEqual(pair.Value, ev.Name);
            }
        }

        [TestMethod]
        public void ExecuteFlags()
        {
            // arrange
            var eventIdMap = new Dictionary<EventFlagType, Guid>()
            {
                { EventFlagType.GroupEvent, new Guid("3A2B85C5-DE73-4402-BD84-8F53AA394A52") }, // Bonus Event: Cull the Flame Legion
                { EventFlagType.MapWide, new Guid("F479B4CF-2E11-457A-B279-90822511B53B") } // Defeat the Karka Queen threatening the settlements.
            };

            foreach (var pair in eventIdMap)
            {
                // act
                var response = new EventDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(1, response.Events.Count);
                Assert.IsTrue(response.Events.ContainsKey(pair.Value));

                var ev = response.Events[pair.Value];

                Assert.AreEqual(pair.Key, ev.FlagsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteSphere()
        {
            // arrange
            var sphereEvent = new Guid("EED8A79F-B374-4AE6-BA6F-B7B98D9D7142"); // Defeat the renegade charr

            // act
            var response = new EventDetailsRequest(sphereEvent).Execute();

            // assert
            Assert.AreEqual(1, response.Events.Count);
            Assert.IsTrue(response.Events.ContainsKey(sphereEvent));

            var ev = response.Events[sphereEvent];

            Assert.AreEqual(LocationType.Sphere, ev.Location.TypeEnum);
            Assert.IsTrue(ev.Location.Radius > 0);
            Assert.IsTrue(ev.Location.Center.Count > 0);
        }

        [TestMethod]
        public void ExecuteCylinder()
        {
            // arrange
            var cylinderEvent = new Guid("3A2B85C5-DE73-4402-BD84-8F53AA394A52"); // Bonus Event: Cull the Flame Legion

            // act
            var response = new EventDetailsRequest(cylinderEvent).Execute();

            // assert
            Assert.AreEqual(1, response.Events.Count);
            Assert.IsTrue(response.Events.ContainsKey(cylinderEvent));

            var ev = response.Events[cylinderEvent];

            Assert.AreEqual(LocationType.Cylinder, ev.Location.TypeEnum);
            Assert.IsTrue(ev.Location.Radius > 0);
            Assert.IsTrue(ev.Location.Center.Count > 0);
            Assert.IsTrue(ev.Location.Height > 0);
        }
        
        [TestMethod]
        public void ExecutePoly()
        {
            // arrange
            var polyEvent = new Guid("CEA84FBF-2368-467C-92EA-7FA60D527C7B"); // Find a way to open the door and escape the armory

            // act
            var response = new EventDetailsRequest(polyEvent).Execute();

            // assert
            Assert.AreEqual(1, response.Events.Count);
            Assert.IsTrue(response.Events.ContainsKey(polyEvent));

            var ev = response.Events[polyEvent];

            Assert.AreEqual(LocationType.Poly, ev.Location.TypeEnum);
            Assert.IsTrue(ev.Location.Center.Count > 0);
            Assert.IsTrue(ev.Location.ZRange.Count > 0);
            Assert.IsTrue(ev.Location.Points.Count > 0);
        }
    }
}
