import { categoriesReducer } from '@/features/category'
import { postsReducer } from '@/features/post'
import { configureStore } from '@reduxjs/toolkit'

export const store = configureStore({
    reducer: {
        posts: postsReducer,
        categories: categoriesReducer,
    },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
