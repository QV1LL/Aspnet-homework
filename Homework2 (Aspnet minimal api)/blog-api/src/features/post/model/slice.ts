import { createEntityAdapter, createSlice, type PayloadAction } from '@reduxjs/toolkit'
import type { Post } from './types'
import type { Category } from '@/features/category'
import { fetchPosts } from './actions'
import type { RootState } from '@/app'

const postsAdapter = createEntityAdapter<Post>({
    sortComparer: (a, b) => a.title.localeCompare(b.title),
})

const initialState = postsAdapter.getInitialState({
    status: 'idle' as 'loading' | 'idle' | 'loaded' | 'error',
    error: null as string | null,
    hasMore: true,
    filters: {
        search: '',
        category: 'All' as Category | 'All',
    },
})

const postsSlice = createSlice({
    name: 'posts',
    initialState,
    reducers: {
        setFilters: (state, action: PayloadAction<Partial<typeof initialState.filters>>) => {
            state.filters = { ...state.filters, ...action.payload }
            postsAdapter.removeAll(state)
        },
        postAdded: postsAdapter.addOne,
        postUpdated: postsAdapter.updateOne,
        postRemoved: postsAdapter.removeOne,
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchPosts.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(fetchPosts.fulfilled, (state, action) => {
                state.status = 'idle'

                if (action.meta.arg.page === 1) {
                    postsAdapter.setAll(state, action.payload)
                } else {
                    postsAdapter.addMany(state, action.payload)
                }

                if (action.payload.length < action.meta.arg.size) {
                    state.hasMore = false
                } else {
                    state.hasMore = true
                }
            })
            .addCase(fetchPosts.rejected, (state, action) => {
                state.status = 'error'
                state.error = action.payload as string
            })
    },
})

export const { setFilters, postAdded, postUpdated, postRemoved } = postsSlice.actions
export const postsReducer = postsSlice.reducer

export const postsSelectors = postsAdapter.getSelectors((state: RootState) => state.posts)
