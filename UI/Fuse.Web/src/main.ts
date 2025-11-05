import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { Quasar, Notify } from 'quasar'
import { VueQueryPlugin } from '@tanstack/vue-query'
import router from './router'

import App from './App.vue'

createApp(App)
  .use(createPinia())
  .use(router)
  .use(VueQueryPlugin)
  .use(Quasar, { plugins: { Notify}})
  .mount('#app')


  // npm i vue-router pinia @tanstack/vue-query axios zod @vueuse/core