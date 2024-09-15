import BasicRestServiceClient from "./BasicRestServiceClient";

export default class UserService extends BasicRestServiceClient {
    async getUserProfile() {
        return await this.sendRequest(
            "user/current", 
            "get", null, {}, [200], true);
    }
}
