import axios from "axios";
import tokenStorageService from "./TokenSorageService";

class BasicRestServiceClient {
    static instance = axios.create({
        baseURL: import.meta.env.VITE_BACKEND_API_URL,
        headers: { 'Content-Type': 'application/json' }
    });

    async sendRequest(
        path,
        method,
        body,
        headers = {},
        awaitableStatusCodes = [200],
        useAuthorization = false
    ) {
        if (useAuthorization) {
            const token = tokenStorageService.getTokenFromStorage();
            if (!token) {
                await this.signInByTelegramWebAppData()
            }

            headers["Authorization"] = "TelegramWebAppData " + tokenStorageService.getTokenFromStorage()
        }

        const requestData = {
            url: path,
            method: method,
            data: body,
            headers: headers
        }

        let response = await this.getResponse(requestData);

        if(useAuthorization && response.status === 401) {
            this.signInByTelegramWebAppData();
            requestData.headers["Authorization"] = "TelegramWebAppData " + tokenStorageService.getTokenFromStorage();
            response = await this.getResponse(requestData);
        }

        if (!awaitableStatusCodes.includes(response.status)) {
            const text = response.data;
            throw new Error(text);
        }

        return response.data;
    }

    async getResponse(requestData) {
        let response = null;

        try {
            response = await BasicRestServiceClient.instance.request(requestData);
        }
        catch(error){
            response = error.response;
        }

        return response;
    }

    async signInByTelegramWebAppData() {
        const telegramWebAppInitData = Telegram.WebApp.initData;
        if (!telegramWebAppInitData) {
            // show not from telegram
            throw new Error("telegramWebInitData undefined")
        }

        const getTokenResult = await this.sendRequest(
            "bot/getToken", 
            "post", 
            telegramWebAppInitData, 
            {
                'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8'
            }, 
            [200]);
        
        if (getTokenResult?.access_token) {
            tokenStorageService.saveTokenToStorage(getTokenResult.access_token)
        }
        else {
            throw new Error("authentication failed");
        }
    }
}

export default BasicRestServiceClient; 