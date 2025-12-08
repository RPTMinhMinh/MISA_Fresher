<template>
    <div v-if="visible" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[9999] p-4"
        @click.self="handleClose">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-md overflow-hidden">
            <!-- Header -->
            <div class="flex items-center gap-4 px-6 pt-10 pb-7">
                <!-- <h3 class="text-lg font-semibold text-gray-900">{{ title }}</h3> -->
                <div class="icon-warning"></div>
                <div class="mt-2 text-gray-600">{{ message }}</div>
            </div>

            <!-- Footer với 2 nút -->
            <div class="px-6 py-4  flex justify-end gap-3">
                <MsButton variant="outline" @click="handleCancel" class="px-4 py-2">
                    {{ cancelText }}
                </MsButton>
                <MsButton variant="main" @click="handleConfirm" class="px-4 py-2">
                    {{ confirmText }}
                </MsButton>
            </div>
        </div>
    </div>
</template>

<script setup>
import MsButton from '@/components/MsButton.vue'

const props = defineProps({
    visible: {
        type: Boolean,
        default: false
    },
    title: {
        type: String,
        default: 'Xác nhận'
    },
    message: {
        type: String,
        default: 'Bạn có chắc chắn muốn thực hiện hành động này?'
    },
    cancelText: {
        type: String,
        default: 'Hủy'
    },
    confirmText: {
        type: String,
        default: 'Xóa'
    }
})

const emit = defineEmits(['confirm', 'cancel', 'update:visible'])

const handleConfirm = () => {
    emit('confirm')
    emit('update:visible', false)
}

const handleCancel = () => {
    emit('cancel')
    emit('update:visible', false)
}

const handleClose = () => {
    emit('cancel')
    emit('update:visible', false)
}
</script>

<style scoped>
/* Animation cho popup */
.v-enter-active,
.v-leave-active {
    transition: opacity 0.2s ease;
}

.v-enter-from,
.v-leave-to {
    opacity: 0;
}
</style>