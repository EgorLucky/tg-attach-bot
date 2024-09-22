const localStorageService = {
    /*
     * ACCESS TOKEN
     */
    getAccessToken: () => localStorage.getItem('access_token'),
    saveAccessToken: (token) => localStorage.setItem("access_token", token),
    /*
     * USER INFO
     */
    saveUserInfo: (userInfo) => {
        const userInfoJson = JSON.stringify(userInfo);
        localStorage.setItem("user_info", userInfoJson);
    },
    getUserInfo: () => {
        const userInfoJson = localStorage.getItem("user_info");
        return JSON.parse(userInfoJson);
    }
}

export default localStorageService;