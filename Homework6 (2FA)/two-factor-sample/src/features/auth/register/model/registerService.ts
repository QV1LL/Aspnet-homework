import { api } from '@/shared'
import type { RegisterRequest, RegisterResponse } from './types'
import { isAxiosError } from 'axios'

export const registerUser = async (data: RegisterRequest): Promise<RegisterResponse | undefined> => {
    try {
        const response = await api.post<RegisterResponse>('/register', data)
        return response.data
    } catch (error: unknown) {
        if (isAxiosError(error)) {
            const message = error.response?.data?.message || 'Registration failed'
            throw new Error(message)
        }
    }
}
