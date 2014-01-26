using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public static class MetaEventDefinitions
    {
        public static IList<MetaEvent> MetaEvents;

        public static HashSet<Guid> EventList;
        public static IDictionary<MetaEvent, List<Guid>> EventDictionary;

        static MetaEventDefinitions()
        {
            MetaEvents = new List<MetaEvent>();

            /* GOOD */
            MetaEvents.Add(new MetaEvent("claw", "The Claw of Jormag", 10800, 16200)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Crystals scatter the landscape")
                            .AddEvent(new Guid("0CA3A7E3-5F66-4651-B0CB-C45D3F0CAD95")) // Destroy the dragon crystal on the road to Slough of Despond.
                            .AddEvent(new Guid("96D736C4-D2C6-4392-982F-AC6B8EF3B1C8")) // Destroy the dragon crystal at Elder's Vale.
                            .AddEvent(new Guid("C957AD99-25E1-4DB0-9938-F54D9F23587B")) // Destroy the dragon crystal near the Pact siege wall.
                            .AddEvent(new Guid("429D6F3E-079C-4DE0-8F9D-8F75A222DB36")) // Destroy the dragon crystal at the Pact flak cannons.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "One more crystal...")
                            .AddEvent(new Guid("BFD87D5B-6419-4637-AFC5-35357932AD2C")) // Lure out the Claws of Jormag by destroying the final dragon crystal.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the Claw of Jormag", 1800)
                            .AddEvent(new Guid("0464CB9E-1848-4AAA-BA31-4779A959DD71")) // Defeat the Claw of Jormag.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("commissar", "Dredge Commissar")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the dredge commissar")
                            .AddEvent(new Guid("95CA969B-0CC6-4604-B166-DBCCE125864F")) // Defeat the dredge commissar.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("eye", "Eye of Zhaitan")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Destroy the risen AA gun")
                            .AddEvent(new Guid("A0E5E563-2701-4D4E-8163-A89FEA02EC38")) // Destroy the Risen anti-aircraft gun so Nallandra can signal her team to land.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort Warmaster Leddron")
                            .AddEvent(new Guid("6FA8BE3F-9F6C-4790-BFBC-380B26FDB06C")) // Escort Warmaster Leddron and his team to Zho'qafa Catacombs.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Search Zho'qafa Catacombs")
                            .AddEvent(new Guid("42884028-C274-4DFA-A493-E750B8E1B353")) // Defend the Pact team as they search Zho'qafa Catacombs for artifacts.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Destroy the Eye of Zhaitan")
                            .AddEvent(new Guid("A0796EC5-191D-4389-9C09-E48829D1FDB2")) // Destroy the Eye of Zhaitan
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("elemental", "Fire Elemental", 1500, 3300)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Destroy the chaotic materials")
                            .AddEvent(new Guid("6B897FF9-4BA8-4EBD-9CEC-7DCFDA5361D8")) // Destroy the chaotic materials created by the reactor meltdown.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Rooba is waiting outside")
                            .AddEvent(new Guid("5E4E9CD9-DD7C-49DB-8392-C99E1EF4E7DF"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort the C.L.E.A.N. golem")
                            .AddEvent(new Guid("5E4E9CD9-DD7C-49DB-8392-C99E1EF4E7DF"), EventStateType.Active) // Escort the C.L.E.A.N. 5000 golem while it absorbs clouds of chaos magic.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Defend the C.L.E.A.N. golem", 300)
                            .AddEvent(new Guid("2C833C11-5CD5-4D96-A4CE-A74C04C9A278")) // Defend the C.L.E.A.N. 5000 golem.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Destroy the fire elemental", 900)
                            .AddEvent(new Guid("33F76E9E-0BB6-46D0-A3A9-BE4CDFC4A3A4")) // Destroy the fire elemental created from chaotic energy fusing with the C.L.E.A.N. 5000's energy core.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("shaman", "Fire Shaman", 1500, 3300)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Kill the Fire Shaman")
                            .AddEvent(new Guid("295E8D3B-8823-4960-A627-23E07575ED96"), EventStateType.Active) // Defeat the fire shaman and his minions.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("foulbear", "Foulbear Chieftain")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Assault Foulbear Kraal", 780)
                            .AddEvent(new Guid("D9F1CF48-B1CB-49F5-BFAF-4CEC5E68C9CF"), EventStateType.Active) // Assault Foulbear Kraal by killing its leaders before the ogres can rally.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Destroy Foulbear Kraal", 720)
                            .AddEvent(new Guid("4B478454-8CD2-4B44-808C-A35918FA86AA")) // Destroy Foulbear Kraal before the ogres can rally.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Kill the Foulbear Chieftain", 600)
                            .AddEvent(new Guid("B4E6588F-232C-4F68-9D58-8803D67E564D")) // Kill the Foulbear Chieftain and her elite guards before the ogres can rally.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("maw", "The Frozen Maw", 3600, 7200)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "The grawl are attacking")
                            .AddEvent(new Guid("6F516B2C-BD87-41A9-9197-A209538BB9DF")) // Protect Tor the Tall's supplies from the grawl.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort Scholar Brogun")
                            .AddEvent(new Guid("D5F31E0B-E0E3-42E3-87EC-337B3037F437")) // Protect Scholar Brogun as he investigates the grawl tribe.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Destroy the totem")
                            .AddEvent(new Guid("6565EFD4-6E37-4C26-A3EA-F47B368C866D")) // Destroy the dragon totem.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Stop the shaman from summoning help", 900)
                            .AddEvent(new Guid("90B241F5-9E59-46E8-B608-2507F8810E00")) // Defeat the shaman's elite guard.
                            .AddEvent(new Guid("DB83ABB7-E5FE-4ACB-8916-9876B87D300D")) // Defeat the Svanir shamans spreading the dragon's corruption.
                            .AddEvent(new Guid("374FC8CB-7AB7-4381-AC71-14BFB30D3019")) // Destroy the corrupted portals summoning creatures from the mists.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Kill the Svanir shaman", 900)
                            .AddEvent(new Guid("F7D9D427-5E54-4F12-977A-9809B23FBA99")) // Kill the Svanir shaman chief to break his control over the ice elemental.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("golem", "Golem Mark II", 2400, 3600)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Recovery, "Harvest kelp")
                            .AddEvent(new Guid("A7E0F553-C4E1-452F-B39F-7BDBEC8B0BB1")) // Harvest kelp from the reef lurker fields so the Lonatl chief can heal his tribe.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Recovery, "Healing the tribe", 420)
                            .AddEvent(new Guid("3ED4FEB4-A976-4597-94E8-8BFD9053522F"), EventStateType.Warmup)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Waste evacuation soon", 180)
                            .AddEvent(new Guid("3ED4FEB4-A976-4597-94E8-8BFD9053522F"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Disable containers", 900)
                            .AddEvent(new Guid("3ED4FEB4-A976-4597-94E8-8BFD9053522F"), EventStateType.Active) // Disable the containers before they release their toxins.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Calculating trajectory...", 120)
                            .AddEvent(new Guid("9AA133DC-F630-4A0E-BB5D-EE34A2B306C2"), EventStateType.Warmup)
                            .AddEvent(new Guid("9AA133DC-F630-4A0E-BB5D-EE34A2B306C2"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "The Golem Mark II has landed", 900)
                            .AddEvent(new Guid("9AA133DC-F630-4A0E-BB5D-EE34A2B306C2"), EventStateType.Active) // Defeat the Inquest's golem Mark II.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("wurm", "Great Jungle Wurm", 7200, 9000)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Gamarien is jumping with excitement")
                            .AddEvent(new Guid("613A7660-8F3A-4897-8FAC-8747C12E42F8"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Take a stroll with Gamarien")
                            .AddEvent(new Guid("613A7660-8F3A-4897-8FAC-8747C12E42F8"), EventStateType.Active) // Protect Gamarien as he scouts Wychmire Swamp.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Blighted things surround the swamp")
                            .AddEvent(new Guid("CF6F0BB2-BD6C-4210-9216-F0A9810AA2BD")) // Destroy the blighted growth.
                            .AddEvent(new Guid("456DD563-9FDA-4411-B8C7-4525F0AC4A6F")) // Destroy the blighted growth.
                            .AddEvent(new Guid("1DCFE4AA-A2BD-44AC-8655-BBD508C505D1")) // Kill the giant blighted grub.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Even more blighted things")
                            .AddEvent(new Guid("61BA7299-6213-4569-948B-864100F35E16")) // Destroy the avatars of blight.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "The wurm has surfaced", 900)
                            .AddEvent(new Guid("C5972F64-B894-45B4-BC31-2DEEA6B7C033")) // Defeat the great jungle wurm.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("karka", "Karka Queen")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the Karka Queen", 600)
                            .AddEvent(new Guid("E1CC6E63-EFFE-4986-A321-95C89EA58C07")) // Defeat the Karka Queen threatening the settlements.
                            .AddEvent(new Guid("5282B66A-126F-4DA4-8E9D-0D9802227B6D")) // Defeat the Karka Queen threatening the settlements.
                            .AddEvent(new Guid("F479B4CF-2E11-457A-B279-90822511B53B")) // Defeat the Karka Queen threatening the settlements.
                            .AddEvent(new Guid("4CF7AA6E-4D84-48A6-A3D1-A91B94CCAD56")) // Defeat the Karka Queen threatening the settlements.
                        )
                );
            
            /* GOOD */
            MetaEvents.Add(new MetaEvent("megadestroyer", "Megadestroyer")
                    .AddStage(new MetaEventMultiLineStage(MetaEventStage.StageType.PreEvent, "Assist the sylvari camp")
                            .AddEvent(new Guid("294E08F6-CA36-42B3-8D06-B321BA06EECA"), "Stop the destroyer-essence collectors") // Stop the Inquest's destroyer-essence collectors by destroying their power cores.
                            .AddEvent(new Guid("61D4579A-C53F-4C26-A31B-92FABE3DA566"), EventStateType.Active, "Defend the Advanced Arcanomics lab") // Defend the Advanced Arcanomics lab.
                            .AddEvent(new Guid("3BA29A69-A30B-405D-96AC-CBA5D511C163"), "Stop the rampaging golem") // Stop the rampaging Inquest golem.
                        )
                    .AddStage(new MetaEventMultiLineStage(MetaEventStage.StageType.PreEvent, "Clear the volcano of destroyers.")
                            .AddEvent(new Guid("9E5D9F1A-FE14-49C6-917F-43AAE227165C"), "Slay destroyers to the north") // Slay destroyers, and close their fissures on the volcano's north side.
                            .AddEvent(new Guid("584A4D22-33DC-4D77-A5D9-2FA7379401ED"), "Slay destroyers to the southeast") // Eliminate destroyer forces in the volcano's southeast cavern.
                            .AddEvent(new Guid("4210CE81-BDB7-448E-BE33-46E18A5A3477"), "Slay destroyers to the south") // Eliminate destroyer forces on the volcano's south side.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Protect the asura")
                            .AddEvent(new Guid("36E81760-7D92-458E-AA22-7CDE94112B8F")) // Protect the asura and their technology while they quell the unstable volcano.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Kill the Megadestroyer", 900)
                            .AddEvent(new Guid("C876757A-EF3E-4FBE-A484-07FF790D9B05")) // Kill the megadestroyer before it blows everyone up.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("ulgoth", "Modniir Ulgoth")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Assault Kingsgate", 1200)
                            .AddEvent(new Guid("DDC0A526-A239-4791-8984-E7396525B648")) // Assault Kingsgate and drive the centaurs back before they can rally their forces.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Kill the war council", 1200)
                            .AddEvent(new Guid("A3101CDC-A4A0-4726-85C0-147EF8463A50")) // Kill the centaur war council before reinforcements arrive.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Dig in at Kingsgate")
                            .AddEvent(new Guid("DA465AE1-4D89-4972-AD66-A9BE3C5A1823")) // Keep the Modniir invaders from retaking Kingsgate.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat Ulgoth and his minions", 1200)
                            .AddEvent(new Guid("E6872A86-E434-4FC1-B803-89921FF0F6D6")) // Defeat Ulgoth the Modniir and his minions.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("behemoth", "Shadow Behemoth", 7200, 9000)
                    .AddStage(new MetaEventBlockedStage("Assistance needed in the swamp")
                            .AddBlockedEvent(new Guid("31CEBA08-E44D-472F-81B0-7143D73797F5"), EventStateType.Preparation) // Defeat the shadow behemoth.
                            .AddEvent(new Guid("1E644459-9EC4-4C63-9975-E37CF5E7C108"), "The monestary requires assistance") // Drive back the centaurs before they steal the monastery's ale.
                            .AddEvent(new Guid("5AB5E4A7-202C-476F-8B0C-5F2CDC5DB3A0"), "The monestary requires assistance") // Return stolen kegs of ale to Abbot Mathias.
                            .AddEvent(new Guid("9062EBB9-EAD2-43E4-A820-DC6BD28A3040"), EventStateType.Warmup, "The monestary requires assistance") // Protect the brew shipment.
                            .AddEvent(new Guid("9062EBB9-EAD2-43E4-A820-DC6BD28A3040"), EventStateType.Preparation, "The monestary requires assistance") // Protect the brew shipment.
                            .AddEvent(new Guid("04084490-0117-4D56-8D67-C4FFFE933C0C"), "Defeat the champion oakheart") // Defeat the champion rotting ancient oakheart.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Portals are outside the swamp")
                            .AddEvent(new Guid("AFCF031A-F71D-4CEA-85E1-957179414B25")) // Drive back Underworld creatures by destroying portals in Taminn Foothills.
                            .AddEvent(new Guid("CFBC4A8C-2917-478A-9063-1A8B43CC8C38")) // Drive back Underworld creatures by destroying portals in the Heartwoods.
                            .AddEvent(new Guid("E539A5E3-A33B-4D5F-AEED-197D2716F79B")) // Drive back Underworld creatures by destroying portals in the monastery.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Portals are inside the swamp")
                            .AddEvent(new Guid("36330140-7A61-4708-99EB-010B10420E39")) // Drive back Underworld creatures by destroying portals in the swamp.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "The beast has awakened", 900)
                            // this goes into prep stages before the pres start even if there's a block. maybe display that?
                            .AddEvent(new Guid("31CEBA08-E44D-472F-81B0-7143D73797F5"), EventStateType.Active) // Defeat the shadow behemoth.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("shatterer", "The Shatterer", 10800, 11100)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Preparations have begun")
                            .AddEvent(new Guid("8E064416-64B5-4749-B9E2-31971AB41783")) // Escort the Sentinel squad to the Vigil camp in Lowland Burns.
                            .AddEvent(new Guid("580A44EE-BAED-429A-B8BE-907A18E36189")) // Collect siege weapon pieces for Crusader Blackhorn.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Preparations are complete")
                            .AddEvent(new Guid("03BF176A-D59F-49CA-A311-39FC6F533F2F"), EventStateType.Warmup)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Ominous winds are blowing")
                            .AddEvent(new Guid("03BF176A-D59F-49CA-A311-39FC6F533F2F"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Slay the Shatterer", 900)
                            .AddEvent(new Guid("03BF176A-D59F-49CA-A311-39FC6F533F2F"), EventStateType.Active) // Slay the Shatterer
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("taidha", "Taidha Covington")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Destroy the canons")
                            .AddEvent(new Guid("B6B7EE2A-AD6E-451B-9FE5-D5B0AD125BB2")) // Eliminate the cannons at the northern defensive tower.
                            .AddEvent(new Guid("189E7ABE-1413-4F47-858E-4612D40BF711")) // Capture Taidha Covington's southern defensive tower.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Assault the gate")
                            .AddEvent(new Guid("0E0801AF-28CF-4FF7-8064-BB2F4A816D23")) // Defend the galleon and help it destroy Taidha's gate.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Kill Taidha Covington", 600)
                            .AddEvent(new Guid("242BD241-E360-48F1-A8D9-57180E146789")) // Kill Admiral Taidha Covington.
                        )
                );

            /* READY FOR TEST */
            MetaEvents.Add(new MetaEvent("balthazar", "Temple of Balthazar")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort the Pact")
                            .AddEvent(new Guid("D0ECDACE-41F8-46BD-BB17-8762EF29868C")) // Help the Pact reach the Altar of Betrayal before their morale is depleted.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Seize the alter")
                            .AddEvent(new Guid("7B7D6D27-67A0-44EF-85EA-7460FFA621A1")) // Seize the Altar of Betrayal before Pact morale can be broken.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the Risen Priest", 600)
                            .AddEvent(new Guid("2555EFCB-2927-4589-AB61-1957D9CC70C8")) // Defeat the Risen Priest of Balthazar before it can summon a horde of Risen.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("dwayna", "Temple of Dwayna")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort Historian Vermoth")
                            .AddEvent(new Guid("F531683F-FC09-467F-9661-6741E8382E24")) // Escort Historian Vermoth to the Altar of Tempests.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Defeat the Risen Priestess")
                            .AddEvent(new Guid("7EF31D63-DB2A-4FEB-A6C6-478F382BFBCB")) // Defeat the Risen Priestess of Dwayna.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Beat up Malchor")
                            .AddEvent(new Guid("526732A0-E7F2-4E7E-84C9-7CDED1962000")) // Drive Malchor to the Altar of Tempests.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the possessed statue")
                            .AddEvent(new Guid("6A6FD312-E75C-4ABF-8EA1-7AE31E469ABA")) // Defeat the possessed statue of Dwayna.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("grenth", "Temple of Grenth")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Assault the temple")
                            .AddEvent(new Guid("C8139970-BE46-419B-B026-485A14002D44")) // Ensure that Keeper Jonez Deadrun reaches the Cathedral of Silence.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Slay the Risen Priest")
                            .AddEvent(new Guid("E16113B1-CE68-45BB-9C24-91523A663BCB")) // Use portals to fight shades, slay the Champion Risen Priest of Grenth, and protect Keeper Jonez Deadrun.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Cover Keeper Jonez", 360)
                            .AddEvent(new Guid("99254BA6-F5AE-4B07-91F1-61A9E7C51A51")) // Cover Keeper Jonez Deadrun as he performs the cleansing ritual.
                        )
                );

            /* READY FOR TEST */
            MetaEvents.Add(new MetaEvent("lyssa", "Temple of Lyssa")
                    /* TODO: The end event is always active, need to find a way to better handle this */
                    .AddStage(new MetaEventMultiLineStage(MetaEventStage.StageType.PreEvent, "Capture the seals")
                            .AddEvent(new Guid("B6D6D060-4974-4385-AB08-F641B6F32823"), EventStateType.Active, "Defend the Song of Lyss") // Defend the seal until the Pact cannon is back online.
                            .AddEvent(new Guid("2F3955DB-5CAD-480E-AACB-4A9D318AA9A8"), EventStateType.Active, "Defend the Song of Lyss") // Drive off the dragon minions.
                            .AddEvent(new Guid("590364E0-0053-4933-945E-21D396B10B20"), EventStateType.Active, "Capture the Song of Lyss") // Defend the Seal of Lyss until the Pact cannon is online.
                            .AddEvent(new Guid("20422E4E-B7C8-46BB-82CD-C0C320E3BD7E"), EventStateType.Active, "Defend the Spark of Ilya") // Defend the energy containment device.
                            .AddEvent(new Guid("A3BEF1D9-10B0-44C7-8B4B-600BEC0F0316"), EventStateType.Active, "Defend the Spark of Ilya") // Stop the Inquest from draining the energy containment device.
                            .AddEvent(new Guid("F66922B5-B4BD-461F-8EC5-03327BD2B558"), EventStateType.Active, "Capture the Spark of Ilya") // Protect the Pact golems until they charge the neutralizer device.
                            .AddEvent(new Guid("F5436671-8934-4BD4-AEF7-4F3741A9CDA4"), EventStateType.Active, "Defend the Seal of Union") // Defeat the Risen forces before they construct fortifications around the Seal of Union.
                            .AddEvent(new Guid("ADC3AA4C-0212-4AE6-98FA-4F59F3C9BCFA"), EventStateType.Active, "Defend the Seal of Union") // Defend the Seal of Union from corruption.
                            .AddEvent(new Guid("35997B10-179B-4E39-AD7F-54E131ECDD57"), EventStateType.Active, "Capture the Seal of Union") // Destroy the Risen fortifications to capture the Seal of Union.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the High Priestess")
                            .AddEvent(new Guid("0372874E-59B7-4A8F-B535-2CF57B8E67E4")) // Kill the Corrupted High Priestess
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("melandru", "Temple of Melandru")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Place the first beacon")
                            .AddEvent(new Guid("3D333172-24CE-47BA-8F1A-1AD47E7B69E4"), EventStateType.Active) // Escort Magister Izzmek to the site of the first signal beacon.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Place the second beacon")
                            .AddEvent(new Guid("E7563D8D-838D-4AF4-80CD-1D3A25B6F6AB")) // Escort Magister Izzmek to the site of the second signal beacon.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Defend the second beacon", 300)
                            .AddEvent(new Guid("F0CE1E71-4B96-48C6-809D-E1941AF40B1D")) // Defend the beacon until Pact reinforcements can arrive.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Escort the Pact")
                            .AddEvent(new Guid("351F7480-2B1C-4846-B03B-ED1B8556F3D7")) // Escort the Pact forces to the Temple of Melandru.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Destroy the Risen Priest")
                            .AddEvent(new Guid("7E24F244-52AF-49D8-A1D7-8A1EE18265E0")) // Destroy the Risen Priest of Melandru.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend the interrupter", 360)
                            .AddEvent(new Guid("A5B5C2AF-22B1-4619-884D-F231A0EE0877")) // Defend the Pact interrupter device while it charges to cleanse the temple.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEventIntervaled("tequatl", "Tequatl the Sunless", 7200, 300, 3600)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Somthing's in the water")
                            .AddEvent(new Guid("568A30CF-8512-462F-9D67-647D69BEFAED"), EventStateType.Preparation)
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat Tequatl the Sunless")
                            .AddEvent(new Guid("568A30CF-8512-462F-9D67-647D69BEFAED"), EventStateType.Active) // Defeat Tequatl the Sunless.
                        )
                );

            /* GOOD */
            MetaEvents.Add(new MetaEvent("scarlet", "Scarlet Briar")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Lornar's Pass", 2700, true)
                            .AddEvent(new Guid("6DEB01AE-675E-4FF9-9789-53CB73FC621E"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Bloodtide Coast", 2700, true)
                            .AddEvent(new Guid("11442531-6B20-411F-B0A6-D2A2C31DD668"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Dredgehaunt Cliffs", 2700, true)
                            .AddEvent(new Guid("526EFDC9-3F3C-492E-911E-14AFE9EAE70D"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Gendarran Fields", 2700, true)
                            .AddEvent(new Guid("90FEBBE9-0066-42CF-9C48-703C920AFB9D"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Timberline Falls", 2700, true)
                            .AddEvent(new Guid("6195E248-1DD4-452B-A7DD-3472162E0683"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Frostgorge Sound", 2700, true)
                            .AddEvent(new Guid("46BBCFDD-1285-4246-A9FA-620773C7D4C6"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Fields of Ruin", 2700, true)
                            .AddEvent(new Guid("C7CC535C-81A1-4E84-993B-6384C911399A"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Harathi Hinterlands", 2700, true)
                            .AddEvent(new Guid("9795C994-4C12-4E1F-82A6-D541F76D9D37"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Fireheart Rise", 2700, true)
                            .AddEvent(new Guid("5FE50E83-758B-4573-A424-A1661FBC970A"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Mount Maelstrom", 2700, true)
                            .AddEvent(new Guid("FE5F6233-DAD6-4C63-921D-F132DFCF3397"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Iron Marches", 2700, true)
                            .AddEvent(new Guid("D1B8B6D2-5E61-44DE-92C6-D49A9BBBB6E2"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Blazeridge Steppes", 2700, true)
                            .AddEvent(new Guid("CF657BC1-D5CE-41D3-A630-A3E509451B7A"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defend Sparkfly Fen", 2700, true)
                            .AddEvent(new Guid("92979945-63A4-42D7-8AE5-1EFADC9E636F"), EventStateType.Active) // Scarlet's Minions Invade!
                        )
                );

            /* READY FOR TEST */
            MetaEvents.Add(new MetaEvent("arah", "Gates of Arah")
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Sieze the promenade steps")
                            .AddEvent(new Guid("E87A021D-4E7C-4A50-BEDB-6F5A54C90A9A")) // Help the Pact seize the steps of the Promenade of the Gods.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Capture the promenade hall")
                            .AddEvent(new Guid("B1B94EFD-4F67-4716-97C2-880CD16F1297")) // Help the Pact capture the hall of the Promenade of the Gods.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the High Wizard")
                            .AddEvent(new Guid("02DECBE6-A0BA-47CC-9256-A6D59881D92A")) // Defeat the Risen High Wizard and secure the Promenade of the Gods.
                        )
                );

            /* READY FOR TEST */
            MetaEvents.Add(new MetaEventIntervaled("marionette", "Twisted Marionette", 7200, 300, 3600)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "The floating machine is churning", 300)
                            .AddEvent(new Guid("861DCD3D-4398-4863-89DE-4F8F4B7DDEDF")) // Prepare for Scarlet's Twisted Marionette
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "The marionette descends")
                            .AddEvent(new Guid("AD581729-46BB-4C23-B6CA-F080F62C6174"), EventStateType.Warmup) // Twisted Marionette Weapon Test
                            .AddEvent(new Guid("AD581729-46BB-4C23-B6CA-F080F62C6174"), EventStateType.Preparation) // Twisted Marionette Weapon Test
                        )
                    .AddStage(new MetaEventMultiLineStage(MetaEventStage.StageType.Boss, "Destroy the regulators", 150, true)
                            .AddEvent(new Guid("8A3BA5D5-2CC1-46CD-9A0A-5DB2B4897439"), "Sever the first chain") // Destroy the power regulators to discharge energy back at the marionette.
                            .AddEvent(new Guid("3E607C66-3CCD-4165-B94F-1C49A09E2F28"), "Sever the second chain") // Destroy the power regulators to discharge energy back at the marionette.
                            .AddEvent(new Guid("79D03B7A-2250-4B20-80DC-225100D96B5D"), "Sever the third chain") // Destroy the power regulators to discharge energy back at the marionette.
                            .AddEvent(new Guid("E5957F3F-3D72-484A-9897-AD481A352B55"), "Sever the fourth chain") // Destroy the power regulators to discharge energy back at the marionette.
                            .AddEvent(new Guid("C0F3FC76-8B2F-478D-8D1A-5E86580231AB"), "Sever the final chain") // Destroy the power regulators to discharge energy back at the marionette.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Hold the lines")
                            .AddEvent(new Guid("AFF75C18-EEF7-4330-B988-BE6DC336B8CB")) // Hold the line!
                            .AddEvent(new Guid("2A9C3838-4460-4CEB-ABFB-FC2AADA7499E")) // Hold the line!
                            .AddEvent(new Guid("5A660E79-A906-44A4-ADE9-6245B74101FE")) // Hold the line!
                            .AddEvent(new Guid("EF083B14-324A-4D10-8918-464800E46BCD")) // Hold the line!
                            .AddEvent(new Guid("D8E6FBE2-83BA-491D-BF32-4B1A315EEC99")) // Hold the line!
                        )
                );

            /* READY FOR TEST */
            MetaEvents.Add(new MetaEventIntervaled("triwurm", "Triple-headed Wurm", 7200, 300, 3600)
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.PreEvent, "Investigations will begin shortly", 360) // (5 min wait + 1 min dialog)
                            .AddEvent(new Guid("B081F000-5928-4B69-8980-20AD93827B6C"), EventStateType.Preparation) // Investigate the rumors of wurm activity in Whisperwill Bogs.
                            .AddEvent(new Guid("743B0A35-118E-43D7-ACCE-10FDF00139BD"), EventStateType.Preparation) // Investigate the rumors of wurm activity in Challdar Gorges.
                            .AddEvent(new Guid("96E9213C-54FD-4D66-B546-EF02FACEACEB"), EventStateType.Preparation) // Investigate the rumors of wurm activity on Jelako Beach.
                        )
                    .AddStage(new MetaEventMultiLineStage(MetaEventStage.StageType.PreEvent, "Investigate the rumors")
                            .AddEvent(new Guid("B081F000-5928-4B69-8980-20AD93827B6C"), "Investigate Whisperwill Bogs") // Investigate the rumors of wurm activity in Whisperwill Bogs.
                            .AddEvent(new Guid("743B0A35-118E-43D7-ACCE-10FDF00139BD"), "Investigate Challdar Gorges") // Investigate the rumors of wurm activity in Challdar Gorges.
                            .AddEvent(new Guid("96E9213C-54FD-4D66-B546-EF02FACEACEB"), "Investigate Jelako Beach") // Investigate the rumors of wurm activity on Jelako Beach.
                        )
                    .AddStage(new MetaEventStage(MetaEventStage.StageType.Boss, "Defeat the heads of the wurm", 900)
                            .AddEvent(new Guid("5F91ED03-6A1B-4A54-9BB4-6A94AE97FC4F")) // Defeat the amber head of the great jungle wurm.
                            .AddEvent(new Guid("8A155E1C-BD89-4B1A-97E6-4673F4A285C8")) // Defeat the crimson head of the great jungle wurm.
                            .AddEvent(new Guid("630E3DF2-7A5F-40CA-AC8B-C0E55FF03047")) // Defeat the cobalt head of the great jungle wurm.
                        )
                );

            // this is here for performance
            EventList = new HashSet<Guid>(MetaEvents.SelectMany(me => me.Stages).SelectMany(s => s.EventStates).Select(s => s.Event).Distinct().ToList());
        }
    }
}
