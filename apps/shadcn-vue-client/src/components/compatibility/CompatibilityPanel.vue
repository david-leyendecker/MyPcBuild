<script setup lang="ts">
import { computed } from 'vue'
import type { CompatibilityIssue } from '@/types/build'
import IssueCard from './IssueCard.vue'

interface Props {
  issues: CompatibilityIssue[]
}

const props = defineProps<Props>()

const errors = computed(() => props.issues.filter(i => i.severity === 'Error'))
const warnings = computed(() => props.issues.filter(i => i.severity === 'Warning'))
</script>

<template>
  <div class="space-y-4">
    <div v-if="errors.length > 0">
      <h3 class="text-lg font-semibold text-destructive mb-3">
        Errors ({{ errors.length }})
      </h3>
      <div class="space-y-2">
        <IssueCard v-for="(issue, idx) in errors" :key="`error-${idx}-${issue.message}`" :issue="issue" />
      </div>
    </div>

    <div v-if="warnings.length > 0">
      <h3 class="text-lg font-semibold text-yellow-500 mb-3">
        Warnings ({{ warnings.length }})
      </h3>
      <div class="space-y-2">
        <IssueCard v-for="(issue, idx) in warnings" :key="`warn-${idx}-${issue.message}`" :issue="issue" />
      </div>
    </div>

    <div v-if="issues.length === 0" class="text-center py-8 text-muted-foreground">
      <p>No compatibility issues found. Your build looks good!</p>
    </div>
  </div>
</template>
