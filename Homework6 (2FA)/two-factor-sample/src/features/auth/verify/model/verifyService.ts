import { api } from '@/shared'
import { useUserStore } from '@/entities/user'
import type { TwoFactorMethod } from '@/entities/user'

interface Verify2FARequest {
    userId: string
    method: TwoFactorMethod
    code: string
}

export const verify2FA = async (data: Verify2FARequest): Promise<void> => {
    const { data: response } = await api.post<{ token: string }>('/2fa/verify', data)
    useUserStore.getState().setToken(response.token)
}
