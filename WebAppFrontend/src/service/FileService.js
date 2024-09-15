import BasicRestServiceClient from "./BasicRestServiceClient";

export default class FileService extends BasicRestServiceClient {
    async getFile(id) {
        return await this.sendRequest(
            `file/${id}`, 
            "get", null, {}, [200], true);
    }
    async update(fileUpdate) {
        return await this.sendRequest(
            `file`,
            'post',
            fileUpdate,
            {}, [200], true
        )
    }
    async delete(id) {
        return await this.sendRequest(
            `file/${id}`, 
            "delete", null, {}, [200], true);
    }
}
