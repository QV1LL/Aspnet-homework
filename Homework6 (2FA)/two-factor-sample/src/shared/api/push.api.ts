import { api } from '../index'
import type { PushNotificationPayload } from '@/features/push-notify/model/types'

export const pushApi = {
    subscribe: (subscription: PushSubscriptionJSON) => api.post('/push/subscribe', subscription),

    send: (userId: string, payload: PushNotificationPayload) => api.post(`/push/send/${userId}`, payload),

    broadcast: (payload: PushNotificationPayload) => api.post('/push/broadcast', payload),
}
