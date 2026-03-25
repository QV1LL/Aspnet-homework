import { useEffect, useState } from 'react'
import { useUserStore } from '@/entities/user'
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { SmsSetupDialog, TotpSetupDialog } from '@/features/auth'
import { LogOut } from 'lucide-react'

const methodLabel: Record<string, string> = {
    Sms: 'SMS',
    Totp: 'Authenticator App',
}

export const ProfilePage = () => {
    const { user, isLoading, fetchProfile, logout } = useUserStore()
    const [isSmsOpen, setSmsOpen] = useState(false)
    const [isTotpOpen, setTotpOpen] = useState(false)

    useEffect(() => {
        fetchProfile()
    }, [fetchProfile])

    if (isLoading && !user) return <Skeleton className="h-[400px] w-full" />
    if (!user) return <div className="p-10 text-center">Будь ласка, залогіньтесь знову.</div>

    const hasSms = user.enabledTwoFactorMethods.includes('Sms')
    const hasTotp = user.enabledTwoFactorMethods.includes('Totp')

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
