using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class Recipes
    {
        [TestMethod]
        public void Execute()
        {
            // act
            var response = new RecipesRequest().Execute();

            // assert
            Assert.IsTrue(response.Recipes.Count > 1);
            Assert.IsTrue(response.Recipes.Contains(7309)); // Bolt of Damask
        }
    }
}
