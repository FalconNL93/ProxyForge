<template>
  <div class="auth-layout">
    <main class="auth-content">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
// Minimal AuthLayout for login/register
</script>

<style scoped>
.auth-layout {
  display: flex;
  min-height: 100vh;
  align-items: center;
  justify-content: center;
  background: #f4f6fb;
}
.auth-content {
  min-width: 320px;
  max-width: 400px;
  padding: 2rem;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
</style>
