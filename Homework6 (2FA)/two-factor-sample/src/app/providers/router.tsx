import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom'
import { useUserStore } from '@/entities/user'
import { AuthPage } from '@/pages/auth'
import { ProfilePage } from '@/pages/profile'

// eslint-disable-next-line react-refresh/only-export-components
const ProtectedRoute = () => {
    const isAuth = useUserStore((state) => !!state.token)
    return isAuth ? <Outlet /> : <Navigate to="/auth" replace />
}

// eslint-disable-next-line react-refresh/only-export-components
const PublicOnlyRoute = () => {
    const isAuth = useUserStore((state) => !!state.token)
    return isAuth ? <Navigate to="/profile" replace /> : <Outlet />
}

export const router = createBrowserRouter([
    {
        element: <PublicOnlyRoute />,
        children: [
            {
                path: '/auth',
                element: <AuthPage />,
            },
        ],
    },
    {
        element: <ProtectedRoute />,
        children: [
            {
                path: '/profile',
                element: <ProfilePage />,
            },
        ],
    },
    {
        path: '/',
        element: <Navigate to="/auth" replace />,
    },
    {
        path: '*',
        element: <div className="flex h-screen items-center justify-center">404 - Not Found</div>,
    },
])
