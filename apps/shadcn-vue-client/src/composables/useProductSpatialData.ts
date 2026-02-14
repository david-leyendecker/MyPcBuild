import type { Ref } from 'vue'
import { computed } from 'vue'
import type { CategoryFormData, FormDataWithSpatial, Dimensions, Slot, Chamber } from '@/types/product'

function hasValidDimensions(dimensions: unknown): dimensions is Dimensions {
  if (!dimensions || typeof dimensions !== 'object') return false
  const d = dimensions as Record<string, unknown>
  return (
    typeof d.length === 'number' ||
    typeof d.width === 'number' ||
    typeof d.height === 'number'
  )
}

function isValidSlots(slots: unknown): slots is Slot[] {
  return Array.isArray(slots) && slots.length > 0
}

function isValidChambers(chambers: unknown): chambers is Chamber[] {
  return Array.isArray(chambers) && chambers.length > 0
}

/**
 * Composable for extracting and validating spatial data from category form data.
 * Used by ProductCreateView and ProductEditView for the 3D preview.
 */
export function useProductSpatialData(formDataRef: Ref<CategoryFormData | Record<string, unknown>>) {
  const hasSpatialData = computed(() => {
    const data = formDataRef.value as FormDataWithSpatial | undefined
    if (!data) return false
    return (
      hasValidDimensions(data.dimensions) ||
      isValidSlots(data.slots) ||
      isValidChambers(data.chambers)
    )
  })

  const dimensions = computed<Dimensions | undefined>(() => {
    const data = formDataRef.value as FormDataWithSpatial | undefined
    return data?.dimensions && hasValidDimensions(data.dimensions) ? data.dimensions : undefined
  })

  const slots = computed<Slot[]>(() => {
    const data = formDataRef.value as FormDataWithSpatial | undefined
    return data?.slots && isValidSlots(data.slots) ? data.slots : []
  })

  const chambers = computed<Chamber[]>(() => {
    const data = formDataRef.value as FormDataWithSpatial | undefined
    return data?.chambers && isValidChambers(data.chambers) ? data.chambers : []
  })

  return {
    hasSpatialData,
    dimensions,
    slots,
    chambers
  }
}
