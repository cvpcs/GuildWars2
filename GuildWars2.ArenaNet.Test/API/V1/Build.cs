using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Build
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new BuildRequest().Execute();

            // assert
            Assert.IsTrue(response.BuildId > 0);
        }
    }
}
