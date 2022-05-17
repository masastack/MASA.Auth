export function InputFileChanged(inputFile, callback) {
    return eval(`${callback}(inputFile.files)`)
}

export function InputFileUpload(inputFile, callback) {
    return eval(`${callback}(inputFile.files)`)
}

export async function GetPreviewImageUrls(imageFiles) {
    const imageUrls = [];
    for (var imageFile of imageFiles) {
        const arrayBuffer = await imageFile.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);
        imageUrls.push(url);
    }
    return imageUrls;
}
