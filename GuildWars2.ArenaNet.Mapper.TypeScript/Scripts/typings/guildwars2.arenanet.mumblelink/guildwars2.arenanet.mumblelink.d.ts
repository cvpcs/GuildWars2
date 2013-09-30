declare module GuildWars2.ArenaNet.MumbleLink {
    export interface MumbleData {
        data_available: boolean;

        game_name?: string;

        player_name?: string;
        player_is_commander?: boolean;
        player_profession?: number;
        player_team_color_id?: number;

        server?: number;
        map?: number;

        pos_x?: number;
        pos_y?: number;
        pos_z?: number;

        rot_player?: number;
        rot_camera?: number;
    }
}