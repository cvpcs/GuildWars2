using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class GuildDetails
    {
        private static readonly Guid SyntaxErrorId = new Guid("16A3E4CB-247F-447C-A885-B0B24DAB600D");
        private const string SyntaxErrorName = "Syntax Error";

        [TestMethod]
        public void ExecuteGuid()
        {
            // act
            var response = new GuildDetailsRequest(SyntaxErrorId).Execute();

            // assert
            AssertIsSyntaxError(response);
        }

        [TestMethod]
        public void ExecuteName()
        {
            // act
            var response = new GuildDetailsRequest(SyntaxErrorName).Execute();

            // assert
            AssertIsSyntaxError(response);
        }

        private void AssertIsSyntaxError(Guild guild)
        {
            Assert.AreEqual(SyntaxErrorId, guild.GuildId);
            Assert.AreEqual(SyntaxErrorName, guild.GuildName);
            Assert.AreEqual("SE", guild.Tag);
            Assert.IsTrue(guild.Emblem.BackgroundId > 0);
            Assert.IsTrue(guild.Emblem.ForegroundId > 0);
            Assert.IsTrue(guild.Emblem.BackgroundColorId > 0);
            Assert.IsTrue(guild.Emblem.ForegroundPrimaryColorId > 0);
            Assert.IsTrue(guild.Emblem.ForegroundSecondaryColorId > 0);
        }
    }
}