var GuildWars2;
(function (GuildWars2) {
    (function (ArenaNet) {
        (function (Mapper) {
            var ArenaNetMap = (function () {
                function ArenaNetMap() {
                    $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (data) {
                        var floor = data;

                        var c = 0;
                        for (var i in floor.regions) {
                            for (var j in floor.regions[i].maps) {
                            }
                        }
                    });
                }
                return ArenaNetMap;
            })();
            Mapper.ArenaNetMap = ArenaNetMap;
        })(ArenaNet.Mapper || (ArenaNet.Mapper = {}));
        var Mapper = ArenaNet.Mapper;
    })(GuildWars2.ArenaNet || (GuildWars2.ArenaNet = {}));
    var ArenaNet = GuildWars2.ArenaNet;
})(GuildWars2 || (GuildWars2 = {}));
