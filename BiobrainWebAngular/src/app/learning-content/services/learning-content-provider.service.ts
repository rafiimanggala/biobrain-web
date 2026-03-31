import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { GlossaryTerm, MaterialPage, Quiz } from 'src/app/api/content/content-data-models';

import { LoaderService } from '../../share/services/loader.service';

import { ContentTreeRow, learningContentDb, QuizRow } from './learning-content-db';
import { EnsureContentStates, LearningContentLoaderService } from './learning-content-loader.service';

@Injectable({
  providedIn: 'root',
})
export class LearningContentProviderService {
  constructor(
    private readonly _loader: LearningContentLoaderService,
    private readonly _loaderService: LoaderService
  ) {
  }

  async getPageByNodeId(courseId: string, nodeId: string): Promise<MaterialPage | undefined> {
    await this._ensureLearningContent();
    return learningContentDb.findPageByContentTree(nodeId, courseId);
  }

  async getQuizById(quizId: string): Promise<Quiz | undefined> {
    await this._ensureLearningContent();
    return learningContentDb.findQuizById(quizId);
  }

  async getAllContentTreeRows(): Promise<ContentTreeRow[]> {
    await this._ensureLearningContent();
    return learningContentDb.getAllContentTreeRows();
  }

  async getAllQuizRows(): Promise<QuizRow[]> {
    await this._ensureLearningContent();
    return learningContentDb.getAllQuizRows();
  }

  async getQuizzByNodeId(courseId: string, nodeId: string): Promise<Quiz | undefined> {
    await this._ensureLearningContent();
    return learningContentDb.findQuizByContentTree(nodeId, courseId);
  }

  async getPagesForCourse(courseId: string): Promise<MaterialPage[]> {
    await this._ensureLearningContent();
    return learningContentDb.getPagesForCourse(courseId);
  }

  async getQuizzesForCourse(courseId: string): Promise<Quiz[]> {
    await this._ensureLearningContent();
    return learningContentDb.getQuizzesForCourse(courseId);
  }

  async getContentTreeForCourse(courseId: string): Promise<ContentTreeRow[]> {
    await this._ensureLearningContent();
    return learningContentDb.findContentTreeForCourse(courseId);
  }

  async getContentTreeNodeById(nodeId: string): Promise<ContentTreeRow|undefined> {
    await this._ensureLearningContent();
    return learningContentDb.findContentTreeById(nodeId);
  }

  async getGlossaryForSubject(subjectCode: number): Promise<GlossaryTerm[]> {
    await this._ensureLearningContent();
    return learningContentDb.findGlossaryTermsForSubject(subjectCode);
  }

  async getTermByRef(ref: string, subjectCode: number): Promise<GlossaryTerm | undefined> {
    await this._ensureLearningContent();
    return learningContentDb.findGlossaryTerms(ref, subjectCode);
  }

  private async _ensureLearningContent(): Promise<void> {
    await this._loader.ensureAnyVersionOfLearningContent().pipe(
      tap({
        next: x => {
          switch (x) {
            case EnsureContentStates.loading:
              this._loaderService.show();
              break;
            case EnsureContentStates.loaded:
              this._loaderService.hide();
              break;
            case EnsureContentStates.errorWhileLoading:
              this._loaderService.hide();
              break;
          }
        },
        error: () => {
          this._loaderService.hide();
        }
      })
    ).toPromise();
  }
}
