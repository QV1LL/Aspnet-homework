import { createBrowserRouter, Navigate } from 'react-router'
import { BaseLayout } from '../layouts/BaseLayout'
import { HomePageRoute } from '@/pages/home'
import { PostDetailsPageRoute } from '@/pages/post-details'
import { AdminCategoriesPageRoute } from '@/pages/admin-categories'
import { AdminPostsPageRoute } from '@/pages/admin-posts'

export const router = createBrowserRouter([
    {
        path: '/',
        element: <BaseLayout />,
        children: [
            HomePageRoute,
            PostDetailsPageRoute,
            AdminCategoriesPageRoute,
            AdminPostsPageRoute,
            {
                path: '*',
                element: <Navigate to="/" replace />,
            },
        ],
    },
])
