import { postsReducer } from '@/features/post/model/slice'
import { configureStore } from '@reduxjs/toolkit'

export const store = configureStore({
    reducer: {
        posts: postsReducer,
    },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
