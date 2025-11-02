<script setup lang="ts">
// App.vue is now the main layout component
// Individual page content will be rendered through <RouterView />
</script>

<template>
  <div id="app">
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <!-- Add more navigation links here as needed -->
    </nav>
    <main>
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
#app {
  max-width: 1280px;
  margin: 0 auto;
  padding: 2rem;
  text-align: center;
}

nav {
  padding: 1rem;
  margin-bottom: 2rem;
  border-bottom: 1px solid #ccc;
}

nav a {
  margin: 0 1rem;
  text-decoration: none;
  color: #42b983;
  font-weight: bold;
}

nav a:hover {
  text-decoration: underline;
}

nav a.router-link-active {
  color: #359268;
}

main {
  padding: 1rem;
}
</style>
