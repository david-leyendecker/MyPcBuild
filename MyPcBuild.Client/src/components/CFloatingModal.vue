<template>
    <NModal
        :show="props.show"
        preset="card"
        :title="props.title"
        :style="modalStyle"
        :mask-closable="props.maskClosable"
        :segmented="{ content: true, footer: 'soft' }"
        :draggable="!hasCustomPosition"
        :show-mask="false"
        @mousedown="hasCustomPosition ? onMouseDown($event) : undefined"
        @update:show="emit('update:show', $event)">
        <slot />
        <template v-if="$slots.footer" #footer><slot name="footer" /></template>
        <template v-if="$slots.header" #header><slot name="header" /></template>

        <!-- Resize handles -->
        <template v-if="props.resizable && hasCustomPosition">
            <div class="resize-handle resize-handle-n" @mousedown.stop="onResizeStart($event, 'n')"></div>
            <div class="resize-handle resize-handle-s" @mousedown.stop="onResizeStart($event, 's')"></div>
            <div class="resize-handle resize-handle-e" @mousedown.stop="onResizeStart($event, 'e')"></div>
            <div class="resize-handle resize-handle-w" @mousedown.stop="onResizeStart($event, 'w')"></div>
            <div class="resize-handle resize-handle-ne" @mousedown.stop="onResizeStart($event, 'ne')"></div>
            <div class="resize-handle resize-handle-nw" @mousedown.stop="onResizeStart($event, 'nw')"></div>
            <div class="resize-handle resize-handle-se" @mousedown.stop="onResizeStart($event, 'se')"></div>
            <div class="resize-handle resize-handle-sw" @mousedown.stop="onResizeStart($event, 'sw')"></div>
        </template>
    </NModal>
</template>

<style>
.n-modal .n-card-header {
    cursor: move !important;
    user-select: none;
}

.resize-handle {
    position: absolute;
    z-index: 10;
}

.resize-handle-n,
.resize-handle-s {
    left: 8px;
    right: 8px;
    height: 8px;
    cursor: ns-resize;
}

.resize-handle-n { top: -4px; }
.resize-handle-s { bottom: -4px; }

.resize-handle-e,
.resize-handle-w {
    top: 8px;
    bottom: 8px;
    width: 8px;
    cursor: ew-resize;
}

.resize-handle-e { right: -4px; }
.resize-handle-w { left: -4px; }

.resize-handle-ne,
.resize-handle-nw,
.resize-handle-se,
.resize-handle-sw {
    width: 12px;
    height: 12px;
}

.resize-handle-ne {
    top: -4px;
    right: -4px;
    cursor: nesw-resize;
}

.resize-handle-nw {
    top: -4px;
    left: -4px;
    cursor: nwse-resize;
}

.resize-handle-se {
    bottom: -4px;
    right: -4px;
    cursor: nwse-resize;
}

.resize-handle-sw {
    bottom: -4px;
    left: -4px;
    cursor: nesw-resize;
}
</style>

<script setup lang="ts">
import { computed, ref, watch, onUnmounted, type CSSProperties } from 'vue'
import { NModal } from 'naive-ui'

interface Props {
    show: boolean
    title?: string
    width?: string | number
    height?: string | number
    maskClosable?: boolean
    resizable?: boolean
    minWidth?: number
    minHeight?: number
    position?: { top?: number; left?: number; bottom?: number; right?: number }
}

const props = withDefaults(defineProps<Props>(), {
    title: '',
    width: 500,
    maskClosable: false,
    resizable: true,
    minWidth: 300,
    minHeight: 200
})

const emit = defineEmits<{ 'update:show': [value: boolean] }>()

const dragStart = ref<{ x: number; y: number } | null>(null)
const draggedPosition = ref<{ top: number; left: number } | null>(null)
const resizedDimensions = ref<{ width: number; height: number } | null>(null)
const resizeState = ref<{
    direction: string;
    startX: number;
    startY: number;
    startWidth: number;
    startHeight: number;
    startTop: number;
    startLeft: number;
} | null>(null)

const hasCustomPosition = computed(() =>
    props.position && Object.values(props.position).some(v => v !== undefined)
)

// Reset position and size when modal reopens
watch(() => props.show, (show) => {
    if (show) {
        draggedPosition.value = null
        resizedDimensions.value = null
    }
})

const modalStyle = computed<CSSProperties>(() => {
    const px = (v?: number) => v !== undefined ? `${v}px` : undefined
    const style: CSSProperties = {
        width: resizedDimensions.value
            ? `${resizedDimensions.value.width}px`
            : typeof props.width === 'number' ? `${props.width}px` : props.width
    }

    if (resizedDimensions.value?.height !== undefined) {
        style.height = `${resizedDimensions.value.height}px`
    } else if (props.height !== undefined) {
        style.height = typeof props.height === 'number' ? `${props.height}px` : props.height
    }

    if (!hasCustomPosition.value) return style

    Object.assign(style, { position: 'fixed', margin: 0, transform: 'none' })

    if (draggedPosition.value) {
        style.top = px(draggedPosition.value.top)
        style.left = px(draggedPosition.value.left)
    } else {
        style.top = px(props.position?.top)
        style.left = px(props.position?.left)
        style.bottom = px(props.position?.bottom)
        style.right = px(props.position?.right)
    }

    return style
})

const modalSize = ref<{ width: number; height: number }>({ width: 0, height: 0 })

const onMouseDown = (e: MouseEvent) => {
    const header = (e.target as HTMLElement).closest('.n-card-header')
    if (!header) return

    const modal = header.closest('.n-modal') as HTMLElement
    if (!modal) return

    e.preventDefault()
    const rect = modal.getBoundingClientRect()
    dragStart.value = { x: e.clientX - rect.left, y: e.clientY - rect.top }
    modalSize.value = { width: rect.width, height: rect.height }
    draggedPosition.value ??= { top: rect.top, left: rect.left }

    document.addEventListener('mousemove', onMouseMove)
    document.addEventListener('mouseup', onMouseUp)
}

const onMouseMove = (e: MouseEvent) => {
    if (!dragStart.value) return

    const newLeft = e.clientX - dragStart.value.x
    const newTop = e.clientY - dragStart.value.y

    // Constrain to viewport boundaries
    const minLeft = 0
    const minTop = 0
    const maxLeft = window.innerWidth - modalSize.value.width
    const maxTop = window.innerHeight - modalSize.value.height

    draggedPosition.value = {
        left: Math.max(minLeft, Math.min(maxLeft, newLeft)),
        top: Math.max(minTop, Math.min(maxTop, newTop))
    }
}

const onMouseUp = () => {
    dragStart.value = null
    document.removeEventListener('mousemove', onMouseMove)
    document.removeEventListener('mouseup', onMouseUp)
}

const onResizeStart = (e: MouseEvent, direction: string) => {
    const modal = (e.target as HTMLElement).closest('.n-modal') as HTMLElement
    if (!modal) return

    e.preventDefault()
    const rect = modal.getBoundingClientRect()

    resizeState.value = {
        direction,
        startX: e.clientX,
        startY: e.clientY,
        startWidth: rect.width,
        startHeight: rect.height,
        startTop: rect.top,
        startLeft: rect.left
    }

    resizedDimensions.value ??= { width: rect.width, height: rect.height }
    draggedPosition.value ??= { top: rect.top, left: rect.left }

    document.addEventListener('mousemove', onResizeMove)
    document.addEventListener('mouseup', onResizeEnd)
}

const onResizeMove = (e: MouseEvent) => {
    if (!resizeState.value || !resizedDimensions.value || !draggedPosition.value) return

    const { direction, startX, startY, startWidth, startHeight, startTop, startLeft } = resizeState.value
    const deltaX = e.clientX - startX
    const deltaY = e.clientY - startY

    let newWidth = startWidth
    let newHeight = startHeight
    let newTop = startTop
    let newLeft = startLeft

    // Handle horizontal resize
    if (direction.includes('e')) {
        newWidth = Math.max(props.minWidth, startWidth + deltaX)
    } else if (direction.includes('w')) {
        const potentialWidth = startWidth - deltaX
        if (potentialWidth >= props.minWidth) {
            newWidth = potentialWidth
            newLeft = startLeft + deltaX
        }
    }

    // Handle vertical resize
    if (direction.includes('s')) {
        newHeight = Math.max(props.minHeight, startHeight + deltaY)
    } else if (direction.includes('n')) {
        const potentialHeight = startHeight - deltaY
        if (potentialHeight >= props.minHeight) {
            newHeight = potentialHeight
            newTop = startTop + deltaY
        }
    }

    // Constrain to viewport
    if (newLeft < 0) {
        newWidth += newLeft
        newLeft = 0
    }
    if (newTop < 0) {
        newHeight += newTop
        newTop = 0
    }
    if (newLeft + newWidth > window.innerWidth) {
        newWidth = window.innerWidth - newLeft
    }
    if (newTop + newHeight > window.innerHeight) {
        newHeight = window.innerHeight - newTop
    }

    resizedDimensions.value = { width: newWidth, height: newHeight }
    draggedPosition.value = { top: newTop, left: newLeft }
}

const onResizeEnd = () => {
    resizeState.value = null
    document.removeEventListener('mousemove', onResizeMove)
    document.removeEventListener('mouseup', onResizeEnd)
}

onUnmounted(() => {
    document.removeEventListener('mousemove', onMouseMove)
    document.removeEventListener('mouseup', onMouseUp)
    document.removeEventListener('mousemove', onResizeMove)
    document.removeEventListener('mouseup', onResizeEnd)
})
</script>
