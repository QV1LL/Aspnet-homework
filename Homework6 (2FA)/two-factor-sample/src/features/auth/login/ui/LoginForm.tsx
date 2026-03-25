import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { useNavigate } from 'react-router-dom'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { loginUser } from '../model/loginService'
import { type TwoFactorMethod } from '@/entities/user'

const loginSchema = z.object({
    userName: z.string().min(1, 'Username is required'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
})

type LoginFormValues = z.infer<typeof loginSchema>

interface LoginFormProps {
    onRequires2FA: (userId: string, availableMethods: TwoFactorMethod[]) => void
}

export const LoginForm = ({ onRequires2FA }: LoginFormProps) => {
    const navigate = useNavigate()

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm<LoginFormValues>({
        resolver: zodResolver(loginSchema),
    })

    const onSubmit = async (values: LoginFormValues) => {
        try {
            const result = await loginUser(values)

            if (result.status === 'requires_2fa' && result.userId) {
                onRequires2FA(result.userId, result.availableMethods ?? [])
                return
            }

            if (result.status === 'success') {
                navigate('/profile')
            }
        } catch (e) {
            if (e instanceof Error) {
                setError('root', { message: e.message })
            }
        }
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
            <div className="grid gap-2">
                <Label htmlFor="userName">Username</Label>
                <Input
                    id="userName"
                    placeholder="admin"
                    {...register('userName')}
                    className={errors.userName ? 'border-destructive' : ''}
                />
                {errors.userName && <span className="text-xs text-destructive">{errors.userName.message}</span>}
            </div>

            <div className="grid gap-2">
                <Label htmlFor="password">Password</Label>
                <Input
                    id="password"
                    type="password"
                    {...register('password')}
                    className={errors.password ? 'border-destructive' : ''}
                />
                {errors.password && <span className="text-xs text-destructive">{errors.password.message}</span>}
            </div>

            {errors.root && <p className="text-sm text-destructive text-center">{errors.root.message}</p>}

            <Button type="submit" disabled={isSubmitting} className="w-full">
                {isSubmitting ? 'Logging in...' : 'Login'}
            </Button>
        </form>
    )
}
