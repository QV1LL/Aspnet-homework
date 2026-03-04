import { createAsyncThunk } from '@reduxjs/toolkit'
import { isAxiosError } from 'axios'
import { api } from '@/app'
import type { Post } from './types'

export const fetchPosts = createAsyncThunk<
    Post[],
    { page: number; size: number; search?: string; category?: string },
    { rejectValue: string }
>('posts/fetchPosts', async ({ page, size, search, category }, { rejectWithValue }) => {
    try {
        const response = await api.get<Post[]>('/posts', {
            params: {
                _page: page,
                _limit: size,
                q: search || undefined,
                category: category === 'All' ? undefined : category,
            },
        })
        return response.data
    } catch (error: unknown) {
        if (isAxiosError(error)) {
            return rejectWithValue(error.response?.data || 'Failed to fetch posts')
        }
        return rejectWithValue('An unexpected error occurred')
    }
})
