import { Colors } from "src/app/share/values/colors";

export class LevelTypeModel {
    public isRed: boolean = false;
    public isYellow: boolean = false;
    public isGreen: boolean = false;
    constructor(
        public levelTypeId: string,
        public levelName: string,
        public levelShortName: string,
        public isAvailable: boolean,
     ) { }
}