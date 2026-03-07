import { createEntityAdapter, createSlice, type PayloadAction } from '@reduxjs/toolkit'
import type { Post } from './types'
import type { Category } from '@/features/category'
import { createPost, deletePost, fetchPosts, updatePost } from './actions'
import type { RootState } from '@/app'

const postsAdapter = createEntityAdapter<Post>({
    sortComparer: (a, b) => a.title.localeCompare(b.title),
})

const postsSlice = createSlice({
    name: 'posts',
    initialState: postsAdapter.getInitialState({
        status: 'idle' as 'loading' | 'idle' | 'error',
        error: null as string | null,
        totalCount: 0,
        currentPage: 1,
        filters: {
            search: '',
            category: 'All' as Category | 'All',
        },
    }),
    reducers: {
        setFilters: (state, action: PayloadAction<Partial<{ search: string; category: Category | 'All' }>>) => {
            state.filters = { ...state.filters, ...action.payload }
            state.currentPage = 1
            state.totalCount = 0
            postsAdapter.removeAll(state)
        },
        setPage: (state, action: PayloadAction<number>) => {
            state.currentPage = action.payload
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
                state.totalCount = action.payload.totalCount

                if (action.meta.arg.page === 1) {
                    postsAdapter.setAll(state, action.payload.items)
                } else {
                    postsAdapter.addMany(state, action.payload.items)
                }
            })
            .addCase(fetchPosts.rejected, (state, action) => {
                state.status = 'error'
                state.error = action.payload as string
            })
            .addCase(createPost.fulfilled, (state, action) => {
                postsAdapter.addOne(state, action.payload)
                state.totalCount += 1
            })
            .addCase(updatePost.fulfilled, (state, action) => {
                postsAdapter.upsertOne(state, action.payload)
            })
            .addCase(deletePost.fulfilled, (state, action) => {
                postsAdapter.removeOne(state, action.payload)
                state.totalCount -= 1
            })
    },
})

export const { setFilters, setPage, postAdded, postUpdated, postRemoved } = postsSlice.actions
export const postsReducer = postsSlice.reducer

export const { selectAll: selectAllPosts, selectById: selectPostById } = postsAdapter.getSelectors(
    (state: RootState) => state.posts,
)

export const selectPostsMeta = (state: RootState) => ({
    status: state.posts.status,
    currentPage: state.posts.currentPage,
    totalCount: state.posts.totalCount,
    filters: state.posts.filters,
})
