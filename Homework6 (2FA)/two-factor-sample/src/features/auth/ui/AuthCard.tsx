import { useState } from 'react'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { LoginForm } from '../login/ui/LoginForm'
import { RegisterForm } from '../register/ui/RegisterForm'
import { Verify2FAForm } from '../verify/ui/Verify2FAForm'
import type { TwoFactorMethod } from '@/entities/user'

type Step = 'auth' | '2fa'

interface TwoFactorData {
    userId: string
    availableMethods: TwoFactorMethod[]
}

export const AuthCard = () => {
    const [step, setStep] = useState<Step>('auth')
    const [twoFactorData, setTwoFactorData] = useState<TwoFactorData | null>(null)

    const handleLoginRequires2FA = (userId: string, availableMethods: TwoFactorMethod[]) => {
        setTwoFactorData({ userId, availableMethods })
        setStep('2fa')
    }

    if (step === '2fa' && twoFactorData) {
        return (
            <Card className="w-full max-w-md shadow-lg">
                <CardHeader className="text-center">
                    <CardTitle className="text-2xl font-bold">Verification</CardTitle>
                    <CardDescription>Secure your session</CardDescription>
                </CardHeader>
                <CardContent>
                    <Verify2FAForm
                        userId={twoFactorData.userId}
                        availableMethods={twoFactorData.availableMethods}
                        onBack={() => setStep('auth')}
                    />
                </CardContent>
            </Card>
        )
    }

    return (
        <Card className="w-full max-w-md shadow-lg">
            <CardHeader className="text-center">
                <CardTitle className="text-2xl font-bold">Welcome</CardTitle>
            </CardHeader>
            <CardContent>
                <Tabs defaultValue="login" className="w-full">
                    <TabsList className="grid w-full grid-cols-2 mb-8">
                        <TabsTrigger value="login">Login</TabsTrigger>
                        <TabsTrigger value="register">Register</TabsTrigger>
                    </TabsList>
                    <TabsContent value="login">
                        <LoginForm onRequires2FA={handleLoginRequires2FA} />
                    </TabsContent>
                    <TabsContent value="register">
                        <RegisterForm />
                    </TabsContent>
                </Tabs>
            </CardContent>
        </Card>
    )
}
