using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Items
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new ItemsRequest().Execute();

            // assert
            Assert.IsTrue(response.Items.Count > 1);
            Assert.IsTrue(response.Items.Contains(30698)); // The Bifrost
        }
    }
}
