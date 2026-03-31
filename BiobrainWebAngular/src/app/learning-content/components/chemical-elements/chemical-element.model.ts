export class ChemicalElementModel {
    constructor(
        public name: string,
        public x: number,
        public y: number,
        public atomicNumber: number,
        public shortName: string,
        public massNumber: number,
        public backgroundColor: string,
        public fontColor: string,
        public isGroupFirst: boolean = false,
        public isPeriodFirsr: boolean = false,
        public blockName: string = "",
        public borderColor: string = ""
    ) { }
}