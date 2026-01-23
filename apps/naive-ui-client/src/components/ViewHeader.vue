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
                    <n-icon>
                        <component :is="getIconComponent(actionButton.icon)" />
                    </n-icon>
                </template>
                {{ actionButton.text }}
            </n-button>
        </n-flex>

        <!-- Optional slot for additional header content (e.g., search bars, filters) -->
        <slot></slot>
    </div>
</template>

<script setup lang="ts">
import type { Component } from 'vue';
import { NButton, NFlex, NIcon } from 'naive-ui';
import { Icons } from '@/utils/icons';

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

function getIconComponent(iconName: string): Component {
    const iconMap: Record<string, Component> = {
        'mdi-plus': Icons.Add,
        'mdi-arrow-right': Icons.ArrowForward,
        'mdi-arrow-left': Icons.ArrowBack,
        'mdi-delete': Icons.Trash,
        'mdi-pencil': Icons.Edit,
        'mdi-check': Icons.Check,
        'mdi-close': Icons.Close,
        'mdi-magnify': Icons.Search
    };
    return iconMap[iconName] || Icons.Info;
}
</script>

<style scoped></style>
