<script setup lang="ts">
import { AlertCircle, AlertTriangle } from 'lucide-vue-next'
import type { CompatibilityIssue } from '@/types/build'
import { Card, CardContent } from '@/components/ui/card'
import { cn } from '@/lib/utils'

interface Props {
  issue: CompatibilityIssue
}

const props = defineProps<Props>()

const iconComponent = props.issue.severity === 'Error' ? AlertCircle : AlertTriangle
const severityClass = props.issue.severity === 'Error' ? 'text-destructive' : 'text-yellow-500'
</script>

<template>
  <Card>
    <CardContent class="p-4">
      <div class="flex items-start gap-3">
        <component :is="iconComponent" :class="cn('h-5 w-5 mt-0.5', severityClass)" />
        <div class="flex-1">
          <div class="flex items-center gap-2 mb-1">
            <span :class="cn('text-sm font-semibold', severityClass)">
              {{ issue.severity }}
            </span>
            <span class="text-xs text-muted-foreground">{{ issue.category }}</span>
          </div>
          <p class="text-sm text-foreground">{{ issue.message }}</p>
        </div>
      </div>
    </CardContent>
  </Card>
</template>
