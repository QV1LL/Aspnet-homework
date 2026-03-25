import { api } from '@/shared'
import { useUserStore } from '@/entities/user'
import type { LoginRequest } from './types'
import type { TwoFactorMethod } from '@/entities/user'

export const loginUser = async (credentials: LoginRequest) => {
    const { data } = await api.post('/login', credentials)

    if (data.requires2FA) {
        return {
            status: 'requires_2fa',
            userId: data.userId,
            availableMethods: data.availableMethods as TwoFactorMethod[],
        }
    }

    useUserStore.getState().setToken(data.token)
    return { status: 'success' }
}

export const sendSmsCode = async (userId: string) => {
    await api.post('/2fa/sms/send-login-code', { userId })
}

export const verifyTwoFactor = async (params: { userId: string; method: TwoFactorMethod; code: string }) => {
    const { data } = await api.post<{ token: string }>('/2fa/verify', params)
    useUserStore.getState().setToken(data.token)
    return data.token
}
