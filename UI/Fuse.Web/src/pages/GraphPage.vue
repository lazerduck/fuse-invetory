<template>
  <div class="graph-page">
    <section class="page-header">
      <div>
        <h1>Relationship Graph</h1>
        <p class="subtitle">
          Visualize connections between environments, servers, applications, and dependencies.
        </p>
      </div>
    </section>

    <section class="graph-controls">
      <div class="filters">
        <q-input
          v-model="searchQuery"
          dense
          outlined
          debounce="300"
          clearable
          placeholder="Search nodes by name"
          class="filter-input"
          prepend-inner-icon="search"
        />
        
        <q-select
          v-model="selectedEnvironment"
          dense
          outlined
          emit-value
          map-options
          clearable
          :options="environmentOptions"
          class="filter-select"
          placeholder="Filter by environment"
        />

        <q-select
          v-model="selectedNodeTypes"
          dense
          outlined
          emit-value
          map-options
          multiple
          clearable
          :options="nodeTypeOptions"
          class="filter-select"
          placeholder="Filter by node type"
        />

        <q-btn
          flat
          dense
          icon="refresh"
          label="Reset View"
          @click="resetView"
        />
      </div>
    </section>

    <section class="graph-container-wrapper">
      <div v-if="isLoading" class="loading-state">
        <q-spinner color="primary" size="48px" />
        <p>Loading graph data...</p>
      </div>
      <div v-else>
        <div ref="graphContainer" class="graph-container"></div>
        <div class="legend">
          <div class="legend-item">
            <div class="legend-color" style="background: #1976D2;"></div>
            <span>Environment</span>
          </div>
          <div class="legend-item">
            <div class="legend-color" style="background: #388E3C;"></div>
            <span>Server</span>
          </div>
          <div class="legend-item">
            <div class="legend-color" style="background: #F57C00;"></div>
            <span>Application</span>
          </div>
          <div class="legend-item">
            <div class="legend-color" style="background: #7B1FA2;"></div>
            <span>Data Store</span>
          </div>
          <div class="legend-item">
            <div class="legend-color" style="background: #C2185B;"></div>
            <span>External Resource</span>
          </div>
          <div class="legend-item">
            <div class="legend-color" style="background: #00796B;"></div>
            <span>Account</span>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onUnmounted } from 'vue'
import { Network } from 'vis-network'
import { DataSet } from 'vis-data'
import { useApplications } from '../composables/useApplications'
import { useServers } from '../composables/useServers'
import { useEnvironments } from '../composables/useEnvironments'
import { useExternalResources } from '../composables/useExternalResources'
import { useDataStores } from '../composables/useDataStores'
import { TargetKind } from '../api/client'
// Refs
const graphContainer = ref<HTMLElement | null>(null)
const searchQuery = ref('')
const selectedEnvironment = ref<string | null>(null)
const selectedNodeTypes = ref<string[]>([])

// Data queries
const applicationsQuery = useApplications()
const serversQuery = useServers()
const environmentsQuery = useEnvironments()
const externalResourcesQuery = useExternalResources()
const dataStoresQuery = useDataStores()

// Consider all query loading states so we don't initialize prematurely
const isLoading = computed(() => 
  applicationsQuery.isLoading.value || 
  serversQuery.isLoading.value || 
  environmentsQuery.isLoading.value ||
  externalResourcesQuery.isLoading.value ||
  dataStoresQuery.isLoading.value
)

// Filter options
const environmentOptions = computed(() => {
  return (environmentsQuery.data.value ?? []).map((env) => ({
    label: env.name ?? 'Environment',
    value: env.id ?? ''
  }))
})

const nodeTypeOptions = [
  { label: 'Environments', value: 'environment' },
  { label: 'Servers', value: 'server' },
  { label: 'Applications', value: 'application' },
  { label: 'Data Stores', value: 'datastore' },
  { label: 'External Resources', value: 'external' },
  { label: 'Accounts', value: 'account' }
]

// Network instance
let network: Network | null = null
// Reusable DataSet instances to avoid re-triggering full physics recalculations
let nodesDataSet: DataSet<GraphNode> | null = null
let edgesDataSet: DataSet<GraphEdge> | null = null

interface GraphNode {
  id: string
  label: string
  group: string
  title?: string
  shape?: string
  color?: string
}

interface GraphEdge {
  id?: string
  from: string
  to: string
  label?: string
  arrows?: string
}

// Transform data into graph nodes and edges
const graphData = computed(() => {
  const nodes: GraphNode[] = []
  const edges: GraphEdge[] = []
  const nodeIds = new Set<string>()
  let edgeId = 0

  const applications = applicationsQuery.data.value ?? []
  const servers = serversQuery.data.value ?? []
  const environments = environmentsQuery.data.value ?? []
  const externalResources = externalResourcesQuery.data.value ?? []
  const dataStores = dataStoresQuery.data.value ?? []

  // Apply filters
  const shouldIncludeNodeType = (type: string) => {
    return selectedNodeTypes.value.length === 0 || selectedNodeTypes.value.includes(type)
  }

  const matchesSearch = (text: string) => {
    if (!searchQuery.value) return true
    return text.toLowerCase().includes(searchQuery.value.toLowerCase())
  }

  // Add environments
  if (shouldIncludeNodeType('environment')) {
    environments.forEach((env) => {
      if (!env.id) return
      if (selectedEnvironment.value && env.id !== selectedEnvironment.value) return
      if (!matchesSearch(env.name ?? '')) return

      nodes.push({
        id: `env-${env.id}`,
        label: env.name ?? 'Environment',
        group: 'environment',
        title: env.description ?? env.name ?? 'Environment',
        shape: 'box',
        color: '#1976D2'
      })
      nodeIds.add(`env-${env.id}`)
    })
  }

  // Add servers
  if (shouldIncludeNodeType('server')) {
    servers.forEach((server) => {
      if (!server.id) return
      if (selectedEnvironment.value && server.environmentId !== selectedEnvironment.value) return
      if (!matchesSearch(server.name ?? '')) return

      nodes.push({
        id: `server-${server.id}`,
        label: server.name ?? 'Server',
        group: 'server',
        title: `${server.name}\n${server.hostname ?? ''}`,
        shape: 'box',
        color: '#388E3C'
      })
      nodeIds.add(`server-${server.id}`)

      // Link server to environment
      if (server.environmentId && nodeIds.has(`env-${server.environmentId}`)) {
        edges.push({
          id: `edge-${edgeId++}`,
          from: `env-${server.environmentId}`,
          to: `server-${server.id}`,
          arrows: 'to'
        })
      }
    })
  }

  // Add data stores
  if (shouldIncludeNodeType('datastore')) {
    dataStores.forEach((ds) => {
      if (!ds.id) return
      if (selectedEnvironment.value && ds.environmentId !== selectedEnvironment.value) return
      if (!matchesSearch(ds.name ?? '')) return

      nodes.push({
        id: `datastore-${ds.id}`,
        label: ds.name ?? 'Data Store',
        group: 'datastore',
        title: `${ds.name}\n${ds.kind ?? ''}`,
        shape: 'database',
        color: '#7B1FA2'
      })
      nodeIds.add(`datastore-${ds.id}`)

      // Link data store to environment
      if (ds.environmentId && nodeIds.has(`env-${ds.environmentId}`)) {
        edges.push({
          id: `edge-${edgeId++}`,
          from: `env-${ds.environmentId}`,
          to: `datastore-${ds.id}`,
          arrows: 'to'
        })
      }
    })
  }

  // Add external resources
  if (shouldIncludeNodeType('external')) {
    externalResources.forEach((resource) => {
      if (!resource.id) return
      if (!matchesSearch(resource.name ?? '')) return

      nodes.push({
        id: `external-${resource.id}`,
        label: resource.name ?? 'External Resource',
        group: 'external',
        title: `${resource.name}\n${resource.resourceUri ?? ''}`,
        shape: 'diamond',
        color: '#C2185B'
      })
      nodeIds.add(`external-${resource.id}`)
    })
  }

  // Add applications and their instances
  if (shouldIncludeNodeType('application')) {
    applications.forEach((app) => {
      if (!app.id) return
      if (!matchesSearch(app.name ?? '')) return

      // Check if app has instances in selected environment
      if (selectedEnvironment.value) {
        const hasInstanceInEnv = (app.instances ?? []).some(
          (inst) => inst.environmentId === selectedEnvironment.value
        )
        if (!hasInstanceInEnv) return
      }

      nodes.push({
        id: `app-${app.id}`,
        label: app.name ?? 'Application',
        group: 'application',
        title: `${app.name}\n${app.framework ?? ''}\n${app.owner ?? ''}`,
        shape: 'ellipse',
        color: '#F57C00'
      })
      nodeIds.add(`app-${app.id}`)

      // Process instances
      ;(app.instances ?? []).forEach((instance) => {
        if (!instance.id) return

        // Filter by environment
        if (selectedEnvironment.value && instance.environmentId !== selectedEnvironment.value) {
          return
        }

        // Link app to server
        if (instance.serverId && nodeIds.has(`server-${instance.serverId}`)) {
          edges.push({
            id: `edge-${edgeId++}`,
            from: `server-${instance.serverId}`,
            to: `app-${app.id}`,
            arrows: 'to'
          })
        }

        // Link app to environment if no server
        if (!instance.serverId && instance.environmentId && nodeIds.has(`env-${instance.environmentId}`)) {
          edges.push({
            id: `edge-${edgeId++}`,
            from: `env-${instance.environmentId}`,
            to: `app-${app.id}`,
            arrows: 'to'
          })
        }

        // Process dependencies
        ;(instance.dependencies ?? []).forEach((dep) => {
          if (!dep.targetId || !dep.targetKind) return

          let targetNodeId = ''
          if (dep.targetKind === TargetKind.Application) {
            targetNodeId = `app-${dep.targetId}`
          } else if (dep.targetKind === TargetKind.DataStore) {
            targetNodeId = `datastore-${dep.targetId}`
          } else if (dep.targetKind === TargetKind.External) {
            targetNodeId = `external-${dep.targetId}`
          }

          if (targetNodeId && nodeIds.has(targetNodeId)) {
            edges.push({
              id: `edge-${edgeId++}`,
              from: `app-${app.id}`,
              to: targetNodeId,
              arrows: 'to',
              label: dep.port ? `:${dep.port}` : undefined
            })
          }
        })
      })
    })
  }

  return { nodes, edges }
})

function initNetwork() {
  if (!graphContainer.value) return

  // Reuse datasets to prevent repeated allocation & physics restarts
  nodesDataSet = new DataSet(graphData.value.nodes)
  edgesDataSet = new DataSet(graphData.value.edges)

  const data = {
    nodes: nodesDataSet,
    edges: edgesDataSet
  }

  const options = {
    nodes: {
      font: {
        size: 14,
        color: '#ffffff'
      },
      borderWidth: 2,
      borderWidthSelected: 3
    },
    edges: {
      width: 2,
      color: { color: '#848484', highlight: '#000000' },
      smooth: {
        enabled: true,
        type: 'continuous',
        roundness: 0.5
      },
      font: {
        size: 12,
        align: 'middle'
      }
    },
    physics: {
      enabled: true,
      barnesHut: {
        gravitationalConstant: -1200, // Slightly less intense to reduce CPU
        centralGravity: 0.3,
        springLength: 150,
        springConstant: 0.04,
        damping: 0.12
      },
      stabilization: {
        iterations: 120
      }
    },
    interaction: {
      hover: true,
      tooltipDelay: 100,
      zoomView: true,
      dragView: true
    },
    layout: {
      improvedLayout: true,
      hierarchical: false
    }
  }

  network = new Network(graphContainer.value, data, options)

  // Disable physics automatically after stabilization to avoid runaway CPU usage
  network.once('stabilizationIterationsDone', () => {
    if (!network) return
    // If graph is very large keep physics off after initial layout
    const nodeCount = graphData.value.nodes.length
    if (nodeCount > 300) {
      console.warn('[Graph] Large node count detected, disabling physics post-stabilization.')
    }
    network.setOptions({ physics: false })
  })

  // Click handler for future detail panels
  network.on('click', (params) => {
    if (params.nodes.length > 0) {
      const nodeId = params.nodes[0]
      console.log('Node clicked:', nodeId)
    }
  })
}

function updateNetwork() {
  if (!network || !nodesDataSet || !edgesDataSet) return

  // Efficiently update existing datasets instead of recreating them
  nodesDataSet.clear()
  edgesDataSet.clear()
  nodesDataSet.add(graphData.value.nodes)
  edgesDataSet.add(graphData.value.edges)

  // Fit view lightly after major changes (skip if extremely large)
  const total = graphData.value.nodes.length + graphData.value.edges.length
  if (total < 1000) {
    try { network.fit({ animation: { duration: 300, easingFunction: 'easeInOutQuad' } }) } catch {}
  }
}

function resetView() {
  searchQuery.value = ''
  selectedEnvironment.value = null
  selectedNodeTypes.value = []
  if (network) {
    network.fit()
  }
}

// Watch for data changes and update the network (no deep watch needed)
watch(graphData, () => {
  if (network) updateNetwork()
})

onMounted(() => {
  // Wait a bit for the container to be ready
  setTimeout(() => {
    if (!isLoading.value) {
      initNetwork()
    }
  }, 100)
})

// Watch for loading state changes
watch(isLoading, (newVal) => {
  if (!newVal && graphContainer.value && !network) {
    setTimeout(() => {
      initNetwork()
    }, 100)
  }
})

onUnmounted(() => {
  if (network) {
    network.destroy()
    network = null
  }
})
</script>

<style scoped>
.graph-page {
  padding: 1.5rem clamp(1.25rem, 3vw, 2.5rem);
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  height: calc(100vh - 60px);
  max-width: 100%;
  margin: 0 auto;
}

.page-header h1 {
  font-size: 1.75rem;
  margin: 0 0 0.375rem;
  font-weight: 600;
}

.subtitle {
  margin: 0;
  font-size: 0.9375rem;
  color: rgba(0, 0, 0, 0.6);
  max-width: 60ch;
}

.graph-controls {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.filters {
  display: flex;
  gap: 0.625rem;
  flex-wrap: wrap;
  align-items: center;
}

.filter-input,
.filter-select {
  min-width: 220px;
}

.graph-container-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
  position: relative;
}

.graph-container {
  width: 100%;
  height: 100%;
  min-height: 500px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background: #fafafa;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  min-height: 500px;
  color: rgba(0, 0, 0, 0.5);
}

.legend {
  position: absolute;
  top: 10px;
  right: 10px;
  background: white;
  padding: 12px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}

.legend-color {
  width: 20px;
  height: 20px;
  border-radius: 4px;
}

@media (max-width: 640px) {
  .filters {
    width: 100%;
  }

  .filter-input,
  .filter-select {
    flex: 1 1 100%;
    min-width: unset;
  }

  .legend {
    position: static;
    margin-top: 10px;
  }
}
</style>
