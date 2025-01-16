const MAX_IMAGE_SIZE = 2 * 1024 * 1024; // 2MB
const ERROR_MESSAGES = {
    IMAGE_TOO_LARGE: 'Image size exceeds 2 MB. Please upload a smaller image.',
};
let imageEventTriggered = false;

window.initializeQuill = function (dotnetHelper) {
    var editorElement = document.querySelector('#editor');
    if (editorElement) {
        const toolbarOptions = {
            container: [
                ['bold', 'italic', 'underline', 'strike'], // toggled buttons
                ['blockquote', 'code-block'],
                ['link', 'image', 'formula'], // includes the image button
                [{ 'header': 1 }, { 'header': 2 }], // custom button values
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                [{ 'script': 'sub' }, { 'script': 'super' }],
                [{ 'indent': '-1' }, { 'indent': '+1' }],
                [{ 'direction': 'rtl' }],
                [{ 'size': ['small', false, 'large', 'huge'] }],
                [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                [{ 'color': [] }, { 'background': [] }],
                [{ 'font': [] }],
                [{ 'align': [] }],
                ['clean'] // remove formatting button
            ],
            handlers: {
                image: function () {
                    selectLocalImage(dotnetHelper, this.quill); // Custom handler for the image button
                }
            }
        };

        // Initialize Quill and store it globally for later disposal
        window.quillEditorInstance = new Quill('#editor', {
            modules: {
                toolbar: toolbarOptions
            },
            theme: 'snow'
        });

        window.quillEditorInstance.root.addEventListener('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            imageEventTriggered = true;
            const dataTransfer = e.dataTransfer;
            if (dataTransfer && dataTransfer.items) {
                for (let i = 0; i < dataTransfer.items.length; i++) {
                    const item = dataTransfer.items[i];
                    if (item.kind === 'file' && item.type.startsWith('image/')) {
                        const file = item.getAsFile();
                        if (file) {
                            handleImageFile(file, dotnetHelper);
                        }
                    }
                }
            }
        });
        //quill.on('text-change', () => {
        //    try {
        //        removeLargeImagesFromQuill(quill);
        //    } catch (error) {
        //        console.error('Error during text-change event:', error);
        //    }
        //});
        window.quillEditorInstance.root.addEventListener('dragover', function (e) {
            e.preventDefault(); // Allow the drop event
            e.stopPropagation();
        });

        // Handle paste events
        window.quillEditorInstance.root.addEventListener('paste', function (e) {
            e.preventDefault();
            e.stopPropagation();
            imageEventTriggered = true;
            const clipboardData = e.clipboardData;
            if (clipboardData && clipboardData.items) {
                for (let i = 0; i < clipboardData.items.length; i++) {
                    const item = clipboardData.items[i];
                    if (item.type.startsWith('image/')) {
                        const file = item.getAsFile();
                        if (file) {
                            handleImageFile(file, dotnetHelper);
                            // removeLargeImagesFromQuill(quill);  // Call removeLargeImagesFromQuill on paste
                        }
                    }
                }
            }
        });
        // Sync Quill content on text change
        window.quillEditorInstance.on('text-change', function () {
            try {
                if (imageEventTriggered) { // Only remove large images if triggered by paste or drop
                    removeLargeImagesFromQuill(window.quillEditorInstance);
                }
            } catch (error) {
                console.error('Error during text-change event:', error);
            } finally {
                imageEventTriggered = false; // Always reset the flag
            }
            var emailBodyContent = window.quillEditorInstance.root.innerHTML;
            dotnetHelper.invokeMethodAsync('UpdateEmailBody', emailBodyContent)
                .catch(error => console.error('Error updating email body:', error));
        });
        console.log('Quill editor initialized successfully');
    } else {
        console.error('Quill editor element not found');
    }

    function removeLargeImagesFromQuill(quill, maxSizeInBytes = MAX_IMAGE_SIZE) {
        if (!quill || !quill.getContents || !quill.setContents) {
            console.error('Quill instance is not available or improperly initialized.');
            return;
        }
        try {
            const delta = quill.getContents();
            const ops = delta.ops;
            const newOps = []; // Array to store filtered operations

            for (const op of ops) {
                if (typeof op.insert === 'object' && op.insert.image) {
                    const imageData = op.insert.image;

                    if (typeof imageData === 'string' && imageData.startsWith('data:image')) {
                        const base64Data = imageData.split(',')[1];
                        const imageSizeInBytes = atob(base64Data).length; // Calculate size

                        if (imageSizeInBytes > maxSizeInBytes) {
                            console.warn('Removed large image (size:', imageSizeInBytes, 'bytes)');
                            // Do not add this op to the newOps array = remove this picture
                            continue;
                        }
                    }
                }
                newOps.push(op); // Keep this operation
            }
            // Only update the content if changes were made
            if (JSON.stringify(delta.ops) !== JSON.stringify(newOps)) {
                quill.setContents({ ops: newOps });
            }
        } catch (error) {
            console.error('Error in remove LargeImages From Quill:', error);
        }
    }
    // Function to handle individual image file items
    function handleImageFile(file, dotnetHelper) {
        if (file.size > 2 * 1024 * 1024) {
            console.error('Image size exceeds 2 MB. Please .');
            alert(ERROR_MESSAGES.IMAGE_TOO_LARGE);
            return;
        }
        const reader = new FileReader();
        reader.onload = async function (event) {
            try {
                const base64Image = event.target.result.split(',')[1]; // Get only the base64 part
                await dotnetHelper.invokeMethodAsync('UploadImage', base64Image,file.name);
          
            } catch (error) {
                console.error('Error handling image:', error);
            }
        };
        reader.readAsDataURL(file);
    }
    function selectLocalImage(dotnetHelper, quill) {
        const input = document.createElement('input');
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/*');
        input.click();

        input.onchange = async function () {
            const file = input.files[0];
            if (file) {
                if (file.size > 2 * 1024 * 1024) {
                    console.error('Image size exceeds 2 MB. Please .');
                    alert(ERROR_MESSAGES.IMAGE_TOO_LARGE);
                    return;
                }
                const reader = new FileReader();
                reader.onload = async function (e) {
                    try {
                        const base64Image = e.target.result.split(',')[1]; // Get only the base64 part
                        const mimeType = file.type; 
                        await dotnetHelper.invokeMethodAsync('UploadImage', base64Image, file.name);

                        // Insert the image into the editor using the base64 data
                        const range = quill.getSelection();
                       // quill.insertEmbed(range.index, 'image', `data:image/png;base64,${base64Image}`);
                        quill.insertEmbed(range.index, 'image', `data:${mimeType};base64,${base64Image}`);


                    } catch (error) {
                        console.error('Image upload failed:', error);
                    }
                };
                reader.readAsDataURL(file);
            }
        };
    }
};

window.disposeQuill = function () {
    // Check if the global instance of Quill exists
    if (window.quillEditorInstance) {
        var quill = window.quillEditorInstance;
        var el = document.querySelector('#editor');

        if (quill && el) {
            console.log('Call destroy_quill to fully destroy the Quill instance');
            // Call destroy_quill to fully destroy the Quill instance
            destroy_quill(quill, el);
            // Reset the global Quill instance
            window.quillEditorInstance = null;
            console.log('Quill editor disposed');
        }
    }
};

function destroy_quill(quill, el) {
    // Remove toolbar if it exists
    console.log('start disposed');
    if (quill.theme.modules.toolbar) {
        quill.theme.modules.toolbar.container.remove();
    }
    console.log('Quill clipboard disposed');
    // Remove clipboard if it exists
    //if (quill.theme.modules.clipboard) {
    //    quill.theme.modules.clipboard.container.remove();
    //}
    console.log('Quill tooltip disposed');
    // Remove tooltip if it exists
    if (quill.theme.tooltip) {
        quill.theme.tooltip.root.remove();
    }
    console.log('Quill all disposed');
    el.classList.forEach((cls) => {
        if (cls.startsWith("ql-")) {
            requestAnimationFrame(() => {
                el.classList.remove(cls);
            });
        }
    });
    console.log('Quill end disposed');
    // Set inner HTML to clean up the editor content
    el.innerHTML = quill.root.innerHTML;
}

// Ensure the Quill editor content is up-to-date when the form is submitted
window.syncQuillContent = function (dotnetHelper) {
    if (window.quillEditorInstance) {
        var emailBodyContent = window.quillEditorInstance.root.innerHTML;
        dotnetHelper.invokeMethodAsync('UpdateEmailBody', emailBodyContent)
            .then(function () {
                console.log('Quill content synced successfully.');
            })
            .catch(function (error) {
                console.error('Error syncing Quill content:', error);
            });
    }
};
