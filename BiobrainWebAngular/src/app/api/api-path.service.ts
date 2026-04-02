import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable()
export class ApiPath {
  public get base(): string {
    const prefix = environment.apiUrl ? environment.apiUrl : '';
    return `${prefix}/api`;
  }

  public userGuide(): string {
    return `${this.base}/UserGuide`;
  }

  public schools(): string {
    return `${this.base}/Schools`;
  }

  public teachers(): string {
    return `${this.base}/Teachers`;
  }

  public students(): string {
    return `${this.base}/Students`;
  }

  public schoolClasses(): string {
    return `${this.base}/SchoolClasses`;
  }

  public quizzesAnalytic(): string {
    return `${this.base}/QuizzesAnalytic`;
  }

  public subjects(): string {
    return `${this.base}/Subjects`;
  }

  public templates(): string {
    return `${this.base}/Templates`;
  }

  public curricula(): string {
    return `${this.base}/Curricula`;
  }

  public courses(): string {
    return `${this.base}/Courses`;
  }

  public content(): string {
    return `${this.base}/Content`;
  }

  public quizAssignments(): string {
    return `${this.base}/QuizAssignments`;
  }

  public quizResults(): string {
    return `${this.base}/QuizResults`;
  }

  public learningMaterialAssignments(): string {
    return `${this.base}/LearningMaterialAssignments`;
  }

  public accounts(): string {
    return `${this.base}/Accounts`;
  }

  public assignedWork(): string {
    return `${this.base}/AssignedWork`;
  }

  public bookmarks(): string {
    return `${this.base}/Bookmarks`;
  }

  public payment(): string{
    return `${this.base}/Payment`;
  }

  public report(): string{
    return `${this.base}/Report`;
  }

  public userTracking(): string{
    return `${this.base}/UserTracking`;
  }

  public accessCodes(): string{
    return `${this.base}/AccessCodes`;
  }

  public vouchers(): string{
    return `${this.base}/Vouchers`;
  }

  public ai(): string {
    return `${this.base}/ai`;
  }

  public whatsNew(): string {
    return `${this.base}/WhatsNew`;
  }

  public feedback(): string {
    return `${this.base}/Feedback`;
  }
}
