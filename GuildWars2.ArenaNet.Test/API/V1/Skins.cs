using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Skins
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new SkinsRequest().Execute();

            // assert
            Assert.IsTrue(response.Skins.Count > 1);
            Assert.IsTrue(response.Skins.Contains(1350)); // Zodiac Light Vest
        }
    }
}
