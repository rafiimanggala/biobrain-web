export interface QuizAnalyticOutput_Student {
  readonly firstName: string;
  readonly lastName: string;
  readonly studentId: string;
}

export interface QuizAnalyticOutput_CourseQuizInfo {
  readonly schoolClassId: string;
  readonly schoolClassName: string;
  readonly schoolClassYear: number;
}

export interface QuizAnalyticOutput_SubjectInfo {
  readonly subjectName: string;
  readonly courseId: string;
}

export interface QuizAnalyticOutput_AverageScoreData {
  readonly studentId: string;
  readonly averageScore: number;
  readonly notApplicable: boolean;
}

export interface QuizAnalyticOutput_ProgressData {
  readonly studentId: string;
  readonly progress: number;
}

export interface QuizAnalyticOutput_QuizAssignment {
  readonly contentTreeNodeId: string;
  readonly quizId: string;
  readonly quizAssignmentId: string;
  readonly path: string[];
  readonly assignedByTeacherId: string;
  readonly quizName: string;
}

export interface QuizAnalyticOutput_QuizStudentAssignment {
  readonly quizStudentAssignmentId: string;
  readonly quizAssignmentId: string;
  readonly assignedToUserId: string;
}


export function getStudentFullName(student: QuizAnalyticOutput_Student): string {
  return `${student.lastName} ${student.firstName}`;
}

export function studentByNameComparer(x1: QuizAnalyticOutput_Student, x2: QuizAnalyticOutput_Student): number {
  return getStudentFullName(x1).localeCompare(getStudentFullName(x2));
}
