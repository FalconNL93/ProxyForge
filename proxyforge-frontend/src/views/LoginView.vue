<template>
  <VaForm ref="form" @submit.prevent="submit">
    <h1 class="font-semibold text-4xl mb-4">Log in</h1>
    <VaInput
      v-model="formData.username"
      :rules="[validators.required]"
      class="mb-4"
      label="Username"
      type="text"
    />
    <VaValue v-slot="isPasswordVisible" :default-value="false">
      <VaInput
        v-model="formData.password"
        :rules="[validators.required]"
        :type="isPasswordVisible.value ? 'text' : 'password'"
        class="mb-4"
        label="Password"
        @clickAppendInner.stop="isPasswordVisible.value = !isPasswordVisible.value"
      >
        <template #appendInner>
          <VaIcon
            :name="isPasswordVisible.value ? 'mso-visibility_off' : 'mso-visibility'"
            class="cursor-pointer"
            color="secondary"
          />
        </template>
      </VaInput>
    </VaValue>
    <div class="flex justify-center mt-4">
      <VaButton class="w-full" @click="submit"> Login</VaButton>
    </div>
  </VaForm>
</template>

<script lang="ts" setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useForm, useToast } from 'vuestic-ui'
import { login } from '@/services/auth'

const { validate } = useForm('form')
const { push } = useRouter()
const { init } = useToast()

const formData = reactive({
  username: '',
  password: ''
})

const submit = async () => {
  if (validate()) {
    try {
      await login({ username: formData.username, password: formData.password })
      init({ message: "You've successfully logged in", color: 'success' })
      push({ name: 'home' })
    } catch (e: any) {
      init({ message: e?.response?.data?.message || 'Login failed', color: 'danger' })
    }
  }
}
</script>
