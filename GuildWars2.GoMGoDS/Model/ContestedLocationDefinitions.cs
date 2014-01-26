using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public static class ContestedLocationDefinitions
    {
        public static IList<ContestedLocation> ContestedLocations;

        public static HashSet<Guid> EventList;
        public static IDictionary<ContestedLocation, List<Guid>> EventDictionary;

        static ContestedLocationDefinitions()
        {
            ContestedLocations = new List<ContestedLocation>();

            ContestedLocations.Add(new ContestedLocation("Citadel of Flame", "CoF")
                    .AddCaptureEvent(new Guid("6A8374CF-9999-43E9-B1C7-BAB1541F2426")) // escort razen
                    .AddDefendEvent(new Guid("A1182080-2599-4ACC-918E-A3275610602B")) // hold gates
                );

            ContestedLocations.Add(new ContestedLocation("Crucible of Eternity", "CoE")
                    .AddCaptureEvent(new Guid("C4860CEF-F801-4F6E-BAD5-183A9ECC16BE")) // slay trolls
                    .AddCaptureEvent(new Guid("08051AA6-F3E7-4B2A-91C2-4AC07AB8F1FF")) // break into IC
                    .AddCaptureEvent(new Guid("7B4EF74F-87CC-4978-9E05-82822F03BD33")) // destroyer lab
                    .AddCaptureEvent(new Guid("63E32CC1-F83B-4AA6-8EE6-D429F2673548")) // zhaitan lab
                    .AddCaptureEvent(new Guid("1F207830-E56C-4639-B375-065828B74249")) // crystal lab
                    .AddCaptureEvent(new Guid("ED8ADFBF-6D6A-4B98-BD49-741D39A04871")) // capture IC
                    .AddDefendEvent(new Guid("9752677E-FAE7-4F56-A48A-275329095B8A")) // hold IC
                );

            ContestedLocations.Add(new ContestedLocation("Gates of Arah", "GoA")
                    .AddCaptureEvent(new Guid("D7246CA2-DD85-42B3-A8D3-D2A1FE464ECF")) // get to anchorage
                    .AddCaptureEvent(new Guid("5761F5A5-48D2-484B-BE21-22096E84E845")) // hold anchorage
                    .AddCaptureEvent(new Guid("9DA0E1E8-1A44-4A3C-9FCC-257350978CE9")) // collect scrap metal
                    .AddCaptureEvent(new Guid("6B5C8659-F3AF-4DFC-A6F5-CD6620E3BE11")) // signal reinforcements
                    .AddCaptureEvent(new Guid("7EA1BE90-C3CB-4598-A2DD-D56764785F7D")) // escort didi
                    .AddCaptureEvent(new Guid("E87A021D-4E7C-4A50-BEDB-6F5A54C90A9A")) // sieze PoG steps
                    .AddCaptureEvent(new Guid("B1B94EFD-4F67-4716-97C2-880CD16F1297")) // sieze PoG hall
                    .AddCaptureEvent(new Guid("02DECBE6-A0BA-47CC-9256-A6D59881D92A")) // defeat wizard
                    .AddDefendEvent(new Guid("DFBFF5FE-5AF0-4B65-9199-B7CACC945ABD")) // ensure GoA
                    .AddDefendEvent(new Guid("EB8A7456-9BE7-40C5-8F13-21E44D4760A0")) // ensure GoA
                );

            ContestedLocations.Add(new ContestedLocation("Temple of Balthazar", "ToB")
                    .AddCaptureEvent(new Guid("D3FFC041-4124-4AA7-A74B-B9363ED1BCBD")) // escort northern
                    .AddCaptureEvent(new Guid("A8D1A2B7-1F1B-413D-8E64-06CA0D26712D")) // escort central
                    .AddCaptureEvent(new Guid("45B84A62-BE33-4371-B9FB-CC8490528276")) // escort southern
                    .AddCaptureEvent(new Guid("D0ECDACE-41F8-46BD-BB17-8762EF29868C")) // reach alter
                    .AddCaptureEvent(new Guid("7B7D6D27-67A0-44EF-85EA-7460FFA621A1")) // sieze alter
                    .AddCaptureEvent(new Guid("2555EFCB-2927-4589-AB61-1957D9CC70C8")) // defeat priest
                    .AddDefendEvent(new Guid("589B1C41-DD96-4AEE-8A3A-4CC607805B05")) // hold alter
                );

            ContestedLocations.Add(new ContestedLocation("Temple of Dwayna", "ToD")
                    .AddCaptureEvent(new Guid("F531683F-FC09-467F-9661-6741E8382E24")) // escort historian
                    .AddCaptureEvent(new Guid("7EF31D63-DB2A-4FEB-A6C6-478F382BFBCB")) // defeat priestess
                    .AddCaptureEvent(new Guid("526732A0-E7F2-4E7E-84C9-7CDED1962000")) // malchor
                    .AddCaptureEvent(new Guid("6A6FD312-E75C-4ABF-8EA1-7AE31E469ABA")) // defeat statue
                    .AddDefendEvent(new Guid("0723E056-E665-439F-99B7-20385442AD4E")) // protect cathedral
                    .AddDefendEvent(new Guid("B78631EA-1584-452A-859F-CE935321B52D")) // protect cathedral
                );

            ContestedLocations.Add(new ContestedLocation("Temple of Grenth", "ToG")
                    .AddCaptureEvent(new Guid("C2AB5C4C-5FAA-449B-985C-93F8E2D579C8")) // help pact reach torch
                    .AddCaptureEvent(new Guid("B41C90F8-AF33-400E-9AD3-3DB0AFCEDC6C")) // protect footi
                    .AddCaptureEvent(new Guid("4B612C93-3700-43B8-B3C1-CBC64FEC0566")) // see jonez to torch
                    .AddCaptureEvent(new Guid("1D1BE3D6-2F0D-4D1C-8233-812AAF261CFF")) // secure torch
                    .AddCaptureEvent(new Guid("27E2F73C-E26B-4046-AC06-72C442D9B2B7")) // defend lightning cannon
                    .AddCaptureEvent(new Guid("C8139970-BE46-419B-B026-485A14002D44")) // ensure jonez gets to cathedral
                    .AddCaptureEvent(new Guid("E16113B1-CE68-45BB-9C24-91523A663BCB")) // fight shades, slay priest, protect jonez
                    .AddCaptureEvent(new Guid("99254BA6-F5AE-4B07-91F1-61A9E7C51A51")) // cover jonez
                    .AddDefendEvent(new Guid("57A8E394-092D-4877-90A5-C238E882C320")) // stop priest
                );

            ContestedLocations.Add(new ContestedLocation("Temple of Lyssa", "ToL")
                    .AddCaptureEvent(new Guid("A3BEF1D9-10B0-44C7-8B4B-600BEC0F0316")) // stop inquest
                    .AddCaptureEvent(new Guid("20422E4E-B7C8-46BB-82CD-C0C320E3BD7E")) // defend energy containment device
                    .AddCaptureEvent(new Guid("F66922B5-B4BD-461F-8EC5-03327BD2B558")) // protect golems
                    .AddCaptureEvent(new Guid("F5436671-8934-4BD4-AEF7-4F3741A9CDA4")) // defeat risen
                    .AddCaptureEvent(new Guid("35997B10-179B-4E39-AD7F-54E131ECDD57")) // capture seal of union
                    .AddCaptureEvent(new Guid("ADC3AA4C-0212-4AE6-98FA-4F59F3C9BCFA")) // defend seal of union
                    .AddCaptureEvent(new Guid("590364E0-0053-4933-945E-21D396B10B20")) // defend seal of lyss
                    .AddCaptureEvent(new Guid("2F3955DB-5CAD-480E-AACB-4A9D318AA9A8")) // drive off minions
                    .AddCaptureEvent(new Guid("0372874E-59B7-4A8F-B535-2CF57B8E67E4")) // kill priestess
                    .AddDefendEvent(new Guid("BFA71CD0-ED1E-4EE3-B8A9-B23B98C3B786")) // kill risen
                );

            ContestedLocations.Add(new ContestedLocation("Temple of Melandru", "ToM")
                    .AddCaptureEvent(new Guid("C15950B3-7EA6-4976-9DD3-97C88354EE0C")) // reclaim landing
                    .AddCaptureEvent(new Guid("C39CA0D3-E00D-498F-9F9A-CCFB715896F4")) // defend landing
                    .AddCaptureEvent(new Guid("3D333172-24CE-47BA-8F1A-1AD47E7B69E4")) // first beacon
                    .AddCaptureEvent(new Guid("E7563D8D-838D-4AF4-80CD-1D3A25B6F6AB")) // second beacon
                    .AddCaptureEvent(new Guid("F0CE1E71-4B96-48C6-809D-E1941AF40B1D")) // defend beacon
                    .AddCaptureEvent(new Guid("351F7480-2B1C-4846-B03B-ED1B8556F3D7")) // escort pact
                    .AddCaptureEvent(new Guid("7E24F244-52AF-49D8-A1D7-8A1EE18265E0")) // destroy priest
                    .AddCaptureEvent(new Guid("A5B5C2AF-22B1-4619-884D-F231A0EE0877")) // defend interrupter
                    .AddDefendEvent(new Guid("989A298C-B06B-4E9B-A871-1506A6EE3FEC")) // defend interruptor
                    .AddDefendEvent(new Guid("04902E61-A102-4D32-860D-C14B150BD4F5")) // defend interruptor
                );

            // this is here for performance
            EventList = new HashSet<Guid>(ContestedLocations.SelectMany(l => l.Events).Distinct().ToList());
        }
    }
}