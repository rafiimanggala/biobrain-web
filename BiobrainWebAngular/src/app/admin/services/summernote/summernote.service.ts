import { EventEmitter, Injectable, NgZone } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CodeviewToggleEventModel } from './codeview.toggle.event.model';
import { ImageModel } from './image.model';
import { AddImageOperation } from '../../operations/images/add-mage.operation';
import { SelectImageOperation } from '../../operations/images/select-image.operation';
import { Api } from 'src/app/api/api.service';
import { UploadUserGuideImageCommand } from 'src/app/api/user-guides/upload-user-guide-image.command';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { StringsService } from 'src/app/share/strings.service';
declare var $: any;


@Injectable()
export class SummernoteService {

    public codeviewToggled: EventEmitter<CodeviewToggleEventModel> = new EventEmitter();

    constructor(
        private readonly addImageOperation: AddImageOperation,
        private readonly selectImageOperation: SelectImageOperation,
        private readonly route: ActivatedRoute,
        private readonly http: HttpClient,
        private readonly api: Api,
        private readonly appEvent: AppEventProvider,
        private readonly strings: StringsService,
        // Need to invoke methods from js
        private ngZone: NgZone,
    ) {
    }

    popovers = {
        image: [
            ['remove', ['removeMedia']]
        ],
        link: [
            ['link', ['linkDialogShow', 'unlink']]
        ],
        table: [
            ['add', ['addRowDown', 'addRowUp', 'addColLeft', 'addColRight']],
            ['delete', ['deleteRow', 'deleteCol', 'deleteTable']],
        ],
        air: [
          ['color', ['color']],
          ['font', ['bold', 'underline', 'clear']]
        ]
    };

    buttons = {
        'addImg': this.addImageButton(),
        // 'selectImg': this.selectImgButton(),
        // 'smallFont': this.smallFontButton(),
        // 'addTerm': this.addTermButton(),
        // 'linkArea': this.linkAreaButton(),
        // 'linkMaterial': this.linkMaterialButton(),
        // 'addFormula': this.addFormulaButton(),
    };

    callbacks = {
        onPaste: (event: any) => { this.onPaste(event); },
        onInit: (event: any) => { this.onInit(event); },
        onImageUpload: (files: FileList) => { this.uploadDroppedImages(files); }
    }

    get materialsConfig(): any {
        let config = {
            height: '250px',
            tabDisable: true,
            disableDragAndDrop: false,
            maximumImageFileSize: 5 * 1024 * 1024, // 5 MB
            // dialogsInBody: true,
            // dialogsFade: true,
            colors: [
                ['#004876','#00a1df'],
                ['#e30918', '#09552a', '#fec50e', '#ffffff', '#55565a', '#000000'],
            ],
            toolbar: [
                ['font', ['bold', 'italic', 'underline', 'superscript', 'subscript', 'smallFont']],
                ['color', ['forecolor']],
                ['fontsize', ['fontsize', 'fontsizeunit']],
                ['para', ['paragraph', 'ul', 'ol']],
                ['insert', ['table', 'link']],
                // ['formulas', ['addFormula']],
                // ['insert', ['equation']],
                //Custom tools
                ['images', ['addImg']],
                // ['links', ['addTerm', 'linkArea', 'linkMaterial']],
                //
                ['misc', ['undo', 'redo']],
            ],
            popover: this.popovers,
            buttons: this.buttons,
            callbacks: this.callbacks
        };

        // if (this.user.roleId == UserRole.Admin)
        //     config.toolbar.push(['codeView', ['codeview']]);
        return config;
    };    

    addImageButton() {
        return (context: any) => {
            const ui = ($ as any).summernote.ui;
            console.log(ui);
            const button = ui.button({
                contents: '<mat-icon class="mat-icon material-icons toolbar-icon" role="img">add_photo_alternate</mat-icon>',
                tooltip: 'Add image',
                container: 'body',
                click: ((event: any) => {
                    event.currentTarget.blur();
                    this.ngZone.run(() => { this.AddImage(context) });

                }).bind(this)
            });
            return button.render();
        }
    }

    async AddImage(context: any) {
        context.invoke('editor.saveRange');
        const result = await this.addImageOperation.perform();
        if (result.isFailed()) {
            console.log(result.reason);
            return;
        }

        const image = <ImageModel>result.data;

        if (image.url == null || image.url == undefined || image.url.length < 1)
            return;

        context.invoke('editor.restoreRange');
        context.invoke('editor.focus');
        context.invoke('editor.insertImage', image.url, function ($image: any) {});
    }

    // selectImgButton() {
    //     return ((context: any) => {
    //         const ui = (<any>$).summernote.ui;
    //         const button = ui.button({
    //             contents: '<span><mat-icon class="mat-icon material-icons toolbar-icon" role="img">image_search</mat-icon></span>',
    //             tooltip: 'Select image',
    //             container: 'body',
    //             click: (function (event: any) {
    //                 event.currentTarget.blur();
    //                 this.ngZone.run(() => { this.selectImage(context) });
    //             }).bind(this)
    //         });
    //         return button.render();
    //     }).bind(this);
    // }

    // async selectImage(context: any): Promise<void> {
    //     context.invoke('editor.saveRange');
    //     const result = await this.selectImageOperation.perform();
    //     if (result.isFailed()) {
    //         alert(result.reason);
    //         return;
    //     }
        
    //     const image = <ImageModel>result.data;

    //     if (image.url == null || image.url == undefined || image.url.length < 1)
    //         return;

    //     context.invoke('editor.restoreRange');
    //     context.invoke('editor.focus');
    //     context.invoke('editor.insertImage', image.url, function ($image: any) {    
            
    //       });
    // }

    

    getLastRouteChild(root: ActivatedRoute) {
        var currentRoute = root;
        while (currentRoute.firstChild) {
            currentRoute = currentRoute.firstChild;
        }
        return currentRoute;
    }

    onInit(event: any) {
        event.note.on('summernote.codeview.toggled', ((e: any) => {
            var event = new CodeviewToggleEventModel();
            // event.isActive = ($(e.target)).summernote('codeview.isActivated');
            event.editorId = e.target.id;
            this.codeviewToggled.emit(event);
        }).bind(this));
    }

    onPaste(e: any) {
        var bufferText = ((e.originalEvent || e).clipboardData).getData('Text');
        e.preventDefault();
        document.execCommand('insertText', false, bufferText);
    }

    private uploadDroppedImages(files: FileList): void {
        if (!files || files.length === 0) return;
        // Capture the currently-focused summernote editor so we can insert into it
        const activeEditor = $('.note-editable:focus').closest('.note-editor').find('.note-editable')[0]
            || document.activeElement;
        const $editor = activeEditor ? $(activeEditor).closest('.note-editor').prev() : null;
        this.ngZone.run(async () => {
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (!file.type || file.type.indexOf('image/') !== 0) {
                    this.appEvent.errorEmit('Only image files can be dropped.');
                    continue;
                }
                try {
                    const result = await firstValueFrom(this.api.sendFile(new UploadUserGuideImageCommand(file)));
                    if (result && result.fileLink && $editor && $editor.length) {
                        $editor.summernote('insertImage', result.fileLink, (img: any) => {
                            img.css('max-width', '100%');
                        });
                    }
                } catch (err) {
                    this.appEvent.errorEmit(this.strings.errors.errorSavingDataOnServer);
                }
            }
        });
    }
}