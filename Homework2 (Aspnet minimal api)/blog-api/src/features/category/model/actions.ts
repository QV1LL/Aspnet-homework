import { createAsyncThunk } from '@reduxjs/toolkit'
import type { Category, CreateCategory, UpdateCategory } from './types'
import { api } from '@/app'
import { isAxiosError } from 'axios'

export const fetchCategories = createAsyncThunk<Category[], undefined, { rejectValue: string }>(
    'categories/fetchCategories',
    async (_, { rejectWithValue }) => {
        try {
            const response = await api.get<Category[]>('/categories')
            return response.data
        } catch (error: unknown) {
            if (isAxiosError(error)) {
                return rejectWithValue(error.response?.data || 'Failed to fetch categories')
            }
            return rejectWithValue('An unexpected error occurred')
        }
    },
)

export const createCategory = createAsyncThunk<Category, CreateCategory>('categories/create', async (dto) => {
    const res = await api.post<Category>('/categories', dto)
    return res.data
})

export const updateCategory = createAsyncThunk<Category, UpdateCategory>('categories/update', async (dto) => {
    await api.put('/categories', dto)
    return { id: dto.id, name: dto.name } as Category
})

export const deleteCategory = createAsyncThunk<string, string>('categories/delete', async (id) => {
    await api.delete(`/categories/${id}`)
    return id
})
