<script setup lang="ts">
import { computed, ref } from 'vue';

const props = defineProps<{
  placeholder: string | null;
}>();

const model = defineModel<string[]>({ required: true });

const keyWords = computed(() => model.value);

const { placeholder } = props;
const inputText = ref<string>();

const keyWordSeparators = [',', '.', ':', ';', ' ', 'Enter'];
const handleKeyWordFilterInputKeyDown = (event: KeyboardEvent) => {
  const separatorIndex = keyWordSeparators.indexOf(event.key);
  if (separatorIndex === -1) {
    return;
  }
  if (inputText.value) {
    const processedInputText = inputText.value.trim()
      .replaceAll(',', '')
      .replaceAll('.', '')
      .replaceAll(':', '')
      .replaceAll(';', '')
      .replaceAll('\n', '')
      .replaceAll('\r', '');
    if (processedInputText 
      && !keyWords.value.includes(processedInputText) 
      && processedInputText.length > 2) {
      keyWords.value.push(processedInputText);
      model.value = keyWords.value;
    }
  }
  inputText.value = '';
};

const handleKeyWordFilterInputKeyUp = (event: KeyboardEvent) => {
  const separatorIndex = keyWordSeparators.indexOf(event.key);
  if (separatorIndex === -1) {
    return;
  }

  inputText.value = '';
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
    @keydown="handleKeyWordFilterInputKeyDown" 
    @keyup="handleKeyWordFilterInputKeyUp" 
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
