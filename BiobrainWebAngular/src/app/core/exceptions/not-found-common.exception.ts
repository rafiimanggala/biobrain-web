export class NotFoundCommonException implements Error{
    name!: string;
    message!: string;
    stack?: string;
}