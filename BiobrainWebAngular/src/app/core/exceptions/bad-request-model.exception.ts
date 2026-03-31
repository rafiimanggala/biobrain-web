export class BadRequestModelException{
    constructor(
        public json: any
    ){}
    message!: string;
}