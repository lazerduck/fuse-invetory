<template>
  <q-card class="inventory-datastore-card" flat bordered>
    <q-card-section class="card-content">
      <div class="card-header">
        <div class="header-left">
          <div class="store-icon">
            <q-icon name="storage" size="20px" />
          </div>
          <div class="header-info">
            <h4 class="item-title">{{ dataStore.name }}</h4>
            <p class="item-subtitle">{{ environmentName }}</p>
          </div>
        </div>
        <q-chip
          v-if="dataStore.kind"
          dense
          outline
          color="purple"
          text-color="purple"
          size="xs"
        >
          {{ dataStore.kind }}
        </q-chip>
      </div>

      <div class="card-details">
        <div class="detail-item" v-if="dataStore.description">
          <span class="detail-label">Description:</span>
          <span class="detail-value">{{ dataStore.description }}</span>
        </div>
        <div class="detail-item" v-if="dataStore.connectionUri">
          <span class="detail-label">Connection:</span>
          <span class="detail-value">{{ dataStore.connectionUri }}</span>
        </div>
        <div class="detail-item" v-if="platformName">
          <span class="detail-label">Platform:</span>
          <span class="detail-value">{{ platformName }}</span>
        </div>
      </div>

      <div v-if="dataStore.tagIds?.length" class="card-tags">
        <q-chip
          v-for="tagId in dataStore.tagIds"
          :key="tagId"
          dense
          outline
          color="grey-7"
          size="xs"
        >
          {{ tagLookup[tagId] ?? tagId }}
        </q-chip>
      </div>

      <div class="card-footer">
        <q-btn
          flat
          dense
          no-caps
          icon="edit"
          label="Edit"
          color="purple"
          size="sm"
          :to="`/data-stores`"
        />
      </div>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import type { DataStore } from '../../api/client'

defineProps<{
  dataStore: DataStore
  environmentName: string
  platformName: string
  tagLookup: Record<string, string>
}>()
</script>

<style scoped>
.inventory-datastore-card {
  border-radius: 12px;
  border: 1px solid rgba(156, 39, 176, 0.2);
  background: linear-gradient(135deg, rgba(156, 39, 176, 0.03) 0%, var(--fuse-panel-bg) 100%);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.inventory-datastore-card:hover {
  box-shadow: var(--fuse-shadow-2);
  transform: translateY(-2px);
}

.card-content {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 0.75rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  flex: 1;
  min-width: 0;
}

.store-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: grid;
  place-items: center;
  background: rgba(156, 39, 176, 0.1);
  color: rgba(156, 39, 176, 0.8);
  flex-shrink: 0;
}

.header-info {
  flex: 1;
  min-width: 0;
}

.item-title {
  font-size: 1rem;
  font-weight: 600;
  margin: 0 0 0.125rem;
  line-height: 1.3;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.item-subtitle {
  font-size: 0.8125rem;
  color: var(--fuse-text-muted);
  margin: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.card-details {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
  padding: 0.5rem 0;
  border-top: 1px solid var(--fuse-panel-border);
  border-bottom: 1px solid var(--fuse-panel-border);
}

.detail-item {
  display: flex;
  gap: 0.5rem;
  font-size: 0.8125rem;
  line-height: 1.4;
}

.detail-label {
  color: var(--fuse-text-subtle);
  font-weight: 500;
  flex-shrink: 0;
}

.detail-value {
  color: var(--fuse-text-primary);
  word-break: break-word;
}

.card-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.card-footer {
  display: flex;
  justify-content: flex-end;
  padding-top: 0.5rem;
  border-top: 1px solid var(--fuse-panel-border);
}
</style>
