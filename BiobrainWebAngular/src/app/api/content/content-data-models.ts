export interface ContentVersion {
  version: number;
  courseId: string;
}

export interface ActualContentVersion {
  version: number;
  courseId: string;
  courseName: string;
}

export interface GlossaryTerm {
  termId: string;
  subjectCode: number;
  ref: string;
  term: string;
  definition: string;
  header: string;
}

export interface MaterialPage {
  readonly pageId: string;
  readonly courseId: string;
  readonly contentTreeNodeId: string;
  readonly materials: Material[];
}

export interface Material {
  readonly materialId: string;
  readonly text: string;
  readonly header: string;
  readonly order: number;
  readonly videoLink: string;
}

export interface ContentTree {
  readonly nodeId: string;
  readonly courseId: string;
  readonly parentId: string | undefined | null;
  readonly name: string;
  readonly order: number;
  readonly icon: Icon | undefined | null;
  readonly contentTreeMeta: ContentTreeMeta;
  readonly nodes: ContentTree[] | null | undefined;
  readonly isAvailableInDemo: boolean;
}

export interface ContentTreeMeta {
  readonly contentTreeMetaId: string;
  readonly name: string;
  readonly depth: number;
  readonly couldAddEntry: boolean;
  readonly couldAddContent: boolean;
  readonly autoExpand: boolean;
}

export interface Icon {
  readonly reference: string;
  readonly name: string;
  readonly fileName: string;
}

export interface Quiz {
  readonly quizId: string;
  readonly courseId: string;
  readonly contentTreeNodeId: string;
  readonly questions: Question[];
}

export interface Question {
  readonly questionId: string;
  readonly questionTypeCode: string;
  readonly questionTypeName: string;
  readonly header: string;
  readonly text: string;
  readonly hint: string;
  readonly feedBack: string;
  readonly answers: Answer[];
}

export interface Answer {
  readonly answerId: string;
  readonly answerOrder: number;
  readonly text: string;
  readonly isCorrect: boolean;
  readonly caseSensitive: boolean;
  readonly score: number;
  readonly response: number;
}
