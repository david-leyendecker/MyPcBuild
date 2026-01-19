<template>
    <div style="margin-bottom: 16px;">
        <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
            <h2 style="font-size: 28px; font-weight: 600; color: var(--n-text-color);">{{ title }}</h2>
            <n-button 
                v-if="actionButton" 
                :type="actionButton.color === 'primary' ? 'primary' : 'default'"
                @click="actionButton.onClick"
            >
                <template v-if="actionButton.icon" #icon>
                    <span>{{ getIcon(actionButton.icon) }}</span>
                </template>
                {{ actionButton.text }}
            </n-button>
        </n-flex>

        <!-- Optional slot for additional header content (e.g., search bars, filters) -->
        <slot></slot>
    </div>
</template>

<script setup lang="ts">
import { NButton, NFlex } from 'naive-ui';

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

function getIcon(iconName: string): string {
    const iconMap: Record<string, string> = {
        'mdi-plus': '+',
        'mdi-arrow-right': '→',
        'mdi-arrow-left': '←',
        'mdi-delete': '🗑',
        'mdi-pencil': '✏',
        'mdi-check': '✓',
        'mdi-close': '✕',
        'mdi-magnify': '🔍'
    };
    return iconMap[iconName] || '';
}
</script>

<style scoped></style>
