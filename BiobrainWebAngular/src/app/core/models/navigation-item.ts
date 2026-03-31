export class NavigationItem {
    constructor(
        public readonly title: string,
        public readonly navigation: string,
        public readonly queryParams: any = {},
        public readonly icon: string = ''
    ) {
    }
}