import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.quard';
import { BaseAdminLayoutComponent } from './base-admin-layout/base-admin-layout.component';
import { NgModule } from '@angular/core';
import { SchoolBaseComponent } from './schools/school-base/school-base.component';
import { SchoolClassListComponent } from './school-classes/school-class-list/school-class-list.component';
import { SchoolListComponent } from './schools/school-list/school-list.component';
import { StudentListComponent } from './students/student-list/student-list.component';
import { TeacherListComponent } from './teachers/teacher-list/teacher-list.component';
import { UserRoles } from '../share/values/user-roles.enum';
import { ContentMapperComponent } from './content/content-mapper/content-mapper.component';
import { PurchaseReportComponent } from './reports/purchase-report/purchase-report.component';
import { AllStudentsPageComponent } from './students/all-students-page/all-students-page.component';
import { UsageReportComponent } from './reports/usage-report/usage-report.component';
import { ContentLoaderComponent } from './content/content-loader/content-loader.component';
import { ContentImportComponent } from './content/content-import/content-import.component';
import { AccessCodesListComponent } from './access-codes/access-codes-list/access-codes-list.component';
import { TemplatesListComponent } from './templates/templates-list/templates-list.component';
import { ContentReportComponent } from './reports/content-report/content-report.component';
import { UserGuidesListComponent } from './user-guides/user-guides-list/user-guides-list.component';
import { UserGuidesComponent } from '../share/components/user-guides/user-guides.component';
import { VoucherListComponent } from './vouchers/vouchers-list/voucher-list.component';
import { ImageLibraryComponent } from './content/image-library/image-library.component';

const routes: Routes = [
  {
    path: 'admin',
    component: BaseAdminLayoutComponent,
    data: {
      roles: [UserRoles.systemAdministrator],
      navigatingSave: true,
    },
    canActivate: [AuthGuard],

    children: [
      {
        path: 'content_mapper',
        component: ContentMapperComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'content_loader',
        component: ContentLoaderComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'content_import',
        component: ContentImportComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'content_report',
        component: ContentReportComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'access_codes',
        component: AccessCodesListComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'vouchers',
        component: VoucherListComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'purchase_report',
        component: PurchaseReportComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'usage_report',
        component: UsageReportComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'user_guides',
        component: UserGuidesListComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'user_guides_preview',
        component: UserGuidesComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'schools',
        component: SchoolListComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'students',
        component: AllStudentsPageComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'templates',
        component: TemplatesListComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'image_library',
        component: ImageLibraryComponent,
        data: {
          roles: [UserRoles.systemAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },

    ]
  },
  {
    path: 'admin',
    component: BaseAdminLayoutComponent,
    data: {
      roles: [UserRoles.systemAdministrator, UserRoles.schoolAdministrator],
      navigatingSave: true,
    },
    canActivate: [AuthGuard],

    children: [
      {
        path: 'schools/:schoolId',
        component: SchoolBaseComponent,
        data: {
          roles: [UserRoles.systemAdministrator, UserRoles.schoolAdministrator],
          navigatingSave: true,
        },
        canActivate: [AuthGuard],
        children: [
          {
            path: 'classes',
            component: SchoolClassListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'teachers',
            component: TeacherListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'students',
            component: StudentListComponent,
            canActivate: [AuthGuard]
          },
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule { }
