<script setup lang="ts">
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
</script>

<template>
  <div class="container mx-auto p-6">
    <h1 class="text-3xl font-bold mb-6">Dashboard</h1>
    <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
      <Card>
        <CardHeader>
          <CardTitle>My Builds</CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-muted-foreground">View and manage your PC builds</p>
          <Button class="mt-4" @click="$router.push('/builds')">
            Go to Builds
          </Button>
        </CardContent>
      </Card>
      
      <Card>
        <CardHeader>
          <CardTitle>Product Catalog</CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-muted-foreground">Browse available PC components</p>
          <Button class="mt-4" @click="$router.push('/catalog')">
            Browse Catalog
          </Button>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
