import { useState } from 'react'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { api } from '@/shared'
import { useUserStore } from '@/entities/user'
import { QRCodeSVG } from 'qrcode.react'

export const TotpSetupDialog = ({ open, onOpenChange }: { open: boolean; onOpenChange: (v: boolean) => void }) => {
    const [step, setStep] = useState<'init' | 'verify'>('init')
    const [qrUri, setQrUri] = useState('')
    const [code, setCode] = useState('')
    const fetchProfile = useUserStore((s) => s.fetchProfile)

    const startSetup = async () => {
        try {
            const { data } = await api.post<{ authenticatorUri: string }>('/2fa/totp/setup')
            setQrUri(data.authenticatorUri)
            setStep('verify')
        } catch (e) {
            console.error('Setup failed', e)
        }
    }

    const enableTotp = async () => {
        try {
            await api.post('/2fa/totp/enable', { code })
            await fetchProfile()
            onOpenChange(false)
            setStep('init')
            setCode('')
        } catch {
            alert('Невірний код')
        }
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Налаштування Authenticator</DialogTitle>
                </DialogHeader>
                {step === 'init' ? (
                    <Button onClick={startSetup} className="w-full">
                        Отримати QR-код
                    </Button>
                ) : (
                    <div className="space-y-4 flex flex-col items-center">
                        {qrUri && (
                            <div className="bg-white p-4 border rounded-xl">
                                <QRCodeSVG value={qrUri} size={200} level="M" includeMargin={false} />
                            </div>
                        )}
                        <p className="text-sm text-center text-muted-foreground">
                            Відскануйте цей код у Google Authenticator або іншому додатку
                        </p>
                        <Input
                            value={code}
                            onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))}
                            placeholder="000 000"
                            className="text-center font-mono text-lg tracking-widest"
                            maxLength={6}
                        />
                        <Button onClick={enableTotp} className="w-full">
                            Підтвердити та активувати
                        </Button>
                    </div>
                )}
            </DialogContent>
        </Dialog>
    )
}
