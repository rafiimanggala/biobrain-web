import { Injectable } from '@angular/core';
import { NavigationExtras, Router, UrlTree } from '@angular/router';

import { hasValue } from '../../share/helpers/has-value';

import { HomePageService } from './home-page.service';

@Injectable()
export class RoutingService {
  constructor(
    private readonly _router: Router,
    private readonly _homePageService: HomePageService,
  ) {
  }

  async navigateToUrl(url: string): Promise<boolean> {
    return this._router.navigateByUrl(url);
  }

  async homePage(): Promise<UrlTree> {
    const homePage = await this._homePageService.getHomePage();
    return this._router.parseUrl(homePage);
  }

  async navigateToHome(isFromSignUp = false): Promise<boolean> {
    const homePage = await this._homePageService.getHomePage();
    return isFromSignUp ? this._router.navigate([homePage], { queryParams: { 'signup': isFromSignUp } }) : this._router.navigate([homePage]);
  }

  navigateToResetPassword(): Promise<boolean> {
    return this._router.navigate(['/reset-password']);
  }

  navigateToUserGuides(): Promise<boolean> {
    return this._router.navigate(['/user_guides']);
  }

  navigateToSignUp(): Promise<boolean> {
    return this._router.navigate(['/sign-up']);
  }

  navigateToLoginPage(backUrl?: string): Promise<boolean> {
    return this._router.navigate(['/login'], { queryParams: { backUrl } });
  }

  navigateToSubscriptionPage(signup = false, freeTrial = false): Promise<boolean> {
    let prefix = freeTrial ? '/freetrial' : '';
    return signup ? this._router.navigate([prefix + '/subscription'], { queryParams: { signup } }) : this._router.navigate([prefix + '/subscription']);
  }

  navigateToMaterialPage(courseId: string, topicId: string | undefined, levelId: string | undefined): Promise<boolean> {
    return this._router.navigate([`/materials/course/${courseId}`], { queryParams: { topicId, levelId } });
  }

  navigateToMaterialsSearch(courseId: string, term: string, replaceUrl = false): Promise<boolean> {
    return this._router.navigate([`/materials-search/course/${courseId}`], { queryParams: { term }, replaceUrl });
  }

  navigateToQuizPage(quizResultId: string, questionId?: string, extras?: NavigationExtras): Promise<boolean> {
    if (hasValue(questionId)) {
      return this._router.navigate([`/quiz/${quizResultId}/question/${questionId}`], extras);
    }

    return this._router.navigate([`/quiz/${quizResultId}`], extras);
  }

  navigateToViewQuizPage(quizId: string, extras?: NavigationExtras): Promise<boolean> {
    return this._router.navigate([`/quiz-overview/${quizId}`], extras);
  }

  navigateToQuizResultPage(quizResultId: string, questionId?: string, extras?: NavigationExtras): Promise<boolean> {
    if (hasValue(questionId)) {
      return this._router.navigate([`/quiz-result/${quizResultId}/question/${questionId}`], extras);
    }

    return this._router.navigate([`/quiz-result/${quizResultId}`], extras);
  }

  navigateToTeachersAdminPage(schoolId: string): Promise<boolean> {
    return this._router.navigate([`admin/schools/${schoolId}/teachers`]);
  }

  getMaterialUrl(courseId: string, nodeId: string): string {
    return `/materials/course/${courseId}?topicId=${nodeId}`;
  }

  myClasses(): UrlTree {
    return this._router.parseUrl('/teacher/my-classes');
  }

  classResults(): UrlTree {
    return this._router.parseUrl('/teacher/class-results');
  }

  learningMaterials(courseId: string): UrlTree {
    return this._router.parseUrl(`/materials/course/${courseId}`);
  }

  navigateToClassResults(): Promise<boolean> {
    return this._router.navigate(['/teacher/class-results']);
  }

  navigateToClassQuizAssignmentResult(quizAssignmentId: string): Promise<boolean> {
    return this._router.navigate([`/teacher/quiz-result/${quizAssignmentId}`]);
  }

  navigateToStudentQuizResults(studentId: string): Promise<boolean> {
    return this._router.navigate([`/teacher/student-results/${studentId}`]);
  }

  glossary(): UrlTree {
    return this._router.parseUrl('glossary');
  }

  periodicTable(): UrlTree {
    return this._router.parseUrl('periodic-table');
  }

  classAdmin(): UrlTree {
    return this._router.parseUrl('/teacher/class-admin');
  }

  workAssigned(): UrlTree {
    return this._router.parseUrl('/teacher/work-assigned');
  }

  teacherCustomQuiz(): UrlTree {
    return this._router.parseUrl('/teacher/custom-quiz');
  }

  quizTemplates(): UrlTree {
    return this._router.parseUrl('/teacher/quiz-templates');
  }

  aiPracticeSet(): UrlTree {
    return this._router.parseUrl('/teacher/ai-practice-set');
  }

  aiInsights(): UrlTree {
    return this._router.parseUrl('/teacher/ai-insights');
  }

  quizResultHistory(): UrlTree {
    return this._router.parseUrl('student/quiz-result-history');
  }

  customQuiz(): UrlTree {
    return this._router.parseUrl('student/custom-quiz');
  }

  bookmarks(): UrlTree {
    return this._router.parseUrl('bookmarks');
  }

  schoolAdminTeachers(schoolId: string): UrlTree {
    return this._router.parseUrl(`admin/schools/${schoolId}/teachers`);
  }

  navigateToMyCourses(): Promise<boolean> {
    return this._router.navigate(['/student/my-courses']);
  }

  async navigateToAssignedWork(rootNodeId: string): Promise<boolean> {
    return this._router.navigate([`/assigned-work/${rootNodeId}`]);
  }

  // getBasePartUrl(role: UserRoles): string {
  //   switch (role) {
  //     case UserRoles.schoolAdministrator:
  //     case UserRoles.systemAdministrator:
  //       return 'admin';
  //     case UserRoles.student:
  //       return '';
  //     case UserRoles.teacher:
  //       return 'teacher';
  //   }
  // }
}
