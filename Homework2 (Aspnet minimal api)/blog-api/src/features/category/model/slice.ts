import { createEntityAdapter, createSlice } from '@reduxjs/toolkit'
import type { Category } from './types'
import { createCategory, deleteCategory, fetchCategories, updateCategory } from './actions'
import type { RootState } from '@/app'

const categoriesAdapter = createEntityAdapter<Category>({
    sortComparer: (a, b) => a.name.localeCompare(b.name),
})

const categoriesSlice = createSlice({
    name: 'categories',
    initialState: categoriesAdapter.getInitialState({
        status: 'idle' as 'idle' | 'pending' | 'loaded' | 'error',
    }),
    reducers: {
        categoryAdded: categoriesAdapter.addOne,
        categoryRemoved: categoriesAdapter.removeOne,
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchCategories.pending, (state) => {
                state.status = 'pending'
            })
            .addCase(fetchCategories.fulfilled, (state, action) => {
                state.status = 'loaded'
                categoriesAdapter.setAll(state, action.payload)
            })
            .addCase(fetchCategories.rejected, (state) => {
                state.status = 'error'
            })
            .addCase(createCategory.fulfilled, (state, action) => {
                categoriesAdapter.addOne(state, action.payload)
            })
            .addCase(updateCategory.fulfilled, (state, action) => {
                categoriesAdapter.updateOne(state, { id: action.payload.id, changes: action.payload })
            })
            .addCase(deleteCategory.fulfilled, (state, action) => {
                categoriesAdapter.removeOne(state, action.payload)
            })
    },
})

export const categoriesReducer = categoriesSlice.reducer
export const { categoryAdded, categoryRemoved } = categoriesSlice.actions

export const { selectAll: selectAllCategories } = categoriesAdapter.getSelectors((state: RootState) => state.categories)
