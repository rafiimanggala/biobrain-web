/* eslint-disable max-classes-per-file */

import Dexie from 'dexie';
import {
  ContentTree,
  ContentTreeMeta,
  ContentVersion,
  GlossaryTerm,
  Icon,
  Material,
  MaterialPage,
  Question,
  Quiz,
} from 'src/app/api/content/content-data-models';
import { hasValue } from 'src/app/share/helpers/has-value';
import { ContentDownloadData } from '../dialogs/content-download-dialog/content-download.dialog-data';

export class LearningContentDb extends Dexie {
  pages: Dexie.Table<MaterialPageRow, string>;
  quizzes: Dexie.Table<QuizRow, string>;
  contentForest: Dexie.Table<ContentTreeRow, string>;
  glossaryTerms: Dexie.Table<GlossaryTermRow, string>;
  contentVersions: Dexie.Table<ContentVersionRow>;

  constructor() {
    super('LearningContentDb');

    const initialVersionNumber = 7;
    this.version(initialVersionNumber).stores({
      pages: 'pageId, [contentTreeNodeId+courseId], courseId',
      quizzes: 'quizId, [contentTreeNodeId+courseId], courseId',
      glossaryTerms: 'termId, [ref+subjectCode], subjectCode',
      contentForest: 'nodeId, courseId',
      contentVersions: '++, courseId',
    })
    .upgrade((db) => db.table("contentVersions").clear());

    this.pages = this.table('pages');
    this.quizzes = this.table('quizzes');
    this.contentForest = this.table('contentForest');
    this.glossaryTerms = this.table('glossaryTerms');
    this.contentVersions = this.table('contentVersions');

    this.pages.mapToClass(MaterialPageRow);
    this.quizzes.mapToClass(QuizRow);
    this.contentForest.mapToClass(ContentTreeRow);
    this.glossaryTerms.mapToClass(GlossaryTermRow);
    this.contentVersions.mapToClass(ContentVersionRow);
  }

  async getPageById(pageId: string): Promise<MaterialPageRow> {
    const page = await this.pages.get(pageId);
    if (!page) {
      throw new Error(`MaterialPage with id='${pageId}' not found`);
    }

    return page;
  }

  findPageById(pageId: string): Promise<MaterialPage | null | undefined> {
    return this.pages.get(pageId);
  }

  findPageByContentTree(contentTreeNodeId: string, courseId: string): Promise<MaterialPageRow | undefined> {
    return this.pages.where({
      contentTreeNodeId,
      courseId,
    }).first();
  }

  getPagesForCourse(courseId: string): Promise<MaterialPageRow[]> {
    return this.pages.where('courseId').equals(courseId).toArray();
  }

  async getQuizById(quizId: string): Promise<QuizRow> {
    const quiz = await this.quizzes.get(quizId);
    if (!quiz) {
      throw new Error(`MaterialPage with id='${quizId}' not found`);
    }

    return quiz;
  }

  findQuizById(quizId: string): Promise<QuizRow | undefined> {
    return this.quizzes.get(quizId);
  }

  getAllContentTreeRows(): Promise<ContentTreeRow[]> {
    return this.contentForest.toArray();
  }

  getAllQuizRows(): Promise<QuizRow[]> {
    return this.quizzes.toArray();
  }

  getQuizzesForCourse(courseId: string): Promise<QuizRow[]> {
    return this.quizzes.where('courseId').equals(courseId).toArray();
  }

  findQuizByContentTree(contentTreeNodeId: string, courseId: string): Promise<QuizRow | undefined> {
    return this.quizzes.where({
      contentTreeNodeId,
      courseId,
    }).first();
  }

  findContentTreeForCourse(courseId: string): Promise<ContentTreeRow[]> {
    return this.contentForest.where('courseId').equals(courseId).toArray();
  }

  findContentTreeById(contentTreeId: string): Promise<ContentTreeRow | undefined> {
    return this.contentForest.get(contentTreeId);
  }

  findGlossaryTerms(ref: string, subjectCode: number): Promise<GlossaryTermRow | undefined> {
    return this.glossaryTerms.where({
      ref,
      subjectCode,
    }).first();
  }

  findGlossaryTermsForSubject(subjectCode: number): Promise<GlossaryTermRow[]> {
    return this.glossaryTerms.where({ subjectCode }).toArray();
  }

  async saveContent(content: ContentDownloadData[], actualVersion: ContentVersion[]): Promise<ContentVersion[]> {
    // Save each course independently — one failure should not block successful downloads
    const perCourseTasks = content.map(async c => {
      try {
        // Wait for the download (already subscribed in ContentDownloadData constructor)
        await c.ready;

        if (c.isError || !c.content) {
          c.isError = true;
          return;
        }

        const version = actualVersion.find(x => x.courseId == c.courseId);
        if (version == null) {
          c.isError = true;
          console.log(`Can't find version for course ${c.courseId}`);
          return;
        }

        await this.saveCourseContent(c.content, version);
        c.complete();
      } catch (e) {
        c.isError = true;
        console.error(`Failed to save course ${c.courseName}`, e);
      }
    });

    await Promise.all(perCourseTasks);
    return actualVersion;
  }

  saveCourseContent(content: Content, version: ContentVersion): Promise<ContentVersion>{
    return this.transaction('rw', this.contentForest, this.quizzes, this.pages, this.glossaryTerms, this.contentVersions, async () => {
      await this.table('quizzes').bulkDelete((await this.quizzes.where("courseId").equals(version.courseId).toArray()).map(_ => _.quizId));
      await this.table('quizzes').bulkAdd(content.quizzes);

      await this.pages.bulkDelete((await this.pages.where("courseId").equals(version.courseId).toArray()).map(_ => _.pageId));
      await this.table('pages').bulkAdd(content.pages);

      await this.contentForest.bulkDelete((await this.contentForest.where("courseId").equals(version.courseId).toArray()).map(_ => _.nodeId));
      await this.table('contentForest').bulkAdd(content.contentForest);

      var subjectCode = content.glossaryTerms.find(_ => hasValue(_.subjectCode))?.subjectCode ?? '';
      await this.glossaryTerms.bulkDelete((await this.glossaryTerms.where("subjectCode").equals(subjectCode).toArray()).map(_ => _.termId));
      await this.table('glossaryTerms').bulkAdd(content.glossaryTerms);

      let versionToDelete = await this.contentVersions.where("courseId").equals(version.courseId).primaryKeys();
      if(versionToDelete)
        await this.contentVersions.bulkDelete(versionToDelete);
      await this.contentVersions.add(new ContentVersionRow(0, 0, version.version, version.courseId));
      return version;
    });
  }


  getContentVersion(): Promise<ContentVersionRow[]> {
    return this.contentVersions?.toCollection().toArray();
  }
}

export interface Content {
  readonly quizzes: Quiz[];
  readonly pages: MaterialPage[];
  readonly contentForest: ContentTree[];
  readonly glossaryTerms: GlossaryTerm[];
}

export class GlossaryTermRow implements GlossaryTerm {
  constructor(
    public readonly termId: string,
    public readonly subjectCode: number,
    public readonly ref: string,
    public readonly term: string,
    public readonly definition: string,
    public readonly header: string,
  ) {
  }

  save(): Promise<string> {
    return learningContentDb.transaction('rw', learningContentDb.glossaryTerms, async () =>
      learningContentDb.glossaryTerms.put(this),
    );
  }
}

export class ContentTreeRow implements ContentTree {
  constructor(
    public readonly nodeId: string,
    public readonly courseId: string,
    public readonly parentId: string | undefined | null,
    public readonly name: string,
    public readonly order: number,
    public readonly icon: Icon | undefined | null,
    public readonly contentTreeMeta: ContentTreeMeta,
    public readonly nodes: ContentTreeRow[] | null | undefined,
    public readonly isAvailableInDemo: boolean = false
  ) {
  }

  save(): Promise<string> {
    return learningContentDb.transaction('rw', learningContentDb.contentForest, async () =>
      learningContentDb.contentForest.put(this),
    );
  }
}

export class QuizRow implements Quiz {
  constructor(
    public quizId: string,
    public courseId: string,
    public contentTreeNodeId: string,
    public questions: Question[],
    public name?: string,
  ) {
  }

  save(): Promise<string> {
    return learningContentDb.transaction('rw', learningContentDb.quizzes, async () =>
      learningContentDb.quizzes.put(this),
    );
  }
}

export class MaterialPageRow implements MaterialPage {
  constructor(
    public pageId: string,
    public courseId: string,
    public materials: Material[],
    public contentTreeNodeId: string,
  ) {
  }

  save(): Promise<string> {
    return learningContentDb.transaction('rw', learningContentDb.pages, async () => learningContentDb.pages.put(this));
  }
}

export class ContentVersionRow implements ContentVersion {
  constructor(public majorVersion: number, public minorVersion: number, public version: number=0, public courseId: string='') {
  }

  save(): Promise<unknown> {
    return learningContentDb.transaction('rw', learningContentDb.contentVersions, async () =>
      learningContentDb.contentVersions.put(this),
    );
  }
}

export const learningContentDb = new LearningContentDb();
