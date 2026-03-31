export class UnauthorizedException implements Error{
    constructor(public message: string){}
    name!: string;    
    stack?: string;
    status = 401;
}