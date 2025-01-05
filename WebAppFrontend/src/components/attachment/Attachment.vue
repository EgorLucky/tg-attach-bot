<script setup>
    import { ref, onMounted, computed } from "vue";
    import moment from "moment";
    import FileService from "../../service/FileService";
    import localStorageService from '../../service/LocalSorageService';
    import { useToast } from 'primevue/usetoast';
    import { getFileUrl } from "../../service/FileUtils";
    import { AttachmentMode } from "./AttachmentMode";
    
    const toast = useToast();
    const props = defineProps({
        fileId: String,
        mode: AttachmentMode,
        file: Object,
        editClickHandler: Function
    })

    const userProfile = localStorageService.getUserInfo()

    const { fileId, mode } = props;

    const file = ref(null); 
    
    if (mode === AttachmentMode.Read) 
        file.value = props.file;

    const fileService = new FileService();

    const createdAt = computed(() => { 
        if (file) {
            const createdAtDate = moment(file.value?.createdAt);
            return createdAtDate.local().format('YYYY-MM-DD HH:mm');
        }
    });

    const fileUrl = computed(() => { 
        if (file) {
            return getFileUrl(file.value)
        }
    });

    onMounted(async () => {
        try {
            if (mode === AttachmentMode.Edit)
                file.value = await fileService.getFile(fileId);
        }
        catch (error){
            console.log(error);
            toast.add({ severity: 'error', summary: "Error getting file info!", detail: error.message, group: 'br' });
        }
    });

    const imageLoaded = ref(false);
    const loaded = () => {
        imageLoaded.value = true;
        console.log("content loaded handler ran")
    }

    const isSavingOrDeleting = ref(false)
    const handleSaveClick = async () => {
        isSavingOrDeleting.value = true;
        try {
            const updateResult = await fileService.update({
                id: file.value.id,
                name: file.value.name,
                keyWords: file.value.keyWords
            });
            
            toast.add({ severity: 'success', message: "Success!", detail: "Your data was saved!", life: 3000, group: 'br' });
        }
        catch (error) {
            console.log(error);
            toast.add({ severity: 'error', summary: "Error!", detail: "Something went wrong :( " + error.message, life: 3000, group: 'br' });
        }
        isSavingOrDeleting.value = false;
    }

    const deleteAttachmentDialog = ref(false);

    const handleDeleteClick = () =>  deleteAttachmentDialog.value = true;
    const deleteAttachment = async () =>
    {
        deleteAttachmentDialog.value = false;
        isSavingOrDeleting.value = true;
        try {
            const deleteResult = await fileService.delete(file.value.id);
            toast.add({ severity: 'info', summary: "Success!", detail: "Your data was deleted!", life: 3000, group: 'br' });
        }
        catch (error) {
            console.log(error);
            toast.add({ severity: 'error', summary: "Error!", detail: "Something went wrong :( " + error.message, life: 3000, group: 'br' });
        }
        isSavingOrDeleting.value = false;
    }
</script>

<template>
    <template v-if="file">
        <div class="field flex align-items-center" style="justify-content: space-between">           
            Added at {{ createdAt }}
            <template v-if="mode === AttachmentMode.Edit">
                <Button
                    v-if="!isSavingOrDeleting"
                    class="flex-end" 
                    icon="pi pi-trash" 
                    severity="danger" 
                    rounded 
                    aria-label="Delete"
                    @click="handleDeleteClick" 
                />
                <Button
                    v-else
                    class="flex-end" 
                    icon="pi pi-trash" 
                    severity="danger" 
                    rounded 
                    aria-label="Delete"
                >
                    <Skeleton shape="circle" size="2rem" class="mr-2"></Skeleton>
                </Button>
            </template>
            <template v-else>
                <Button
                    v-if="file.telegramUserId === userProfile.id"
                    class="flex-end" 
                    icon="pi pi-pencil" 
                    severity="secondary" 
                    rounded 
                    aria-label="Edit"
                    @click="props.editClickHandler" 
                />
            </template>
        </div>
        <!--content-->
        <div class="field">
            <div class="flex justify-content-center">
                <template v-if="file.fileType === 'Image' || file.fileType === 'Animation'">
                    <Image
                        :hidden="!imageLoaded"
                        :src="fileUrl" 
                        alt="Image" 
                        width="300px" 
                        preview
                        @load="loaded"
                    />
                </template>
                <template v-if="file.fileType === 'Video'">
                    <video
                        :hidden="!imageLoaded"
                        controls
                        :src="fileUrl"
                        width="300px"
                        @canplay="loaded"
                    >
                    </video>
                </template>
                <template v-if="file.fileType === 'Audio'">
                    <audio
                        :hidden="!imageLoaded"
                        controls
                        :src="fileUrl"
                        width="300px"
                        @canplay="loaded"
                    >
                    </audio>
                </template>
                <template v-if="file.fileType === 'Other'">
                    <a 
                        :href="fileUrl"
                        :download="file.name">
                        Download ({{ file.mimeType }})
                    </a>
                </template>
                <Skeleton 
                    v-if="!imageLoaded && file.fileType !== 'Other'"
                    width="300px" 
                    height="300px">
                </Skeleton>
            </div>
        </div>
        <!--/content-->
        <template v-if="mode === AttachmentMode.Edit">
            <div class="field">
                <label for="name">Name</label>
                <InputText  
                    v-model="file.name"
                    id="name" 
                    type="text"
                />
            </div>
            <div class="field">
                <label for="key-words">Key words</label>
                <Chips 
                    id="key-words" 
                    v-model="file.keyWords" 
                    separator=" "
                    :allowDuplicate="false"
                />
            </div>
            <Button
                v-if="!isSavingOrDeleting"
                label="Save"
                class="mt-2"
                @click="handleSaveClick"
            />
            <Button
                v-else
                class="mt-2"
            >
                <Skeleton height="1rem" class="mb-2"></Skeleton>
            </Button>
        </template>
        <template v-else>
            <div class="pt-1">
                <div class="flex flex-row justify-between items-start gap-2">
                    <div>
                        <div class="text-lg font-medium mt-1">{{ file.name }}</div>
                        <div class="flex flex-wrap gap-1 mt-2">
                            <Chip v-for="keyWord in file.keyWords" :key="keyWord" :label="keyWord" />
                        </div>
                    </div>
                </div>
            </div>
        </template>
    </template>
    <template v-else>
        <div class="field">
            <Skeleton height="2rem" class="mb-2"></Skeleton>
        </div>
        <div class="field">
            <div class="flex justify-content-center">
                <Skeleton width="250px" height="250px"></Skeleton>
            </div>
        </div>
        <div class="field">
            <Skeleton height="2rem" class="mb-2"></Skeleton>
            <Skeleton height="2rem" class="mb-2"></Skeleton>
        </div>
        <div class="field">
            <Skeleton height="2rem" class="mb-2"></Skeleton>
            <Skeleton height="2rem" class="mb-2"></Skeleton>
        </div>
    </template>
    <Dialog v-model:visible="deleteAttachmentDialog" :style="{ width: '450px' }" header="Confirm" :modal="true">
        <div class="flex align-items-center justify-content-center">
            <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem" />
            <span v-if="file">Are you sure you want to delete?</span>
        </div>
        <template #footer>
            <Button label="No" icon="pi pi-times" text @click="deleteAttachmentDialog = false" />
            <Button label="Yes" icon="pi pi-check" text @click="deleteAttachment" />
        </template>
    </Dialog>
    <Toast position="bottom-right" group="br" />
</template>
