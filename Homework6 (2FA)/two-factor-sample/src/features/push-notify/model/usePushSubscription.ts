import { urlBase64ToUint8Array } from '@/lib/push/vapidHelper'
import { pushApi } from '@/shared/api/push.api'
import type { PushNotificationPayload } from './types'

export function usePushSubscription() {
    const subscribe = async (): Promise<void> => {
        if (!('serviceWorker' in navigator)) {
            console.error('Service Workers not supported')
            return
        }

        console.log('Registering SW...')
        const registration = await navigator.serviceWorker.register('/sw.js')
        console.log('SW registered:', registration)

        await navigator.serviceWorker.ready // ← wait until fully active
        console.log('SW ready')

        const permission = await Notification.requestPermission()
        console.log('Permission:', permission)
        if (permission !== 'granted') return

        const subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: urlBase64ToUint8Array(import.meta.env.VITE_VAPID_PUBLIC_KEY),
        })
        console.log('Subscription endpoint:', subscription.endpoint)

        await pushApi.subscribe(subscription.toJSON())
        console.log('Subscription sent to backend')
    }

    const send = async (userId: string, payload: PushNotificationPayload): Promise<void> => {
        await pushApi.send(userId, payload)
    }

    const broadcast = async (payload: PushNotificationPayload): Promise<void> => {
        await pushApi.broadcast(payload)
    }

    return { subscribe, send, broadcast }
}
