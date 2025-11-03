import { createRouter, createWebHistory } from 'vue-router'
import Home from '../pages/Home.vue'
import Services from '../pages/Services.vue'
import NewService from '../pages/NewService.vue'
import EditService from '../pages/EditService.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  {
    path: '/services',
    name: 'services',
    component: Services
  },
  {
    path: '/services/new',
    name: 'new-service',
    component: NewService
  },
  {
    path: '/services/:id/edit',
    name: 'edit-service',
    component: EditService
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
