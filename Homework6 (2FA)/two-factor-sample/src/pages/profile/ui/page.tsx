import { useEffect, useState } from 'react'
import { useUserStore } from '@/entities/user'
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { SmsSetupDialog, TotpSetupDialog } from '@/features/auth'
import { usePushSubscription } from '@/features/push-notify'
import { LogOut, Bell, Send, Radio } from 'lucide-react'

const methodLabel: Record<string, string> = {
    Sms: 'SMS',
    Totp: 'Authenticator App',
}

export const ProfilePage = () => {
    const { user, isLoading, fetchProfile, logout } = useUserStore()
    const [isSmsOpen, setSmsOpen] = useState(false)
    const [isTotpOpen, setTotpOpen] = useState(false)

    const { subscribe, send, broadcast } = usePushSubscription()

    const [targetUserId, setTargetUserId] = useState('')
    const [sendTitle, setSendTitle] = useState('')
    const [sendBody, setSendBody] = useState('')
    const [isSending, setIsSending] = useState(false)

    const [broadcastTitle, setBroadcastTitle] = useState('')
    const [broadcastBody, setBroadcastBody] = useState('')
    const [isBroadcasting, setIsBroadcasting] = useState(false)

    useEffect(() => {
        fetchProfile()
    }, [fetchProfile])

    if (isLoading && !user) return <Skeleton className="h-[400px] w-full" />
    if (!user) return <div className="p-10 text-center">Будь ласка, залогіньтесь знову.</div>

    const hasSms = user.enabledTwoFactorMethods.includes('Sms')
    const hasTotp = user.enabledTwoFactorMethods.includes('Totp')

    const handleSend = async () => {
        if (!targetUserId || !sendTitle || !sendBody) return
        setIsSending(true)
        try {
            await send(targetUserId, { title: sendTitle, body: sendBody })
            setTargetUserId('')
            setSendTitle('')
            setSendBody('')
        } finally {
            setIsSending(false)
        }
    }

    const handleBroadcast = async () => {
        if (!broadcastTitle || !broadcastBody) return
        setIsBroadcasting(true)
        try {
            await broadcast({ title: broadcastTitle, body: broadcastBody })
            setBroadcastTitle('')
            setBroadcastBody('')
        } finally {
            setIsBroadcasting(false)
        }
    }

    return (
        <div className="container flex items-center justify-center min-h-[80vh]">
            <Card className="w-full max-w-md mx-auto shadow-lg">
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-xl font-bold">Профіль: {user.userName}</CardTitle>
                </CardHeader>

                <CardContent className="space-y-6">
                    <div className="space-y-1">
                        <p className="text-sm text-muted-foreground">Контактні дані</p>
                        <p>
                            <strong>Телефон:</strong> {user.phoneNumber ?? '—'}
                        </p>
                        <div className="flex gap-2 flex-wrap mt-2">
                            {user.enabledTwoFactorMethods.length > 0 ? (
                                user.enabledTwoFactorMethods.map((m) => (
                                    <Badge key={m} variant="secondary">
                                        {methodLabel[m]}
                                    </Badge>
                                ))
                            ) : (
                                <span className="text-xs text-muted-foreground italic">2FA не активовано</span>
                            )}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-3 pt-4 border-t">
                        <p className="text-sm font-medium text-muted-foreground mb-1">Безпека</p>
                        <Button
                            variant={hasSms ? 'outline' : 'default'}
                            onClick={() => setSmsOpen(true)}
                            disabled={hasSms}
                        >
                            {hasSms ? 'SMS активовано' : 'Підключити SMS 2FA'}
                        </Button>
                        <Button
                            variant={hasTotp ? 'outline' : 'default'}
                            onClick={() => setTotpOpen(true)}
                            disabled={hasTotp}
                        >
                            {hasTotp ? 'App активовано' : 'Підключити Authenticator App'}
                        </Button>
                    </div>

                    <div className="grid grid-cols-1 gap-3 pt-4 border-t">
                        <p className="text-sm font-medium text-muted-foreground mb-1">Push сповіщення</p>
                        <Button variant="outline" className="gap-2" onClick={subscribe}>
                            <Bell className="h-4 w-4" />
                            Підписатись на сповіщення
                        </Button>
                    </div>

                    <div className="space-y-3 pt-4 border-t">
                        <p className="text-sm font-medium text-muted-foreground">Надіслати користувачу</p>
                        <Input
                            placeholder="User ID"
                            value={targetUserId}
                            onChange={(e) => setTargetUserId(e.target.value)}
                        />
                        <Input
                            placeholder="Заголовок"
                            value={sendTitle}
                            onChange={(e) => setSendTitle(e.target.value)}
                        />
                        <Textarea
                            placeholder="Текст повідомлення"
                            value={sendBody}
                            onChange={(e) => setSendBody(e.target.value)}
                            rows={3}
                        />
                        <Button
                            className="w-full gap-2"
                            onClick={handleSend}
                            disabled={isSending || !targetUserId || !sendTitle || !sendBody}
                        >
                            <Send className="h-4 w-4" />
                            {isSending ? 'Надсилання...' : 'Надіслати'}
                        </Button>
                    </div>

                    <div className="space-y-3 pt-4 border-t">
                        <p className="text-sm font-medium text-muted-foreground">Розсилка всім</p>
                        <Input
                            placeholder="Заголовок"
                            value={broadcastTitle}
                            onChange={(e) => setBroadcastTitle(e.target.value)}
                        />
                        <Textarea
                            placeholder="Текст повідомлення"
                            value={broadcastBody}
                            onChange={(e) => setBroadcastBody(e.target.value)}
                            rows={3}
                        />
                        <Button
                            className="w-full gap-2"
                            variant="secondary"
                            onClick={handleBroadcast}
                            disabled={isBroadcasting || !broadcastTitle || !broadcastBody}
                        >
                            <Radio className="h-4 w-4" />
                            {isBroadcasting ? 'Розсилка...' : 'Розіслати всім'}
                        </Button>
                    </div>
                </CardContent>

                <CardFooter className="flex justify-end pt-2 border-t mt-4 bg-slate-50/50 rounded-b-lg">
                    <Button
                        variant="ghost"
                        className="text-destructive hover:text-destructive hover:bg-destructive/10 gap-2"
                        onClick={logout}
                    >
                        <LogOut className="h-4 w-4" />
                        Вийти
                    </Button>
                </CardFooter>

                <SmsSetupDialog open={isSmsOpen} onOpenChange={setSmsOpen} />
                <TotpSetupDialog open={isTotpOpen} onOpenChange={setTotpOpen} />
            </Card>
        </div>
    )
}
