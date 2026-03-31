
export class TemplateDialogData {
  constructor(

    public courses: { courseId: string, name: string, isSelected: boolean }[],
    public types: { key: number, value: string }[],

    public templateId: string | null,
    public template: string,
    public templateType: number,
    public courseIds: string[],
  ) { }
}
