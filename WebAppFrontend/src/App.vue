<script setup>

import { onBeforeMount } from 'vue';
import { useLayout } from '@/layout/composables/layout';
import { usePrimeVue } from 'primevue/config';

const { setScale, layoutConfig } = useLayout();
const $primevue = usePrimeVue();
const darkTheme = Telegram.WebApp.colorScheme === 'dark';

onBeforeMount(() => {
    if (darkTheme)
        $primevue.changeTheme(
            layoutConfig.theme.value, 
            'aura-dark-green', 
            'theme-css', 
            () => {
                layoutConfig.theme.value = 'aura-dark-green';
                layoutConfig.darkTheme.value = darkTheme;
            });
})

</script>

<template>
    <router-view />
</template>

<style scoped></style>
