import { AuthCard } from '@/features/auth'

export const AuthPage = () => {
    return (
        <main className="min-h-screen w-full flex items-center justify-center bg-slate-50">
            <div className="w-full max-w-md animate-in fade-in zoom-in duration-300">
                <AuthCard />
            </div>
        </main>
    )
}
