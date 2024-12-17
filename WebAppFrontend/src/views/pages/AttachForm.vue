<script setup>
    import Attachment from "../../components/attachment/Attachment.vue";
    import { AttachmentMode } from "../../components/attachment/AttachmentMode";

    const fileId = Telegram.WebApp.initDataUnsafe.start_param;
</script>

<template>
    <Attachment 
        :fileId="fileId" 
        :mode="AttachmentMode.Edit"
    />
</template>
