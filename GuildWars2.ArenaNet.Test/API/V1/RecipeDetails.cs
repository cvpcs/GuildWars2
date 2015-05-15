using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class RecipeDetails
    {
        [TestMethod]
        public void Execute()
        {
            // arrange
            var recipeId = 7309; // Bolt of Damask

            // act
            var response = new RecipeDetailsRequest(recipeId).Execute();
            
            // assert
            Assert.AreEqual(recipeId, response.RecipeId);
            Assert.AreNotEqual(ItemType.Invalid, response.TypeEnum);
            
            Assert.IsTrue(response.OutputItemId > 0);
            Assert.IsTrue(response.OutputItemCount > 0);

            Assert.IsTrue(response.MinRating >= 0);
            Assert.IsTrue(response.TimeToCraftMs > 0);
            Assert.IsTrue(response.VendorValue >= 0);

            Assert.AreNotEqual(DisciplineType.Invalid, response.DisciplinesEnum & DisciplineType.Invalid);
            Assert.AreNotEqual(RecipeFlagType.Invalid, response.FlagsEnum & RecipeFlagType.Invalid);

            Assert.IsTrue(response.Ingredients.Count > 0);
        }

        [TestMethod]
        public void ExecuteWithDisciplines()
        {
            // arrange
            var recipeIdMap = new Dictionary<DisciplineType, int>()
            {
                { DisciplineType.Armorsmith, 7309 }, // Bolt of Damask
                { DisciplineType.Artificer, 7313 }, // Deldrimor Steel Ingot
                { DisciplineType.Chef, 3155 }, // Omnomberry Bar
                { DisciplineType.Huntsman, 7313 }, // Deldrimor Steel Ingot
                { DisciplineType.Jeweler, 3725 }, // Gift of Music
                { DisciplineType.Leatherworker, 7309 }, // Bolt of Damask
                { DisciplineType.Tailor, 7309 }, // Bolt of Damask
                { DisciplineType.Weaponsmith, 7313 } // Deldrimor Steel Ingot
            };

            foreach (var pair in recipeIdMap)
            {
                // act
                var response = new RecipeDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.DisciplinesEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithFlags()
        {
            // arrange
            var recipeIdMap = new Dictionary<RecipeFlagType, int>()
            {
                { RecipeFlagType.AutoLearned, 21 }, // Orichalcum Ingot
                { RecipeFlagType.LearnedFromItem, 3725 } // Gift of Music
            };

            foreach (var pair in recipeIdMap)
            {
                // act
                var response = new RecipeDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.FlagsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithTypes()
        {
            var recipeIdMap = new Dictionary<RecipeType, int>()
            {
                { RecipeType.Amulet, 3744 }, // Topaz Silver Pendant
                { RecipeType.Axe, 5599 }, // Ravaging Steel Axe
                { RecipeType.Backpack, 8523 }, // Intricate Armorsmith's Backpack
                { RecipeType.Bag, 2840 }, // Ogre Bag
                { RecipeType.Boots, 2066 }, // Hearty Student Shoes
                { RecipeType.Bulk, 1888 }, // Satchel of Rejuvenating Noble Armor
                { RecipeType.Coat, 925 }, // Hearty Gladiator Chestplate
                { RecipeType.Component, 1152 }, // Coarse Glove Panel
                { RecipeType.Consumable, 7840 }, // Toxic Sharpening Stone
                { RecipeType.Dagger, 7762 }, // Chorben's Razor
                { RecipeType.Dessert, 3149 }, // Peach Pie
                { RecipeType.Dye, 3371 }, // Unidentified Orange Dye
                { RecipeType.Earring, 3661 }, // Coral Orichalcum Earring
                { RecipeType.Feast, 3173 }, // Tray of Strawberry Pies
                { RecipeType.Focus, 4047 }, // Assassin's Krait Star
                { RecipeType.Gloves, 8376 }, // Keeper's Warfist
                { RecipeType.Greatsword, 9149 }, // Handcrafted Woundfire
                { RecipeType.Hammer, 6421 }, // Assassin's Primordus Maul
                { RecipeType.Harpoon, 5710 }, // Hunter's Steel Spear
                { RecipeType.Helm, 8804 }, // Mask of 1,000 Faces
                { RecipeType.IngredientCooking, 2856 }, // Pasta Noodles
                { RecipeType.Inscription, 8370 }, // Keeper's Zealot Inscription
                { RecipeType.Insignia, 240 }, // Cleric's Intricate Linen Insignia
                { RecipeType.Leggings, 1179 }, // Hearty Outlaw Pants
                { RecipeType.LongBow, 5150 }, // Hearty Bandit Longbow
                { RecipeType.Mace, 7067 }, // Sentinel's Mithril Mace
                { RecipeType.Meal, 3093 }, // Bowl of Krytan Meatball Dinner
                { RecipeType.Pistol, 5163 }, // Vigorous Bandit Revolver
                { RecipeType.Potion, 4456 }, // Mystery Tonic
                { RecipeType.Refinement, 7307 }, // Elonian Leather Square
                { RecipeType.RefinementEctoplasm, 7320 }, // Glob of Elder Spirit Residue
                { RecipeType.RefinementObsidian, 7316 }, // Dragonite Ingot
                { RecipeType.Rifle, 7512 }, // Coalforge's Musket
                { RecipeType.Ring, 3793 }, // Topaz Gold Ring
                { RecipeType.Scepter, 8436 }, // Keeper's Wand
                { RecipeType.Seasoning, 2922 }, // Bottle of Sesame Ginger Sauce
                { RecipeType.Shield, 6212 }, // Hearty Dredge Barricade
                { RecipeType.ShortBow, 7472 }, // Dire Pearl Needler
                { RecipeType.Shoulders, 478 }, // Strong Splint Pauldrons
                { RecipeType.Snack, 2959 }, // Loaf of Zucchini Bread
                { RecipeType.Soup, 2991 }, // Bowl of Yam Soup
                { RecipeType.Speargun, 4713 }, // Honed Seasoned Wood Harpoon Gun
                { RecipeType.Staff, 9732 }, // Nomad's Pearl Quarterstaff
                { RecipeType.Sword, 5479 }, // Ravaging Iron Sword
                { RecipeType.Torch, 5183 }, // Strong Bandit Torch
                { RecipeType.Trident, 7426 }, // Soldier's Pearl Trident
                { RecipeType.UpgradeComponent, 486 }, // Minor Rune of the Brawler
                { RecipeType.Warhorn, 7533 } // Zojja's Herald
            };

            foreach (var pair in recipeIdMap)
            {
                // act
                var response = new RecipeDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.TypeEnum);
            }
        }
    }
}
