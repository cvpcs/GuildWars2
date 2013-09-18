declare module GuildWars2.SyntaxError.Model {

    export interface ChampionEventsResponse extends Array<string> { }

    export interface GuildBounty {
        name: string;
        map_id: number;
        spawns?: number[][];
        paths?: GuildBountyPath[];
    }

    export interface GuildBountyPath {
        direction: PathDirectionType;
        points: number[][];
    }

    export enum PathDirectionType {
        invalid,
        clockwise,
        counterclockwise
    }

}