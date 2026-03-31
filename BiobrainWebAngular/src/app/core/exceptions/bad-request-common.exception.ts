export class BadRequestCommonException implements Error{
    name!: string;
    message!: string;
    stack?: string;
    error!: string;
}