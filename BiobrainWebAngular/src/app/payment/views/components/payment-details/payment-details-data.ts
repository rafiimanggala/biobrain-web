export class PaymentDetailsData{
    constructor(
        public cardToken: string,
        public promocode: string | null
    ){}
}