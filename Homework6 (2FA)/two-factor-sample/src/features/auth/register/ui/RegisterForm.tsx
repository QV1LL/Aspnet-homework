import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { registerUser } from '../model/registerService'

const registerSchema = z
    .object({
        userName: z.string().min(2, 'Username is too short'),
        phoneNumber: z.string().min(10, 'Invalid phone number format'),
        password: z.string().min(6, 'Password must be at least 6 characters'),
        confirmPassword: z.string(),
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords don't match",
        path: ['confirmPassword'],
    })

type RegisterFormValues = z.infer<typeof registerSchema>

export const RegisterForm = () => {
    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm<RegisterFormValues>({
        resolver: zodResolver(registerSchema),
    })

    const onSubmit = async (values: RegisterFormValues) => {
        try {
            const { ...requestData } = values
            await registerUser(requestData)

            alert('Account created! You can now log in.')
        } catch (e: unknown) {
            const error = e as Error
            if (error) setError('root', { message: error.message })
        }
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
            <div className="grid gap-2">
                <Label htmlFor="reg-user">Username</Label>
                <Input
                    id="reg-user"
                    placeholder="johndoe"
                    {...register('userName')}
                    className={errors.userName ? 'border-destructive' : ''}
                />
                {errors.userName && <span className="text-xs text-destructive">{errors.userName.message}</span>}
            </div>

            <div className="grid gap-2">
                <Label htmlFor="reg-phone">Phone Number</Label>
                <Input
                    id="reg-phone"
                    type="tel"
                    placeholder="+380..."
                    {...register('phoneNumber')}
                    className={errors.phoneNumber ? 'border-destructive' : ''}
                />
                {errors.phoneNumber && <span className="text-xs text-destructive">{errors.phoneNumber.message}</span>}
            </div>

            <div className="grid gap-2">
                <Label htmlFor="reg-pass">Password</Label>
                <Input
                    id="reg-pass"
                    type="password"
                    {...register('password')}
                    className={errors.password ? 'border-destructive' : ''}
                />
                {errors.password && <span className="text-xs text-destructive">{errors.password.message}</span>}
            </div>

            <div className="grid gap-2">
                <Label htmlFor="reg-confirm">Confirm Password</Label>
                <Input
                    id="reg-confirm"
                    type="password"
                    {...register('confirmPassword')}
                    className={errors.confirmPassword ? 'border-destructive' : ''}
                />
                {errors.confirmPassword && (
                    <span className="text-xs text-destructive">{errors.confirmPassword.message}</span>
                )}
            </div>

            {errors.root && <p className="text-sm text-destructive text-center">{errors.root.message}</p>}

            <Button type="submit" disabled={isSubmitting} variant="outline" className="w-full">
                {isSubmitting ? 'Creating account...' : 'Create Account'}
            </Button>
        </form>
    )
}
