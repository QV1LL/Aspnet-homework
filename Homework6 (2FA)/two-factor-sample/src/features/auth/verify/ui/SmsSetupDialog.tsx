import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { useUserStore } from '@/entities/user'
import { api } from '@/shared'
import { useState } from 'react'

export const SmsSetupDialog = ({ open, onOpenChange }: { open: boolean; onOpenChange: (v: boolean) => void }) => {
    const [step, setStep] = useState<'init' | 'verify'>('init')
    const [code, setCode] = useState('')
    const fetchProfile = useUserStore((s) => s.fetchProfile)

    const startSmsSetup = async () => {
        try {
            await api.post('/2fa/sms/setup')
            setStep('verify')
        } catch {
            alert('Помилка відправки SMS. Перевірте номер телефону.')
        }
    }

    const enableSms = async () => {
        try {
            await api.post('/2fa/sms/enable', { code })
            await fetchProfile()
            onOpenChange(false)
        } catch {
            alert('Невірний код з SMS')
        }
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Налаштування SMS 2FA</DialogTitle>
                </DialogHeader>
                {step === 'init' ? (
                    <div className="space-y-4">
                        <p className="text-sm text-muted-foreground">
                            Ми надішлемо код підтвердження на ваш номер телефону.
                        </p>
                        <Button onClick={startSmsSetup} className="w-full">
                            Надіслати код
                        </Button>
                    </div>
                ) : (
                    <div className="space-y-4 pt-4">
                        <Input
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            placeholder="Код із SMS"
                            maxLength={6}
                        />
                        <Button onClick={enableSms} className="w-full">
                            Активувати
                        </Button>
                    </div>
                )}
            </DialogContent>
        </Dialog>
    )
}
