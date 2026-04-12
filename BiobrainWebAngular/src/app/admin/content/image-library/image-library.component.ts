import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { Api } from '../../../api/api.service';
import { ApiPath } from '../../../api/api-path.service';
import { GetContentImagesQuery, ContentImageItem } from '../../../api/content-images/get-content-images.query';
import { UploadContentImageResult } from '../../../api/content-images/upload-content-image.command';

@Component({
  selector: 'app-image-library',
  templateUrl: './image-library.component.html',
  styleUrls: ['./image-library.component.scss'],
})
export class ImageLibraryComponent implements OnInit {
  images: ContentImageItem[] = [];
  totalCount = 0;
  search = '';
  page = 1;
  pageSize = 50;
  loading = false;
  uploading = false;

  uploadCode = '';
  uploadDescription = '';
  selectedFile: File | null = null;
  selectedFileName = '';

  successMessage = '';
  errorMessage = '';

  constructor(
    private readonly _api: Api,
    private readonly _apiPath: ApiPath,
    private readonly _http: HttpClient,
  ) {}

  ngOnInit(): void {
    this.loadImages();
  }

  async loadImages(): Promise<void> {
    this.loading = true;
    try {
      const result = await firstValueFrom(
        this._api.send(new GetContentImagesQuery(this.search, this.page, this.pageSize))
      );
      this.images = result.images;
      this.totalCount = result.totalCount;
    } catch {
      this.errorMessage = 'Failed to load images.';
    } finally {
      this.loading = false;
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.selectedFileName = this.selectedFile.name;
    }
  }

  async upload(): Promise<void> {
    if (!this.selectedFile || !this.uploadCode.trim()) {
      this.errorMessage = 'File and code are required.';
      return;
    }

    this.uploading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formData = new FormData();
    formData.set('file', this.selectedFile, this.selectedFile.name);
    formData.set('code', this.uploadCode.trim());
    formData.set('description', this.uploadDescription.trim());

    try {
      const url = `${this._apiPath.contentImages()}/Upload`;
      const result = await firstValueFrom(
        this._http.post<UploadContentImageResult>(url, formData)
      );
      this.successMessage = `Uploaded: ${result.code} → ${result.fileLink}`;
      this.resetUploadForm();
      this.loadImages();
    } catch {
      this.errorMessage = 'Upload failed.';
    } finally {
      this.uploading = false;
    }
  }

  onSearchChange(): void {
    this.page = 1;
    this.loadImages();
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.totalCount) {
      this.page++;
      this.loadImages();
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadImages();
    }
  }

  copyCode(code: string): void {
    navigator.clipboard.writeText(code);
    this.successMessage = `Copied: ${code}`;
    setTimeout(() => this.successMessage = '', 2000);
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1048576) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / 1048576).toFixed(1)} MB`;
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  private resetUploadForm(): void {
    this.uploadCode = '';
    this.uploadDescription = '';
    this.selectedFile = null;
    this.selectedFileName = '';
  }
}
