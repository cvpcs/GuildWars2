using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class ItemDetails
    {
        [TestMethod]
        public void Execute()
        {
            // arrange
            var itemId = 30698; // The Bifrost

            // act
            var response = new ItemDetailsRequest(itemId).Execute();
            
            // assert
            Assert.AreEqual(itemId, response.ItemId);
            Assert.AreEqual("The Bifrost", response.Name);
            Assert.AreNotEqual(ItemType.Invalid, response.TypeEnum);
            
            Assert.IsTrue(response.Level > 0);
            Assert.AreNotEqual(RarityType.Invalid, response.RarityEnum);
            Assert.IsTrue(response.VendorValue > 0);
            Assert.IsTrue(response.IconFileId > 0);
            
            Assert.IsNotNull(response.IconFileSignature);
            Assert.IsNotNull(response.IconFile);
            Assert.AreEqual(response.IconFileId, response.IconFile.FileId);
            Assert.AreEqual(response.IconFileSignature, response.IconFile.Signature);

            Assert.IsTrue(response.DefaultSkinId > 0);
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var itemId = 30698; // The Bifrost
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Der Bifröst" },
                { LanguageCode.EN, "The Bifrost" },
                { LanguageCode.ES, "El Bifrost" },
                { LanguageCode.FR, "Bifrost" },
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new ItemDetailsRequest(itemId, pair.Key).Execute();

                // assert
                Assert.AreEqual(pair.Value, response.Name);
            }
        }

        [TestMethod]
        public void ExecuteWithDescription()
        {
            // arrange
            var itemWithDescription = 66168; // Light of Dwayna (infused)

            // act
            var response = new ItemDetailsRequest(itemWithDescription).Execute();
            
            // assert
            Assert.IsNotNull(response.Description);
            Assert.AreNotEqual(string.Empty, response.Description);
        }

        [TestMethod]
        public void ExecuteWithGameTypes()
        {
            // arrange
            var itemIdMap = new Dictionary<GameType, int>()
            {
                { GameType.Activity, 26920 }, // Honed Soft Wood Warhorn
                { GameType.Dungeon, 49464 }, // Limited-Use Bronze Dolyak
                { GameType.Pve, 49464 }, // Limited-Use Bronze Dolyak
                { GameType.Pvp, 49464 }, // Limited-Use Bronze Dolyak
                { GameType.PvpLobby, 49464 }, // Limited-Use Bronze Dolyak
                { GameType.Wvw, 49464 } // Limited-Use Bronze Dolyak
            };

            foreach (var pair in itemIdMap)
            {
                // act
                var response = new ItemDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.GameTypesEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithFlags()
        {
            // arrange
            var itemIdMap = new Dictionary<ItemFlagType, int>()
            {
                { ItemFlagType.AccountBindOnUse, 44709 }, // Sovereign Beacon Skin
                { ItemFlagType.AccountBound, 44709 }, // Sovereign Beacon Skin
                { ItemFlagType.HideSuffix, 44709 }, // Sovereign Beacon Skin
                { ItemFlagType.MonsterOnly, 8354 }, // Pile of Wood
                { ItemFlagType.NoMysticForge, 44709 }, // Sovereign Beacon Skin
                { ItemFlagType.NoSalvage, 44709 }, // Sovereign Beacon Skin
                { ItemFlagType.NoSell, 8354 }, // Pile of Wood
                { ItemFlagType.NotUpgradeable, 39546 }, // Preserved Red Iris Flower
                { ItemFlagType.NoUnderwater, 20328 }, // Owl Tonic
                { ItemFlagType.SoulbindOnAcquire, 8354 }, // Pile of Wood
                { ItemFlagType.SoulBindOnUse, 8354 }, // Pile of Wood
                { ItemFlagType.Unique, 39546 } // Preserved Red Iris Flower
            };

            foreach (var pair in itemIdMap)
            {
                // act
                var response = new ItemDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.FlagsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithRarities()
        {
            // arrange
            var itemIdMap = new Dictionary<RarityType, int>()
            {
                { RarityType.Junk, 19531 }, // Porous Bone
                { RarityType.Basic, 23046 }, // PvP Salvage Kit
                { RarityType.Fine, 42599 }, // Mini Southsun Kasmeer
                { RarityType.Masterwork, 38013 }, // 20 Slot Fractal Uncommon Equipment Box
                { RarityType.Rare, 44883 }, // Musical Harp
                { RarityType.Exotic, 23095 }, // Triforge pendant
                { RarityType.Ascended, 66168 }, // Light of Dwayna (Infused)
                { RarityType.Legendary, 30698 } // The Bifrost
            };

            foreach (var pair in itemIdMap)
            {
                // act
                var response = new ItemDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.RarityEnum);
            }
        }

        [TestMethod]
        public void ExecuteWithRestrictions()
        {
            // arrange
            var itemIdMap = new Dictionary<RestrictionType, int>()
            {
                { RestrictionType.Asura, 3681 }, // Carrion Savant Epaulets
                { RestrictionType.Charr, 6196 }, // Trapper Shoulders
                { RestrictionType.Human, 6198 }, // Scout's Coat
                { RestrictionType.Norn, 3788 }, // Soldier's Lupine Circlet
                { RestrictionType.Sylvari, 6155 } // Dryad Boots
            };

            foreach (var pair in itemIdMap)
            {
                // act
                var response = new ItemDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.RestrictionsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithTypes()
        {
            var itemIdMap = new Dictionary<ItemType, int>()
            {
                { ItemType.Armor, 3710 }, // Rampager's Sorcerer's Coat
                { ItemType.Back, 66168 }, // Light of Dwayna (Infused)
                { ItemType.Bag, 38013 }, // 20 Slot Fractal Uncommon Equipment Box
                { ItemType.Consumable, 12452 }, // Omnomberry Bar
                { ItemType.Container, 9373 }, // Heavy Moldy Bag
                { ItemType.CraftingMaterial, 24305 }, // Charged Lodestone
                { ItemType.Gathering, 48933 }, // Molten Alliance Mining Pick
                { ItemType.Gizmo, 44883 }, // Musical Harp
                { ItemType.MiniPet, 42599 }, // Mini Southsun Kasmeer
                { ItemType.Tool, 23046 }, // PvP Salvage Kit
                { ItemType.Trinket, 23095 }, // Triforge pendant
                { ItemType.Trophy, 19531 }, // Porous Bone
                { ItemType.UpgradeComponent, 24642 }, // Superior Sigil of Undead Slaying
                { ItemType.Weapon, 30698 } // The Bifrost
            };

            foreach (var pair in itemIdMap)
            {
                // act
                var response = new ItemDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.TypeEnum);
            }
        }
    }
}
