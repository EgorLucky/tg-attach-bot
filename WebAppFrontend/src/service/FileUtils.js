const getFileUrl = (file) => {
    return import.meta.env.VITE_BACKEND_API_URL + '/file/download/' 
            + encodeURIComponent(file.filePath) + '?isImage=' 
            + (file.fileType === 'Animation' || file.fileType === 'Image')
}

export { getFileUrl };