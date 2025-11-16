<template>
  <q-card class="inventory-instance-card" flat bordered>
    <q-card-section class="card-content">
      <div class="card-header">
        <div class="header-left">
          <div class="app-icon">
            <q-icon name="precision_manufacturing" size="20px" />
          </div>
          <div class="header-info">
            <h4 class="item-title">{{ applicationName }}</h4>
            <p class="item-subtitle">{{ environmentName }}</p>
          </div>
        </div>
        <div class="header-right">
          <q-chip
            v-if="healthStatus"
            dense
            :color="healthStatusColor"
            :text-color="healthStatusTextColor"
            size="xs"
            :icon="healthStatusIcon"
          >
            {{ healthStatusLabel }}
          </q-chip>
        </div>
      </div>

      <div class="card-details">
        <div class="detail-item" v-if="instance.baseUri">
          <span class="detail-label">Base URI:</span>
          <span class="detail-value">{{ instance.baseUri }}</span>
        </div>
        <div class="detail-item" v-if="platformName">
          <span class="detail-label">Platform:</span>
          <span class="detail-value">{{ platformName }}</span>
        </div>
        <div class="detail-item" v-if="instance.version">
          <span class="detail-label">Version:</span>
          <span class="detail-value">v{{ instance.version }}</span>
        </div>
      </div>

      <div class="card-actions">
        <q-btn
          v-if="instance.baseUri"
          flat
          dense
          no-caps
          icon="launch"
          label="Base"
          color="primary"
          size="sm"
          :href="instance.baseUri"
          target="_blank"
          rel="noopener"
        />
        <q-btn
          v-if="instance.healthUri"
          flat
          dense
          no-caps
          icon="favorite"
          label="Health"
          color="positive"
          size="sm"
          :href="instance.healthUri"
          target="_blank"
          rel="noopener"
        />
        <q-btn
          v-if="instance.openApiUri"
          flat
          dense
          no-caps
          icon="api"
          label="OpenAPI"
          color="secondary"
          size="sm"
          :href="instance.openApiUri"
          target="_blank"
          rel="noopener"
        />
      </div>

      <div v-if="visibleDependencies.length" class="card-dependencies">
        <p class="dependencies-label">Dependencies:</p>
        <div class="dependency-chips">
          <q-chip
            v-for="dependency in visibleDependencies"
            :key="dependency.id"
            dense
            outline
            color="secondary"
            size="xs"
          >
            {{ dependencyFormatter(dependency) }}
          </q-chip>
          <q-chip
            v-if="hasHiddenDependencies"
            dense
            outline
            color="grey-7"
            size="xs"
          >
            +{{ hiddenDependenciesCount }} more
          </q-chip>
        </div>
      </div>

      <div class="card-footer">
        <q-btn
          flat
          dense
          no-caps
          icon="edit"
          label="Edit"
          color="primary"
          size="sm"
          :to="`/applications/${applicationId}/instances/${instance.id}`"
        />
      </div>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ApplicationInstance } from '../../api/client'
import { useHealthCheck } from '../../composables/useHealthCheck'
import { MonitorStatus } from '../../types/health'

const props = defineProps<{
  instance: ApplicationInstance
  applicationId: string
  applicationName: string
  environmentName: string
  platformName: string
  dependencyFormatter: (dependency: any) => string
}>()

const MAX_VISIBLE_DEPENDENCIES = 6

const visibleDependencies = computed(() => {
  const deps = props.instance.dependencies ?? []
  return deps.slice(0, MAX_VISIBLE_DEPENDENCIES)
})

const hasHiddenDependencies = computed(() => {
  const deps = props.instance.dependencies ?? []
  return deps.length > MAX_VISIBLE_DEPENDENCIES
})

const hiddenDependenciesCount = computed(() => {
  const deps = props.instance.dependencies ?? []
  return Math.max(0, deps.length - MAX_VISIBLE_DEPENDENCIES)
})

const hasHealthCheck = computed(() => !!props.instance.healthUri)

const { data: healthStatus } = useHealthCheck(
  props.applicationId,
  props.instance.id ?? '',
  hasHealthCheck.value
)

const healthStatusColor = computed(() => {
  if (!healthStatus.value) return 'grey'
  
  switch (healthStatus.value.Status) {
    case MonitorStatus.Up:
      return 'positive'
    case MonitorStatus.Down:
      return 'negative'
    case MonitorStatus.Pending:
      return 'warning'
    case MonitorStatus.Maintenance:
      return 'info'
    default:
      return 'grey'
  }
})

const healthStatusTextColor = computed(() => {
  return 'white'
})

const healthStatusIcon = computed(() => {
  if (!healthStatus.value) return 'help'
  
  switch (healthStatus.value.Status) {
    case MonitorStatus.Up:
      return 'check_circle'
    case MonitorStatus.Down:
      return 'cancel'
    case MonitorStatus.Pending:
      return 'schedule'
    case MonitorStatus.Maintenance:
      return 'construction'
    default:
      return 'help'
  }
})

const healthStatusLabel = computed(() => {
  if (!healthStatus.value) return 'Unknown'
  
  switch (healthStatus.value.Status) {
    case MonitorStatus.Up:
      return 'Healthy'
    case MonitorStatus.Down:
      return 'Down'
    case MonitorStatus.Pending:
      return 'Pending'
    case MonitorStatus.Maintenance:
      return 'Maintenance'
    default:
      return 'Unknown'
  }
})
</script>

<style scoped>
.inventory-instance-card {
  border-radius: 12px;
  border: 1px solid var(--fuse-panel-border);
  background: var(--fuse-panel-bg);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.inventory-instance-card:hover {
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

.app-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: grid;
  place-items: center;
  background: rgba(64, 120, 255, 0.1);
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

.header-right {
  flex-shrink: 0;
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

.card-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.card-dependencies {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.dependencies-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--fuse-text-subtle);
  margin: 0;
  font-weight: 500;
}

.dependency-chips {
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
