self.addEventListener('install', () => {
    self.skipWaiting()
})

self.addEventListener('activate', (event) => {
    event.waitUntil(clients.claim())
})

self.addEventListener('push', (event) => {
    let title = 'Notification'
    let body = ''

    try {
        const data = event.data.json()
        title = data.title
        body = data.body
    } catch {
        body = event.data.text()
    }

    event.waitUntil(
        self.registration.showNotification(title, {
            body,
            icon: '/favicon.svg',
        }),
    )
})
