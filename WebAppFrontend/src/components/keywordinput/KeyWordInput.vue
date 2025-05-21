<script setup lang="ts">
import { computed, ref, nextTick } from 'vue';
import InputText from 'primevue/inputtext'

const props = defineProps<{
  placeholder: string | undefined;
}>();

const model = defineModel<string[]>({ required: true });

const keyWords = computed(() => model.value ?? []);

const { placeholder } = props;
const inputText = ref<string>();

const keyWordSeparators = [',', '.', '…', ':', ';', ' ', '\r', '\n'];

const moveKeyWordFromInput = (keyWordString: string) => {
  if (keyWordString 
    && !keyWords.value.includes(keyWordString) 
    && keyWordString.length > 2) {
    keyWords.value.push(keyWordString);
    model.value = keyWords.value;
  }
}

const handleKeyWordFilterInputKeyUp = (event: KeyboardEvent) => {
  if (event.code !== 'Enter' && event.key !== 'Enter')
    return

  moveKeyWordFromInput(inputText.value!)

  nextTick(() => inputText.value = '')
};

const handleInputTextModelUpdated = (value: string|undefined) => {
  if (!value)
    return
  
  const data = value
  const separators = keyWordSeparators.filter(separator => data?.includes(separator))

  if (!separators.length)
    return
  if (inputText.value) {
    const processedInputText = separators
      .reduce((prev, curr) => prev.replaceAll(curr, ''), 
        inputText.value.trim())

    moveKeyWordFromInput(processedInputText)
  }

  nextTick(() => inputText.value = '')
};


const handleChipRemove = (index: number) => {
  const filteredWords = keyWords.value.filter((s, i) => i !== index);
  model.value = filteredWords;
};

const chipDragStart = (evt: DragEvent, index: number) => {
  const dataTransfer = evt.dataTransfer;
  if (!dataTransfer) return;
  dataTransfer.dropEffect = 'move';
  dataTransfer.effectAllowed = 'move';
  dataTransfer.setData('index', index.toString());
};

const onChipDrop = (evt: DragEvent, indexToDrop: number) => {
  const dataTransfer = evt.dataTransfer;
  if (!dataTransfer) return;
  const draggedIndex = parseInt(dataTransfer.getData('index'));

  if (draggedIndex === indexToDrop) return;

  const draggableElem = keyWords.value[draggedIndex];

  if (draggedIndex > indexToDrop) {
    for (let i = draggedIndex - 1; i >= indexToDrop; i--) 
      keyWords.value[i + 1] = keyWords.value[i];
  } else {
    for (let i = draggedIndex + 1; i <= indexToDrop; i++) 
      keyWords.value[i - 1] = keyWords.value[i];
  }

  keyWords.value[indexToDrop] = draggableElem;
};
</script>

<template>
  <InputText
    v-model="inputText" 
    type="text"
    @keypress.enter="handleKeyWordFilterInputKeyUp"
    @update:model-value="handleInputTextModelUpdated"
    :placeholder="placeholder" 
  />
  <div class="flex flex-wrap gap-1 mt-2">
    <Chip
      v-for="(keyWord, index) in keyWords"
      :key="`${index}_${keyWord}`"
      :label="keyWord"
      removable
      draggable="true"
      @remove="() => handleChipRemove(index)"
      @dragstart="chipDragStart($event, index)"
      @drop="onChipDrop($event, index)"
      @dragover.prevent
      @dragenter.prevent
    />
  </div>
</template>
