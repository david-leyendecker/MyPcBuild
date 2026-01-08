<template>
    <div class="mb-4">
        <div class="d-flex justify-space-between align-center mb-3">
            <h2 class="text-h4 text-primary">{{ title }}</h2>
            <v-btn v-if="actionButton" :prepend-icon="actionButton.icon" :color="actionButton.color || 'primary'"
                :rounded="actionButton.rounded || true" @click="actionButton.onClick">
                {{ actionButton.text }}
            </v-btn>
        </div>

        <!-- Optional slot for additional header content (e.g., search bars, filters) -->
        <slot></slot>
    </div>
</template>

<script setup lang="ts">
interface ActionButton {
    text: string;
    icon?: string;
    color?: string;
    rounded?: boolean;
    onClick: () => void;
}

interface Props {
    title: string;
    actionButton?: ActionButton;
}

defineProps<Props>();
</script>

<style scoped></style>
