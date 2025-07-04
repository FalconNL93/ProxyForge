<template>
  <div class="app-layout">
    <nav class="sidebar">
      <router-link to="/">Home</router-link>
      <router-link to="/about">About</router-link>
      <router-link to="/login">Logout</router-link>
    </nav>
    <main class="main-content">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
// Minimal AppLayout for admin panel
</script>

<style scoped>
.app-layout {
  display: flex;
  min-height: 100vh;
}
.sidebar {
  width: 200px;
  background: #f8f9fa;
  padding: 2rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.main-content {
  flex: 1;
  padding: 2rem;
}
</style>
