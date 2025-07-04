import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    token: localStorage.getItem('jwt') || '',
    username: '',
  }),
  actions: {
    setToken(token: string) {
      this.token = token
      localStorage.setItem('jwt', token)
    },
    clear() {
      this.token = ''
      this.username = ''
      localStorage.removeItem('jwt')
    },
  },
})
