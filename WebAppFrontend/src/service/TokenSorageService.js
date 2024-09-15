const tokenStorageService = {
    getTokenFromStorage: () => localStorage.getItem('access_token'),
    saveTokenToStorage: (token) => localStorage.setItem("access_token", token)
}

export default tokenStorageService;