export class HistoryQuizResult {
  constructor(
    public readonly quizResultId: string,
    public readonly quizId: string,
    public readonly courseId: string,
    public readonly unitId: string,
    public readonly nodeId: string,
    public readonly parentNodeId: string | null,
    public readonly path: string[],
    public readonly nameLines: string[],
    public readonly score: number,
    public readonly date: string
  ) {
  }
}
