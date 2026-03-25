import { useState, useEffect } from 'react'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { type TwoFactorMethod } from '@/entities/user'
import { Smartphone, ShieldCheck, Loader2 } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { sendSmsCode, verifyTwoFactor } from '../../login'

interface Verify2FAFormProps {
    userId: string
    availableMethods: TwoFactorMethod[]
    onBack: () => void
}

export const Verify2FAForm = ({ userId, availableMethods, onBack }: Verify2FAFormProps) => {
    const [selectedMethod, setSelectedMethod] = useState<TwoFactorMethod>(availableMethods[0])
    const [code, setCode] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isLoading, setIsLoading] = useState(false)
    const [isSendingSms, setIsSendingSms] = useState(false)
    const navigate = useNavigate()

    useEffect(() => {
        if (selectedMethod === 'Sms') {
            handleSendSms()
        }
    }, [selectedMethod])

    const handleSendSms = async () => {
        try {
            setIsSendingSms(true)
            setError(null)
            await sendSmsCode(userId)
        } catch {
            setError('Failed to send SMS. Please try again.')
        } finally {
            setIsSendingSms(false)
        }
    }

    const handleVerify = async () => {
        if (code.length !== 6) {
            setError('Code must be exactly 6 digits')
            return
        }

        try {
            setIsLoading(true)
            setError(null)
            await verifyTwoFactor({ userId, method: selectedMethod, code })
            navigate('/profile')
        } catch {
            setError('Invalid verification code')
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <div className="grid gap-6">
            <div className="text-center space-y-2">
                <h2 className="text-lg font-semibold tracking-tight">Two-step verification</h2>
                <p className="text-sm text-muted-foreground">Select a method and enter the code</p>
            </div>

            <div className="grid grid-cols-2 gap-3">
                {availableMethods.map((method) => (
                    <button
                        key={method}
                        type="button"
                        onClick={() => {
                            setSelectedMethod(method)
                            setCode('')
                            setError(null)
                        }}
                        className={`flex flex-col items-center gap-2 p-3 rounded-xl border-2 transition-all ${
                            selectedMethod === method
                                ? 'border-primary bg-primary/5'
                                : 'border-muted hover:border-muted-foreground/30'
                        }`}
                    >
                        {method === 'Sms' ? <Smartphone className="h-5 w-5" /> : <ShieldCheck className="h-5 w-5" />}
                        <span className="text-xs font-medium">{method === 'Sms' ? 'SMS' : 'App'}</span>
                    </button>
                ))}
            </div>

            <div className="space-y-4">
                <div className="grid gap-2">
                    <div className="flex justify-between items-center">
                        <Label>Verification Code</Label>
                        {selectedMethod === 'Sms' && (
                            <button
                                onClick={handleSendSms}
                                disabled={isSendingSms}
                                className="text-[10px] uppercase font-bold text-primary hover:underline disabled:opacity-50"
                            >
                                {isSendingSms ? 'Sending...' : 'Resend SMS'}
                            </button>
                        )}
                    </div>
                    <Input
                        value={code}
                        onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))}
                        placeholder="000 000"
                        className="text-center text-lg tracking-[0.5em] font-mono"
                        maxLength={6}
                    />
                    {error && <p className="text-xs text-destructive text-center">{error}</p>}
                </div>

                <Button onClick={handleVerify} className="w-full" disabled={isLoading || isSendingSms}>
                    {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                    Verify identity
                </Button>

                <Button variant="ghost" onClick={onBack} className="w-full text-xs">
                    Cancel and go back
                </Button>
            </div>
        </div>
    )
}
