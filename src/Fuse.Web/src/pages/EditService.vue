<template>
    <q-page class="q-pa-md">
        <div class="row items-center justify-left q-mb-md">
            <q-btn round color="primary" icon="arrow_back" @click="goBack" />
            <h4 class="text-h4 q-pl-md">{{ isLoading ? 'Loading...' : 'Edit Service' }}</h4>
        </div>

        <div v-if="error" class="q-pa-md">
            <q-banner class="bg-negative text-white">
                <template v-slot:avatar>
                    <q-icon name="error" color="white" />
                </template>
                {{ error }}
            </q-banner>
            <q-btn color="primary" label="Go Back" @click="goBack" class="q-mt-md" />
        </div>

        <service-form
            v-else-if="service"
            v-model="service"
            mode="edit"
            :loading="saving"
            submit-label="Save Changes"
            @submit="handleSubmit"
            @cancel="goBack"
        />

        <div v-else-if="isLoading" class="q-pa-md text-center">
            <q-spinner color="primary" size="3em" />
        </div>
    </q-page>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import ServiceForm from '../components/ServiceForm.vue'
import { Client, type ServiceManifest, type ICreateServiceCommand } from '../httpClients/client.gen'

const $q = useQuasar()
const router = useRouter()
const route = useRoute()

const isLoading = ref(true)
const saving = ref(false)
const service = ref<ServiceManifest | null>(null)
const error = ref<string | null>(null)

const serviceId = route.params.id as string

onMounted(async () => {
    await loadService()
})

async function loadService() {
    try {
        isLoading.value = true
        error.value = null
        const baseUrl = (import.meta as any).env?.VITE_API_BASE_URL ?? ''
        const api = new Client(baseUrl)
        service.value = await api.servicesGET(serviceId)
        
        if (!service.value) {
            error.value = 'Service not found'
        }
    } catch (e: any) {
        error.value = e?.message ?? 'Failed to load service'
        console.error('Error loading service:', e)
    } finally {
        isLoading.value = false
    }
}

async function handleSubmit(payload: ICreateServiceCommand | ServiceManifest) {
    try {
        saving.value = true
        const baseUrl = (import.meta as any).env?.VITE_API_BASE_URL ?? ''
        const api = new Client(baseUrl)
        
        // Ensure we have the ID for the PUT request
        await api.servicesPUT(serviceId, payload as ServiceManifest)
        
        $q.notify({ type: 'positive', message: 'Service updated successfully' })
        router.push('/services')
    } catch (e: any) {
        $q.notify({ type: 'negative', message: e?.message ?? 'Failed to update service' })
        console.error('Error updating service:', e)
    } finally {
        saving.value = false
    }
}

function goBack() {
    router.push('/services')
}
</script>
