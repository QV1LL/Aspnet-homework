import axios from 'axios'

export const api = axios.create({
    baseURL: import.meta.env.VITE_BLOG_API_URL,
})
