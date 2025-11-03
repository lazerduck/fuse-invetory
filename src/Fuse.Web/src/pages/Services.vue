<template>
  <q-page class="q-pa-md">
    <div class="row items-center justify-between q-mb-md">
      <div class="text-h4">Services</div>

      <q-btn
        round
        color="primary"
        icon="add"
        @click="$router.push('/services/new')"
      />
    </div>
    <div class="q-pa-md">
      <q-input
        filled
        v-model="filter"
        label="Search services"
        class="q-mb-md"
        debounce="300"
      />

      <q-table
        :rows="filteredServices"
        :columns="columns"
        row-key="id"
        flat
        bordered
      >
      <template v-slot:body-cell-actions="props">
        <q-td align="right">
          <q-btn
            dense
            flat
            icon="visibility"
            @click="viewService(props.row)"
            color="primary"
          />
        </q-td>
      </template>
      </q-table>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useServicesStore } from '../stores/ServicesStore';
import type { ServiceManifest } from '../httpClients/client.gen';

const router = useRouter();
const servicesStore = useServicesStore();

const filter = ref('');

const columns = [
  { name: 'name', label: 'Name', field: 'name', sortable: true },
  { name: 'type', label: 'Type', field: 'type', sortable: true },
  { name: 'author', label: 'Author', field: 'author', sortable: true },
  { name: 'updatedAt', label: 'Updated At', field: 'updatedAt', sortable: true },
  { name: 'actions', label: 'Actions', field: 'actions' }
]

const filteredServices = computed(() => {
  if (!filter.value) {
    return servicesStore.services;
  }
  return servicesStore.services.filter(service =>
    (service.name?.toLowerCase() ?? '').includes(filter.value.toLowerCase()) ||
    (service.type ? service.type.toString().toLowerCase() : '').includes(filter.value.toLowerCase()) ||
    (service.author?.toLowerCase() ?? '').includes(filter.value.toLowerCase())
  );
});

function viewService(service: ServiceManifest) {
  router.push(`/services/${service.id}/edit`);
}

onMounted(async () => {
  await servicesStore.loadAll();
});

</script>
