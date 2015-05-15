using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.Test.API.V1
{
    [TestClass]
    public class SkinDetails
    {
        [TestMethod]
        public void Execute()
        {
            // arrange
            var skinId = 1350; // Zodiac Light Vest

            // act
            var response = new SkinDetailsRequest(skinId).Execute();

            // assert
            Assert.AreEqual(skinId, response.SkinId);
            Assert.AreEqual("Zodiac Light Vest", response.Name);
            Assert.AreNotEqual(SkinType.Invalid, response.TypeEnum);

            Assert.IsTrue(response.IconFileId > 0);

            Assert.IsNotNull(response.IconFileSignature);
            Assert.IsNotNull(response.IconFile);
            Assert.AreEqual(response.IconFileId, response.IconFile.FileId);
            Assert.AreEqual(response.IconFileSignature, response.IconFile.Signature);

            Assert.AreNotEqual(SkinFlagType.Invalid, response.FlagsEnum & SkinFlagType.Invalid);
            Assert.AreNotEqual(RestrictionType.Invalid, response.RestrictionsEnum & RestrictionType.Invalid);
        }

        [TestMethod]
        public void ExecuteTranslated()
        {
            // arrange
            var skinId = 1350; // Zodiac Light Vest
            var translatedNames = new Dictionary<LanguageCode, string>()
            {
                { LanguageCode.DE, "Leichte Zodiak-Weste" },
                { LanguageCode.EN, "Zodiac Light Vest" },
                { LanguageCode.ES, "Chaleco del zodiaco ligero" },
                { LanguageCode.FR, "Gilet léger zodiacal" }
            };

            foreach (var pair in translatedNames)
            {
                // act
                var response = new SkinDetailsRequest(skinId, pair.Key).Execute();

                // assert
                Assert.AreEqual(pair.Value, response.Name);
            }
        }

        [TestMethod]
        public void ExecuteWithFlags()
        {
            // arrange
            var skinIdMap = new Dictionary<SkinFlagType, int>()
            {
                //{ SkinFlagType.HideIfUnlocked, -1 }, No skin exists at this time
                { SkinFlagType.NoCost, 760 }, // Radiant Greaves
                { SkinFlagType.ShowInWardrobe, 1350 } // Zodiac Light Vest
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.FlagsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithTypes()
        {
            var skinIdMap = new Dictionary<SkinType, int>()
            {
                { SkinType.Armor, 1350 }, // Zodiac Light Vest
                { SkinType.Back, 5856 }, // Lucky Ram Lantern
                { SkinType.Weapon, 5789 } // Caithe's Bloom Dagger
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.TypeEnum);
            }
        }

        [TestMethod]
        public void ExecuteWithRestrictions()
        {
            var skinIdMap = new Dictionary<RestrictionType, int>()
            {
                { RestrictionType.Asura, 297 }, // Prototype Helm
                { RestrictionType.Charr, 356 }, // Dreadnought Tassets
                { RestrictionType.Human, 417 }, // Assassin's Leggings
                { RestrictionType.Norn, 474 }, // Stag Mail
                { RestrictionType.Sylvari, 484 } // Snapdragon Leggings
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.AreEqual(pair.Key, response.RestrictionsEnum & pair.Key);
            }
        }

        [TestMethod]
        public void ExecuteWithArmorTypes()
        {
            var skinIdMap = new Dictionary<ArmorType, int>()
            {
                { ArmorType.Boots, 760 }, // Radiant Greaves
                { ArmorType.Coat, 5588 }, // Radiant Brigandine
                { ArmorType.Gloves, 766 }, // Radiant Vambraces
                { ArmorType.Helm, 765 }, // Radiant Warhelm
                { ArmorType.HelmAquatic, 1041 }, // Heavy Consortium Breathing Mask
                { ArmorType.Leggings, 5591 }, // Radiant Legguards
                { ArmorType.Shoulders, 763 } // Radiant Mantle
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.IsNotNull(response.Armor);
                Assert.AreEqual(pair.Key, response.Armor.TypeEnum);
            }
        }

        [TestMethod]
        public void ExecuteWithArmorWeightClasses()
        {
            var skinIdMap = new Dictionary<ArmorWeightClassType, int>()
            {
                { ArmorWeightClassType.Clothing, 5455 }, // Ancestral Outfit
                { ArmorWeightClassType.Heavy, 5592 }, // Radiant Cuirass
                { ArmorWeightClassType.Light, 5588 }, // Radiant Brigandine
                { ArmorWeightClassType.Medium, 760 } // Radiant Greaves
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.IsNotNull(response.Armor);
                Assert.AreEqual(pair.Key, response.Armor.WeightClassEnum);
            }
        }

        [TestMethod]
        public void ExecuteWithWeaponTypes()
        {
            var skinIdMap = new Dictionary<WeaponType, int>()
            {
                { WeaponType.Axe, 4325 }, // Adamant Guard Axe
                { WeaponType.Dagger, 5789 }, // Caithe's Bloom Dagger
                { WeaponType.Focus, 4309 }, // Warden Focus
                { WeaponType.Greatsword, 4316 }, // Wolfborn Greatsword
                { WeaponType.Hammer, 4340 }, // Peacemaker's Hammer
                { WeaponType.Harpoon, 4318 }, // Peacemaker's Spear
                { WeaponType.LongBow, 4332 }, // Peacemaker's Longbow
                { WeaponType.Mace, 4274 }, // Seraph Mace
                { WeaponType.Pistol, 4270 }, // Warden Pistol
                { WeaponType.Rifle, 4273 }, // Peacemaker's Rifle
                { WeaponType.Scepter, 4327 }, // Adamant Guard Scepter
                { WeaponType.Shield, 4328 }, // Seraph Shield
                { WeaponType.ShortBow, 4307 }, // Seraph Short Bow
                { WeaponType.Speargun, 4288 }, // Peacemaker's Speargun
                { WeaponType.Staff, 4347 }, // Adamant Guard Staff
                { WeaponType.Sword, 4351 }, // Adamant Guard Blade
                { WeaponType.Torch, 4287 }, // Peacemaker's Torch
                { WeaponType.Trident, 4263 }, // Peacemaker's Trident
                { WeaponType.Warhorn, 4295 } // Seraph Warhorn
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.IsNotNull(response.Weapon);
                Assert.AreEqual(pair.Key, response.Weapon.TypeEnum);
            }
        }

        [TestMethod]
        public void ExecuteWithWeaponDamageTypes()
        {
            var skinIdMap = new Dictionary<WeaponDamageType, int>()
            {
                { WeaponDamageType.Fire, 4682 }, // Incinerator
                { WeaponDamageType.Ice, 4674 }, // Frostfang
                { WeaponDamageType.Lightning, 4684 }, // Bolt
                { WeaponDamageType.Physical, 5789 } // Caithe's Bloom Dagger
            };

            foreach (var pair in skinIdMap)
            {
                // act
                var response = new SkinDetailsRequest(pair.Value).Execute();

                // assert
                Assert.IsNotNull(response.Weapon);
                Assert.AreEqual(pair.Key, response.Weapon.DamageTypeEnum);
            }
        }
    }
}
