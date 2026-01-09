import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, catchError, Observable, of, retry,throwError } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ContentDescriptorModel, VariantDescriptor } from '../../api/models';
import { MediaServerService } from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  contents: BehaviorSubject<ContentDescriptorModel[]> = new BehaviorSubject<
    ContentDescriptorModel[]
  >([] as ContentDescriptorModel[]);
  TranslationConstants = TranslationConstants;
  constructor(
    private media: MediaServerService,
    private moryxSnackbar: MoryxSnackbarService,
    private translate: TranslateService
  ) {}

  loadContents(): void {
    this.media
      .getAll()
      .pipe(
        catchError(
          this.handleError<ContentDescriptorModel[]>('Retrieving Contents', [])
        )
      )
      .subscribe((response) => {
        this.contents.next(response);
      });
  }

  loadContent(id: string): Observable<ContentDescriptorModel> {
    return this.media
      .get({ guid: id })
      .pipe(
        catchError(
          this.handleError<ContentDescriptorModel>('Retrieving Contents')
        )
      );
  }

  getContent(id: string): ContentDescriptorModel | undefined {
    let contentValues = this.contents.getValue();
    return contentValues.find((c) => c.id === id);
  }

  removeContent(id: string): Observable<any> {
    return this.media
      .removeContent({ guid: id })
      .pipe(catchError(this.handleError<any>('Removing content')));
  }

  removeVariant(id: string, variantName: string): Observable<any> {
    return this.media
      .removeVariant({ guid: id, variantName: variantName })
      .pipe(catchError(this.handleError<any>('Removing variant')));
  }

  uploadContent(file: File): void {
    this.media.addMaster({ body: { formFile: file } }).subscribe({
      next: (data) => {
        this.loadContent(data).subscribe({
          next: (x) => {
            if (x !== undefined) {
              // Delay until server generate the preview
             this.wait(1000).then(() => {
                this.contents.value.push(x);
             });
            }
          },
        });
      },
      error: async (err: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(err);
      },
    });
  }

  uploadVariant(
    id: string,
    variantName: string,
    file: File
  ): Observable<string> {
    return this.media
      .addVariant({
        contentId: id,
        variantName: variantName,
        body: { formFile: file },
      })
      .pipe(
        catchError(this.handleError<string>('Upload variant', {} as string))
      );
  }

  getPicture(
    variantName: string,
    contentGuid: string,
    preview: boolean
  ): Observable<Blob> {
    return this.media
      .getVariantStream$Json({
        guid: contentGuid,
        variantName: variantName,
        preview: preview,
      })
  }

  getVariant(
    variantName: string,
    contentGuid: string
  ): Observable<VariantDescriptor> {
    return this.media
      .getVariant({ guid: contentGuid, variantName: variantName })
      .pipe(
        catchError(this.handleError<VariantDescriptor>('Retrieving variant'))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(error);
      this.moryxSnackbar.handleError(error);
      return of(result as T);
    };
  }

  wait(milliseconds: number) {
    return new Promise((vars) => setTimeout(vars, milliseconds));
  }
}
