export class PinCardModel{
    constructor(
        public publishable_api_key: string,
        public number: string,
        public expiry_month: string,
        public expiry_year: string,
        public cvc: string,
        public name: string,
        public address_line1: string,
        public address_city: string,
        public address_country: string,
    ){}
}