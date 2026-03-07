import { createAsyncThunk } from '@reduxjs/toolkit'
import { isAxiosError } from 'axios'
import { api } from '@/app'
import type { CreatePost, Post, UpdatePost } from './types'
import type { Category } from '@/features/category'

export const fetchPosts = createAsyncThunk<
    { items: Post[]; totalCount: number },
    { page: number; size: number; search?: string; category?: 'All' | Category },
    { rejectValue: string }
>('posts/fetchPosts', async ({ page, size, search, category: categoryId }, { rejectWithValue }) => {
    try {
        const response = await api.get<{ items: Post[]; totalCount: number }>('/posts', {
            params: {
                page,
                size,
                q: search || undefined,
                categoryId: categoryId === 'All' ? undefined : categoryId,
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

export const createPost = createAsyncThunk<Post, CreatePost>('posts/create', async (dto) => {
    const res = await api.post<Post>('/posts', dto)
    return res.data
})

export const updatePost = createAsyncThunk<Post, UpdatePost>('posts/update', async (dto) => {
    await api.put('/posts', dto)
    const res = await api.get<Post>(`/posts/${dto.id}`)
    return res.data
})

export const deletePost = createAsyncThunk<string, string>('posts/delete', async (id) => {
    await api.delete(`/posts/${id}`)
    return id
})
