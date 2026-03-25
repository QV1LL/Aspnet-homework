import { useUserStore } from '@/entities/user'
import axios from 'axios'

export const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || '/api',
    headers: {
        'Content-Type': 'application/json',
    },
})

api.interceptors.request.use((config) => {
    const token = useUserStore.getState().token
    if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})
