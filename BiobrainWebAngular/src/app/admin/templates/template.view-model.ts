import { TemplateTypes } from "./template-types.enum";

export class TemplateViewModel{
    constructor(
    public templateId: string,
    public template: string,
    public templateType: TemplateTypes,
    public courses: {id: string, name: string}[]
    ){}

}