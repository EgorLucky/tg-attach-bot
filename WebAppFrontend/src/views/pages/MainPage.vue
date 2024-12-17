<script setup>
import { onMounted, ref } from 'vue';
import FileService from '../../service/FileService';
import { useToast } from 'primevue/usetoast';
import localStorageService from '../../service/LocalSorageService';
import Attachment from '../../components/attachment/Attachment.vue';
import { AttachmentMode } from '../../components/attachment/AttachmentMode';

const userProfile = localStorageService.getUserInfo()

const fileService = new FileService();
const files = ref(null);
const searchTags = ref(null);
const userId = userProfile.id;
const source = ref('notTagged')

const toast = useToast();

onMounted(async () => {
    try {
        files.value = await fileService.list({
            userId: userId,
            keyWords: searchTags.value,
            take: 100,
            source: source.value
        });
    } catch (error) {
        console.log(error);
        toast.add({ 
            severity: 'error', 
            summary: 'Error getting file list!', 
            detail: error.message, 
            group: 'br' 
        });
    }
});
</script>
<template>
    <div class="grid">
        <div class="col-12">
            <div class="card">
                <Toolbar class="mb-4">
                    <!-- <template v-slot:start>
                        <div class="my-2">
                            <Button label="New" icon="pi pi-plus" class="mr-2" severity="success" @click="" />
                            <Button label="Delete" icon="pi pi-trash" severity="danger" @click="" :disabled="!selectedProducts || !selectedProducts.length" />
                        </div>
                    </template>

                    <template v-slot:end>
                        <FileUpload mode="basic" accept="image/*" :maxFileSize="1000000" label="Import" chooseLabel="Import" class="mr-2 inline-block" />
                        <Button label="Export" icon="pi pi-upload" severity="help" @click="exportCSV($event)" />
                    </template> -->
                </Toolbar>

                <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
                    <h5>Manage attachments</h5>
                    <InputGroup>
                        <InputGroupAddon>
                            <i class="pi pi-search"></i>
                        </InputGroupAddon>
                        <Chips 
                            id="key-words" 
                            class="w-full sm:w-auto" 
                            placeholder="Search..." 
                            v-model="searchTags" 
                            separator=" " 
                            :allowDuplicate="false" 
                        />
                    </InputGroup>
                </div>
                <DataView
                    :value="files" 
                    layout="grid">
                    <template #grid="slotProps">
                        <div class="grid grid-cols-12 gap-4">
                            <Attachment 
                                v-for="(item, index) in slotProps.items"
                                :fileId="item.id"
                                :mode="AttachmentMode.Read"
                                :file="item"
                            />
                            <!-- <div 
                                v-for="(item, index) in slotProps.items" 
                                :key="index" 
                                class="col-span-12 sm:col-span-6 lg:col-span-4 p-2"
                            >
                                <div class="p-6 border border-surface-200 dark:border-surface-700 bg-surface-0 dark:bg-surface-900 rounded flex flex-col">
                                    <div class="bg-surface-50 flex justify-center rounded p-4">
                                        <div class="relative mx-auto">
                                            <img 
                                                class="rounded w-full" 
                                                :src="getFileUrl(item)" 
                                                :alt="item.name" 
                                                style="max-width: 300px" 
                                            />
                                        </div>
                                    </div>
                                    <div class="pt-6">
                                        <div class="flex flex-row justify-between items-start gap-2">
                                            <div>
                                                <span class="font-medium text-surface-500 dark:text-surface-400 text-sm">{{ item.category }}</span>
                                                <div class="text-lg font-medium mt-1">{{ item.name }}</div>
                                            </div>
                                            <div class="bg-surface-100 p-1" style="border-radius: 30px">
                                                <div
                                                    class="bg-surface-0 flex items-center gap-2 justify-center py-1 px-2"
                                                    style="
                                                        border-radius: 30px;
                                                        box-shadow:
                                                            0px 1px 2px 0px rgba(0, 0, 0, 0.04),
                                                            0px 1px 2px 0px rgba(0, 0, 0, 0.06);
                                                    "
                                                >
                                                    <span class="text-surface-900 font-medium text-sm">{{ item.rating }}</span>
                                                    <i class="pi pi-star-fill text-yellow-500"></i>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="flex flex-col gap-6 mt-6">
                                            <span class="text-2xl font-semibold">${{ item.price }}</span>
                                            <div class="flex gap-2">
                                                <Button icon="pi pi-shopping-cart" label="Buy Now" :disabled="item?.inventoryStatus === 'OUTOFSTOCK'" class="flex-auto whitespace-nowrap"></Button>
                                                <Button icon="pi pi-heart" outlined></Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div> -->
                        </div>
                    </template>
                </DataView>
            </div>
        </div>
    </div>
    <Toast position="bottom-right" group="br" />
</template>
