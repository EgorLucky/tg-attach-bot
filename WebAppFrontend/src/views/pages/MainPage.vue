<script setup>
import { computed, nextTick, onMounted, ref, watch } from 'vue';
import FileService from '../../service/FileService';
import { useToast } from 'primevue/usetoast';
import localStorageService from '../../service/LocalSorageService';
import Attachment from '../../components/attachment/Attachment.vue';
import { AttachmentMode } from '../../components/attachment/AttachmentMode';
import KeyWordInput from '../../components/keywordinput/KeyWordInput.vue';
import ProgressSpinner from 'primevue/progressspinner';

const toast = useToast();
const userProfile = localStorageService.getUserInfo();

const fileService = new FileService();
const files = ref([]);
const searchTags = ref([]);
const userId = userProfile.id;
const editAttachmentModalVisible = ref(false);
const fileToEditId = ref(null);
const tabs = ref([
  {
    tabId: 'tagged',
    title: 'Tagged'
  },
  {
    tabId: 'notTagged',
    title: 'Not tagged'
  }
]);
const selectedTabIndex = ref(0);
const loading = ref(false);
const endOfListLoaded = ref(false)

const getFileList = async () => {
  try {
    loading.value = true;

    const offset = files.value.at(files.value.length - 1)?.createdAt
    const excludedFileIds = offset
      ? files.value.filter(f => f.createdAt === offset).map(f => f.id)
      : null

    const loadedFiles = await fileService.list({
      userId: userId,
      keyWords: searchTags.value,
      take: 100,
      source: tabs.value[selectedTabIndex.value].tabId,
      offset: offset,
      offsetExcludedFileIds: excludedFileIds
    });
    
    if (loadedFiles.length) {
      const oldFiles = files.value
      oldFiles.push(...loadedFiles)
      await nextTick();
      files.value = oldFiles;
    } else {
      endOfListLoaded.value = true;
    }
  } catch (error) {
    console.log(error);
    toast.add({
      severity: 'error',
      summary: 'Error getting file list!',
      detail: error.message,
      group: 'br'
    });
  }
  loading.value = false;
};

watch(selectedTabIndex, async (newIndex, oldIndex) => {
  files.value = [];
  endOfListLoaded.value = false
  await getFileList();
});

onMounted(async () => await getFileList());

const handleEditClick = (file) => {
  fileToEditId.value = file.id;
  editAttachmentModalVisible.value = true;
};

const handleSearchClick = () => {
  files.value = []
  endOfListLoaded.value = false
  getFileList()
}

const handleScroll = (e) => {
  const {scrollTop, scrollHeight, offsetHeight} = e.target;

  if(scrollTop + offsetHeight < scrollHeight - 10) {
    return;
  }

  if (!loading.value) {
    getFileList()
  }
}
</script>
<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">
        <TabView 
					v-model:activeIndex="selectedTabIndex"
				>
          <TabPanel 
						v-for="tab in tabs" 
						:key="tab.tabId" 
						:header="tab.title"
					>
            <!--<Toolbar v-if="tab.tabId === 'tagged'" class="mb-4">
                             <template v-slot:start>
                                <div class="my-2">
                                    <Button label="New" icon="pi pi-plus" class="mr-2" severity="success" @click="" />
                                    <Button label="Delete" icon="pi pi-trash" severity="danger" @click="" :disabled="!selectedProducts || !selectedProducts.length" />
                                </div>
                            </template>

                            <template v-slot:end>
                                <FileUpload mode="basic" accept="image/*" :maxFileSize="1000000" label="Import" chooseLabel="Import" class="mr-2 inline-block" />
                                <Button label="Export" icon="pi pi-upload" severity="help" @click="exportCSV($event)" />
                            </template>
                        </Toolbar>-->
            <h5>Manage attachments</h5>
            <div 
							v-if="tab.tabId === 'tagged'" 
							class="flex flex-column md:flex-row md:justify-content-between md:align-items-center mb-6"
						>
              <KeyWordInput 
								v-model="searchTags" 
								placeholder="Search..." 
							/>
              <Button 
								type="button" 
								label="Search" 
								icon="pi pi-search" 
								iconPos="right" 
								:loading="loading" 
								@click="handleSearchClick" 
							/>
            </div>
            <div 
              class="h-30rem grid grid-cols-12 gap-4 overflow-y-auto"
              style="width:122%"
              @scroll="handleScroll"
            >
              <div 
                v-for="(item, index) in files" 
                :key="index" 
                class="col-span-12 sm:col-span-6 lg:col-span-4"
              >
                <div class="border border-surface-200 dark:border-surface-700 bg-surface-0 dark:bg-surface-900 rounded">
                  <Attachment 
                    :fileId="item.id" 
                    :mode="AttachmentMode.Read" 
                    :file="item" :editClickHandler="() => handleEditClick(item)" 
                  />
                </div>
              </div>
              <ProgressSpinner
                v-if="loading && !endOfListLoaded"
                style="width: 50px; height: 50px" 
                strokeWidth="8" 
                fill="var(--surface-ground)"
                animationDuration=".5s" 
                aria-label="Custom ProgressSpinner" 
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
          </TabPanel>
        </TabView>
      </div>
    </div>
  </div>
  <Toast position="bottom-right" group="br" />
  <Dialog 
		v-if="editAttachmentModalVisible" 
		v-model:visible="editAttachmentModalVisible" 
		modal 
		header="Edit attachment" 
		:style="{ width: '25rem' }"
	>
    <div className="p-fluid">
      <Attachment 
				:fileId="fileToEditId" 
				:mode="AttachmentMode.Edit"	
			/>
    </div>
  </Dialog>
</template>
