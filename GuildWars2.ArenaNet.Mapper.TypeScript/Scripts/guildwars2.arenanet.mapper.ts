/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/leaflet/leaflet.d.ts" />
/// <reference path="typings/guildwars2.arenanet/guildwars2.arenanet.d.ts" />
/// <reference path="typings/guildwars2.syntaxerror/guildwars2.syntaxerror.d.ts" />

module GuildWars2.ArenaNet.Mapper {
    export class ArenaNetMap {
        constructor() {
            $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (data: any) {
                var floor = <GuildWars2.ArenaNet.Model.MapFloorResponse>data;

                var c = 0;
                for (var i in floor.regions) {
                    for (var j in floor.regions[i].maps) {
                    }
                }
            });
        }
    }
}
